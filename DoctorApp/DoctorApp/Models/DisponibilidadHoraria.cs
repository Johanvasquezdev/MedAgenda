namespace DoctorApp.Models;

/// <summary>
/// Modelo de Disponibilidad Horaria con soporte para Idempotencia
/// </summary>
public class DisponibilidadHoraria
{
    /// <summary>
    /// ID idempotente: basado en Día + Hora (nunca cambia)
    /// </summary>
    public int Id { get; set; }

    public DiaSemana Dia { get; set; }
    public string Hora { get; set; } = string.Empty;
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public bool EstaDisponible { get; set; }
    public int DuracionMinutos { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public DateTime? FechaActualizacion { get; set; }
    public bool Activo { get; set; } = true;
    public string? Notas { get; set; }

    // Propiedades Calculadas

    /// <summary>
    /// Formato amigable para mostrar el estado
    /// </summary>
    public string EstadoTexto => EstaDisponible ? "✅ Disponible" : "❌ No Disponible";

    /// <summary>
    /// Validación: Verificar si el horario es válido
    /// </summary>
    public bool EsValido() => HoraInicio < HoraFin && DuracionMinutos > 0 && Activo;

    /// <summary>
    /// Verificar si el horario ha sido modificado recientemente
    /// </summary>
    public bool FueModificado => FechaActualizacion.HasValue && 
                                 (DateTime.Now - FechaActualizacion.Value).TotalMinutes < 5;
}

