using FluentValidation;
using DoctorApp.DTOs.Requests;
using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using DoctorApp.Services.ApiClient;
using DoctorApp.Services.Interfaces;
using DoctorApp.Validators;

namespace DoctorApp.Services.Implementation;

/// <summary>
/// Servicio de Disponibilidad - Consume endpoints de /api/disponibilidad
/// </summary>
public class DisponibilidadService : IDisponibilidadService
{
    private readonly IApiClient _apiClient;
    private readonly CrearDisponibilidadValidator _crearValidator;
    private readonly ActualizarDisponibilidadValidator _actualizarValidator;

    public DisponibilidadService(
        IApiClient apiClient,
        CrearDisponibilidadValidator crearValidator,
        ActualizarDisponibilidadValidator actualizarValidator)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _crearValidator = crearValidator;
        _actualizarValidator = actualizarValidator;
    }

    public async Task<List<DisponibilidadResponseDto>> ObtenerDisponibilidadesAsync(int medicoId)
    {
        try
        {
            var endpoint = $"/api/disponibilidad/medico/{medicoId}";
            var result = await _apiClient.GetAsync<List<DisponibilidadResponseDto>>(endpoint);
            return result ?? new List<DisponibilidadResponseDto>();
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al obtener disponibilidades: {ex.Message}", ex);
        }
    }

    public async Task<DisponibilidadResponseDto> CrearDisponibilidadAsync(CrearDisponibilidadRequest request)
    {
        var validationResult = await _crearValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException("Error de validación", errors);
        }

        try
        {
            const string endpoint = "/api/disponibilidad";
            var result = await _apiClient.PostAsync<DisponibilidadResponseDto>(endpoint, request);

            return result ?? throw new AppException("No se recibió respuesta del servidor");
        }
        catch (ConflictException ex)
        {
            throw new ConflictException(
                $"Este horario ya existe. {ex.Message}", ex);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al crear disponibilidad: {ex.Message}", ex);
        }
    }

    public async Task<DisponibilidadResponseDto> ActualizarDisponibilidadAsync(ActualizarDisponibilidadRequest request)
    {
        var validationResult = await _actualizarValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException("Error de validación", errors);
        }

        try
        {
            var endpoint = $"/api/disponibilidad/{request.DisponibilidadId}";
            var result = await _apiClient.PutAsync<DisponibilidadResponseDto>(endpoint, request);

            return result ?? throw new AppException("No se recibió respuesta del servidor");
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al actualizar disponibilidad: {ex.Message}", ex);
        }
    }

    public async Task EliminarDisponibilidadAsync(int disponibilidadId)
    {
        try
        {
            var endpoint = $"/api/disponibilidad/{disponibilidadId}";
            await _apiClient.DeleteAsync(endpoint);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al eliminar disponibilidad: {ex.Message}", ex);
        }
    }

    public async Task<DisponibilidadResponseDto?> ObtenerDisponibilidadPorIdAsync(int disponibilidadId)
    {
        try
        {
            var endpoint = $"/api/disponibilidad/{disponibilidadId}";
            return await _apiClient.GetAsync<DisponibilidadResponseDto>(endpoint);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al obtener disponibilidad: {ex.Message}", ex);
        }
    }
}
