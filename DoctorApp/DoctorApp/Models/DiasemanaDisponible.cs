namespace DoctorApp.Models;

public enum DiaSemana
{
    Lunes,
    Martes,
    Miercoles,
    Jueves,
    Viernes,
    Sabado,
    Domingo
}

public class DisponibilidadDiaria
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DiaSemana Dia { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public int DuracionConsultaMinutos { get; set; } = 30;
    public bool Disponible { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaUltimaModificacion { get; set; }
}

public class DisponibilidadEspecial
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeOnly? HoraInicio { get; set; }
    public TimeOnly? HoraFin { get; set; }
    public bool Disponible { get; set; } = true;
    public string? Razon { get; set; } // Ej: "Congreso médico", "Vacaciones", etc.
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
