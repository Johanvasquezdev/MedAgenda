using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using DoctorApp.Services.ApiClient;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.Services.Implementation;

/// <summary>
/// Servicio de Pacientes - consume endpoints de /api/pacientes
/// </summary>
public class PacienteService : IPacienteService
{
    private readonly IApiClient _apiClient;

    public PacienteService(IApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public async Task<PacienteResponseDto?> ObtenerPorCedulaAsync(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
            throw new Exceptions.ValidationException("La cedula es requerida");

        try
        {
            var endpoint = $"/api/pacientes/cedula/{cedula}";
            return await _apiClient.GetAsync<PacienteResponseDto>(endpoint);
        }
        catch (AppException ex) when (ex.Code == "NOT_FOUND")
        {
            return null;
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al buscar paciente: {ex.Message}", ex);
        }
    }

    public async Task<PacienteResponseDto?> ObtenerPorIdAsync(int id)
    {
        if (id <= 0)
            throw new Exceptions.ValidationException("El ID del paciente es requerido");

        try
        {
            var endpoint = $"/api/pacientes/{id}";
            return await _apiClient.GetAsync<PacienteResponseDto>(endpoint);
        }
        catch (AppException ex) when (ex.Code == "NOT_FOUND")
        {
            return null;
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al buscar paciente por ID: {ex.Message}", ex);
        }
    }
}
