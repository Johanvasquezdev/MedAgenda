namespace DoctorApp.DTOs.Requests;

/// <summary>
/// DTO para obtener la agenda del médico
/// </summary>
public class ObtenerAgendaRequest
{
    public DateTime Fecha { get; set; }
    public int? MedicoId { get; set; }
}

/// <summary>
/// DTO para confirmar una cita
/// </summary>
public class ConfirmarCitaRequest
{
    public int CitaId { get; set; }
    public bool Confirmada { get; set; }
    public string? Notas { get; set; }
}

/// <summary>
/// DTO para iniciar una consulta
/// </summary>
public class IniciarConsultaRequest
{
    public int CitaId { get; set; }
}

/// <summary>
/// DTO para marcar asistencia
/// </summary>
public class MarcarAsistenciaRequest
{
    public int CitaId { get; set; }
    public bool Asistio { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para crear/actualizar disponibilidad
/// </summary>
public class CrearDisponibilidadRequest
{
    public int MedicoId { get; set; }
    public int DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public int DuracionCitaMin { get; set; }
    public bool EsRecurrente { get; set; } = true;
}

/// <summary>
/// DTO para actualizar disponibilidad
/// </summary>
public class ActualizarDisponibilidadRequest
{
    [System.Text.Json.Serialization.JsonIgnore]
    public int DisponibilidadId { get; set; }
    public int MedicoId { get; set; }
    public int DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public int DuracionCitaMin { get; set; }
    public bool EsRecurrente { get; set; } = true;
}
