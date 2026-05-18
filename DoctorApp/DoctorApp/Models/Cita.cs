namespace DoctorApp.Models;

public class Cita
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public string? EspecialidadId { get; set; }
    public DateTime FechaHora { get; set; }
    public int DuracionMinutos { get; set; } = 30;
    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
    public string Motivo { get; set; } = string.Empty;
    public string? Notas { get; set; }
    public bool Confirmada { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaUltimaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }

    // Propiedades de navegación (se cargarán desde la BD)
    public Paciente? Paciente { get; set; }
    public DateTime FechaFin => FechaHora.AddMinutes(DuracionMinutos);
    public bool Vencida => DateTime.Now > FechaHora && Estado != EstadoCita.Completada;
    public string ResumenCita => $"{FechaHora:dd/MM/yyyy HH:mm} - {Motivo}";
}
