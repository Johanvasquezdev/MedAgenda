namespace DoctorApp.Models;

public class Medico
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string NumeroLicencia { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Consultorio { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public string NombreCompleto => $"Dr/a. {Nombre} {Apellido}";

    // Propiedades de navegación
    public List<DisponibilidadDiaria> DisponibilidadDiaria { get; set; } = new();
    public List<DisponibilidadEspecial> DisponibilidadEspecial { get; set; } = new();
    public List<Cita> Citas { get; set; } = new();
}
