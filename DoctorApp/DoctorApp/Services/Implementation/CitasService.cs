using FluentValidation;
using DoctorApp.DTOs.Requests;
using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using DoctorApp.Services.ApiClient;
using DoctorApp.Services.Interfaces;
using DoctorApp.Validators;

namespace DoctorApp.Services.Implementation;

/// <summary>
/// Citas service - consumes /api/citas endpoints
/// </summary>
public class CitasService : ICitasService
{
    private readonly IApiClient _apiClient;
    private readonly ConfirmarCitaValidator _confirmarValidator;
    private readonly IniciarConsultaValidator _iniciarValidator;
    private readonly MarcarAsistenciaValidator _asistenciaValidator;

    public CitasService(
        IApiClient apiClient,
        ConfirmarCitaValidator confirmarValidator,
        IniciarConsultaValidator iniciarValidator,
        MarcarAsistenciaValidator asistenciaValidator)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _confirmarValidator = confirmarValidator;
        _iniciarValidator = iniciarValidator;
        _asistenciaValidator = asistenciaValidator;
    }

    public async Task<ObtenerAgendaResponse> ObtenerAgendaAsync(DateTime fecha)
    {
        try
        {
            var endpoint = $"/api/citas/medico/agenda?fecha={fecha:yyyy-MM-dd}";
            var citas = await _apiClient.GetAsync<List<CitaResponseDto>>(endpoint)
                ?? new List<CitaResponseDto>();

            var confirmadas = citas.Count(c =>
                c.Estado.Equals("Confirmada", StringComparison.OrdinalIgnoreCase) ||
                c.Estado.Equals("Completada", StringComparison.OrdinalIgnoreCase));

            var pendientes = citas.Count(c =>
                c.Estado.Equals("Solicitada", StringComparison.OrdinalIgnoreCase) ||
                c.Estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase));

            return new ObtenerAgendaResponse
            {
                Fecha = fecha,
                Citas = citas,
                TotalCitas = citas.Count,
                CitasConfirmadas = confirmadas,
                CitasPendientes = pendientes
            };
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al obtener agenda: {ex.Message}", ex);
        }
    }

    public async Task<List<CitaResponseDto>> ObtenerCitasMedicoAsync()
    {
        try
        {
            const string endpoint = "/api/citas/medico";
            var citas = await _apiClient.GetAsync<List<CitaResponseDto>>(endpoint);
            return citas ?? new List<CitaResponseDto>();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al obtener citas del medico: {ex.Message}", ex);
        }
    }

    public async Task<CitaResponseDto> ConfirmarCitaAsync(int citaId, bool confirmada, string? notas = null)
    {
        var request = new ConfirmarCitaRequest
        {
            CitaId = citaId,
            Confirmada = confirmada,
            Notas = notas
        };

        var validationResult = await _confirmarValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException("Error de validacion", errors);
        }

        try
        {
            var endpoint = $"/api/citas/{citaId}/confirmar";
            var response = await _apiClient.PutAsync<CitaResponseDto>(endpoint, request);
            return response ?? new CitaResponseDto { Id = citaId, Estado = "Confirmada" };
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al confirmar cita: {ex.Message}", ex);
        }
    }

    public async Task<CitaResponseDto> CancelarCitaAsync(int citaId, string? motivo = null)
    {
        if (citaId <= 0)
            throw new Exceptions.ValidationException("El ID de la cita es requerido");

        var payload = new
        {
            motivo = string.IsNullOrWhiteSpace(motivo) ? "Cancelada por el medico" : motivo.Trim()
        };

        try
        {
            var endpoint = $"/api/citas/{citaId}/cancelar";
            await _apiClient.PutAsync<object>(endpoint, payload);

            var actualizada = await ObtenerCitaPorIdAsync(citaId);
            return actualizada ?? new CitaResponseDto { Id = citaId, Estado = "Cancelada" };
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al cancelar cita: {ex.Message}", ex);
        }
    }

    public async Task<CitaResponseDto> IniciarConsultaAsync(int citaId)
    {
        var request = new IniciarConsultaRequest { CitaId = citaId };

        var validationResult = await _iniciarValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException("Error de validacion", errors);
        }

        try
        {
            var endpoint = $"/api/citas/{citaId}/iniciar-consulta";
            var response = await _apiClient.PutAsync<CitaResponseDto>(endpoint, request);
            return response ?? new CitaResponseDto { Id = citaId, Estado = "EnProgreso" };
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al iniciar consulta: {ex.Message}", ex);
        }
    }

    public async Task<CitaResponseDto> MarcarAsistenciaAsync(int citaId, bool asistio, string? observaciones = null)
    {
        var request = new MarcarAsistenciaRequest
        {
            CitaId = citaId,
            Asistio = asistio,
            Observaciones = observaciones
        };

        var validationResult = await _asistenciaValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException("Error de validacion", errors);
        }

        try
        {
            await EjecutarMarcarAsistenciaAsync(citaId, request);

            var actualizada = await ObtenerCitaPorIdAsync(citaId);
            return actualizada ?? new CitaResponseDto { Id = citaId, Estado = asistio ? "Completada" : "NoAsistio" };
        }
        catch (ConflictException) when (asistio)
        {
            await IntentarCompletarConTransicionesAsync(citaId, request);

            var actualizada = await ObtenerCitaPorIdAsync(citaId);
            return actualizada ?? new CitaResponseDto { Id = citaId, Estado = "Completada" };
        }
        catch (ConflictException) when (!asistio)
        {
            await IntentarMarcarNoAsistioConTransicionesAsync(citaId, request);

            var actualizada = await ObtenerCitaPorIdAsync(citaId);
            return actualizada ?? new CitaResponseDto { Id = citaId, Estado = "NoAsistio" };
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al marcar asistencia: {ex.Message}", ex);
        }
    }

    private async Task EjecutarMarcarAsistenciaAsync(int citaId, MarcarAsistenciaRequest request)
    {
        var endpoint = $"/api/citas/{citaId}/asistencia?asistio={request.Asistio}";
        await _apiClient.PutAsync<object>(endpoint, request);
    }

    private async Task IntentarCompletarConTransicionesAsync(int citaId, MarcarAsistenciaRequest request)
    {
        try
        {
            await IniciarConsultaAsync(citaId);
        }
        catch (ConflictException)
        {
            try
            {
                await ConfirmarCitaAsync(citaId, true);
            }
            catch (ConflictException)
            {
                // Puede estar ya confirmada o en otro estado.
            }

            await IniciarConsultaAsync(citaId);
        }

        await EjecutarMarcarAsistenciaAsync(citaId, request);
    }

    private async Task IntentarMarcarNoAsistioConTransicionesAsync(int citaId, MarcarAsistenciaRequest request)
    {
        try
        {
            await ConfirmarCitaAsync(citaId, true);
        }
        catch (ConflictException)
        {
            // Puede estar ya confirmada o en estado no apto.
        }

        await EjecutarMarcarAsistenciaAsync(citaId, request);
    }

    public async Task<List<CitaResponseDto>> ObtenerCitasDelDiaAsync()
    {
        try
        {
            var agenda = await ObtenerAgendaAsync(DateTime.Now.Date);
            return agenda.Citas ?? new List<CitaResponseDto>();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al obtener citas del dia: {ex.Message}", ex);
        }
    }

    public async Task<CitaResponseDto?> ObtenerCitaPorIdAsync(int citaId)
    {
        try
        {
            var endpoint = $"/api/citas/{citaId}";
            return await _apiClient.GetAsync<CitaResponseDto>(endpoint);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al obtener cita: {ex.Message}", ex);
        }
    }
}
