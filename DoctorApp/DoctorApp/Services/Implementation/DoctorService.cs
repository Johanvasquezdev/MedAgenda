using FluentValidation;
using DoctorApp.DTOs.Requests;
using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using DoctorApp.Services.ApiClient;
using DoctorApp.Services.Interfaces;
using DoctorApp.Validators;

namespace DoctorApp.Services.Implementation;

/// <summary>
/// Doctor service - consumes /api/medicos endpoints
/// Handles register, fetch, update, and doctor lookups
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly IApiClient _apiClient;
    private readonly RegistrarDoctorValidator _registrarValidator;
    private int? _cachedDoctorId;

    public DoctorService(
        IApiClient apiClient,
        RegistrarDoctorValidator registrarValidator)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _registrarValidator = registrarValidator ?? throw new ArgumentNullException(nameof(registrarValidator));
    }

    /// <summary>
    /// Gets doctor appointments. This API uses the authenticated doctor token.
    /// </summary>
    public async Task<List<CitaResponseDto>> GetCitasByDoctorIdAsync(int doctorId)
    {
        if (doctorId <= 0)
            throw new AppException("Doctor ID invalido", "INVALID_DOCTOR_ID", 400);

        try
        {
            var endpoint = $"/api/citas/medico/agenda?fecha={DateTime.Today:yyyy-MM-dd}";
            System.Diagnostics.Debug.WriteLine($"[DoctorService] Obteniendo citas desde {endpoint}");

            var response = await _apiClient.GetAsync<List<CitaResponseDto>>(endpoint);
            return response ?? new List<CitaResponseDto>();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error inesperado al obtener citas del doctor: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Registers a new doctor
    /// </summary>
    public async Task<DoctorResponseDto> RegistrarDoctorAsync(DoctorRequestDto datos)
    {
        var validationResult = await _registrarValidator.ValidateAsync(datos);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException("Error de validacion en el registro", errors);
        }

        try
        {
            var endpoint = "/api/medicos";
            System.Diagnostics.Debug.WriteLine($"[DoctorService] Registrando doctor: {datos.Nombre} {datos.Apellido}");
            var response = await _apiClient.PostAsync<DoctorResponseDto>(endpoint, datos);

            if (response == null || response.Id == 0)
                throw new AppException("No se recibio respuesta valida del servidor");

            _cachedDoctorId = response.Id;
            System.Diagnostics.Debug.WriteLine($"[DoctorService] Doctor registrado exitosamente. ID: {response.Id}");
            return response;
        }
        catch (ConflictException ex)
        {
            throw new AppException($"Doctor ya existe: {ex.Message}", "DOCTOR_EXISTS", 409);
        }
        catch (Exceptions.ValidationException)
        {
            throw;
        }
        catch (ConnectionException ex) when (ex.Message.Contains("500"))
        {
            throw new ConnectionException("Error en el servidor al registrar doctor. Intenta mas tarde.", ex);
        }
        catch (ConnectionException ex) when (ex.Message.Contains("503"))
        {
            throw new ConnectionException("El servidor no esta disponible. Intenta en unos momentos.", ex);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al registrar doctor: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets current doctor data using cached doctor id
    /// </summary>
    public async Task<DoctorResponseDto> ObtenerDoctorActualAsync()
    {
        try
        {
            if (!_cachedDoctorId.HasValue)
                throw new AppException("Doctor ID no disponible. Por favor inicia sesion primero.", "NO_DOCTOR_ID", 400);

            var endpoint = $"/api/medicos/{_cachedDoctorId}";
            System.Diagnostics.Debug.WriteLine($"[DoctorService] Obteniendo datos del doctor {_cachedDoctorId}");
            var response = await _apiClient.GetAsync<DoctorResponseDto>(endpoint);

            if (response == null)
                throw new AppException("Doctor no encontrado");

            return response;
        }
        catch (AppException ex) when (ex.Code == "NOT_FOUND")
        {
            throw new AppException(
                "Tu perfil de doctor no fue encontrado. Verifica que estes registrado.",
                "DOCTOR_NOT_FOUND",
                404
            );
        }
        catch (ConnectionException ex) when (ex.Message.Contains("500"))
        {
            throw new ConnectionException("Error en el servidor al obtener datos del doctor. Intenta mas tarde.", ex);
        }
        catch (ConnectionException ex) when (ex.Message.Contains("503"))
        {
            throw new ConnectionException("El servidor no esta disponible. Intenta en unos momentos.", ex);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al obtener datos del doctor: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Updates doctor data
    /// </summary>
    public async Task<DoctorResponseDto> ActualizarDoctorAsync(int doctorId, DoctorRequestDto datos)
    {
        if (doctorId <= 0)
            throw new AppException("Doctor ID invalido", "INVALID_DOCTOR_ID", 400);

        var validationResult = await _registrarValidator.ValidateAsync(datos);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException("Error de validacion en la actualizacion", errors);
        }

        try
        {
            var endpoint = $"/api/medicos/{doctorId}";
            System.Diagnostics.Debug.WriteLine($"[DoctorService] Actualizando doctor {doctorId}");
            var response = await _apiClient.PutAsync<DoctorResponseDto>(endpoint, datos);

            if (response == null)
                throw new AppException("No se recibio respuesta valida del servidor");

            System.Diagnostics.Debug.WriteLine($"[DoctorService] Doctor {doctorId} actualizado exitosamente");
            return response;
        }
        catch (AppException ex) when (ex.Code == "NOT_FOUND")
        {
            throw new AppException($"El doctor con ID {doctorId} no fue encontrado.", "DOCTOR_NOT_FOUND", 404);
        }
        catch (Exceptions.ValidationException)
        {
            throw;
        }
        catch (ConnectionException ex) when (ex.Message.Contains("500"))
        {
            throw new ConnectionException("Error en el servidor al actualizar doctor. Intenta mas tarde.", ex);
        }
        catch (ConnectionException ex) when (ex.Message.Contains("503"))
        {
            throw new ConnectionException("El servidor no esta disponible. Intenta en unos momentos.", ex);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al actualizar doctor: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Sets cached doctor id
    /// </summary>
    public void EstablecerDoctorId(int doctorId)
    {
        _cachedDoctorId = doctorId;
    }

    /// <summary>
    /// Gets cached doctor id
    /// </summary>
    public int? ObtenerDoctorIdCacheado()
    {
        return _cachedDoctorId;
    }
}
