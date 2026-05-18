using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoctorApp.DTOs.Requests;
using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.Services.Mock;

/// <summary>
/// Mock services (only for offline testing)
/// </summary>
public class MockCitasService : ICitasService
{
    public Task<ObtenerAgendaResponse> ObtenerAgendaAsync(DateTime fecha)
    {
        return Task.FromResult(new ObtenerAgendaResponse
        {
            Fecha = fecha,
            Citas = new List<CitaResponseDto>(),
            TotalCitas = 0,
            CitasConfirmadas = 0,
            CitasPendientes = 0
        });
    }

    public Task<CitaResponseDto> ConfirmarCitaAsync(int citaId, bool confirmada, string? notas = null)
    {
        return Task.FromResult(new CitaResponseDto { Id = citaId, Estado = confirmada ? "Confirmada" : "Cancelada" });
    }

    public Task<CitaResponseDto> IniciarConsultaAsync(int citaId)
    {
        return Task.FromResult(new CitaResponseDto { Id = citaId, Estado = "EnConsulta" });
    }

    public Task<CitaResponseDto> MarcarAsistenciaAsync(int citaId, bool asistio, string? observaciones = null)
    {
        return Task.FromResult(new CitaResponseDto { Id = citaId, Estado = asistio ? "Completada" : "NoAsistio" });
    }

    public Task<List<CitaResponseDto>> ObtenerCitasDelDiaAsync()
    {
        return Task.FromResult(new List<CitaResponseDto>());
    }

    public Task<List<CitaResponseDto>> ObtenerCitasMedicoAsync()
    {
        return Task.FromResult(new List<CitaResponseDto>());
    }

    public Task<CitaResponseDto?> ObtenerCitaPorIdAsync(int citaId)
    {
        return Task.FromResult<CitaResponseDto?>(new CitaResponseDto { Id = citaId });
    }
// 👇 REEMPLAZA EL MÉTODO ANTERIOR POR ESTE 👇
    public Task<CitaResponseDto> CancelarCitaAsync(int citaId, string? motivo)
    {
        // Ahora sí devolvemos el objeto que exige la interfaz
        return Task.FromResult(new CitaResponseDto 
        { 
            Id = citaId, 
            Estado = "Cancelada" 
        });
    }
    }