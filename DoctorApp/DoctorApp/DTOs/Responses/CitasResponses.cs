using System.Text.Json.Serialization;

namespace DoctorApp.DTOs.Responses;

/// <summary>
/// DTO para la respuesta de agenda
/// </summary>
public class ObtenerAgendaResponse
{
    public DateTime Fecha { get; set; }
    public List<CitaResponseDto> Citas { get; set; } = new();
    public int TotalCitas { get; set; }
    public int CitasConfirmadas { get; set; }
    public int CitasPendientes { get; set; }
}

/// <summary>
/// DTO para una cita en la respuesta (alineado con la API)
/// </summary>
public class CitaResponseDto
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public string PacienteNombre { get; set; } = string.Empty;
    public int MedicoId { get; set; }
    public string MedicoNombre { get; set; } = string.Empty;
    public int? DisponibilidadId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Motivo { get; set; }
    public string? Notas { get; set; }
    public DateTime FechaCreacion { get; set; }

    [JsonIgnore]
    public bool Confirmada => Estado.Equals("Confirmada", StringComparison.OrdinalIgnoreCase)
        || Estado.Equals("Completada", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public int DuracionMinutos { get; set; }
}

/// <summary>
/// DTO para disponibilidad (alineado con la API)
/// </summary>
public class DisponibilidadResponseDto
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public string MedicoNombre { get; set; } = string.Empty;
    public string DiaSemana { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public int DuracionCitaMin { get; set; }
    public bool EsRecurrente { get; set; }
}

/// <summary>
/// DTO para respuesta de token
/// </summary>
public class AuthTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
}

/// <summary>
/// DTO para respuesta genérica de error
/// </summary>
public class ApiErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
