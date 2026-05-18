namespace DoctorApp.DTOs.Responses;

/// <summary>
/// DTO para paciente (alineado con la API)
/// </summary>
public class PacienteResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cedula { get; set; }
    public string? Telefono { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public int? Edad { get; set; }
    public string? TipoSeguro { get; set; }
    public string? NumeroSeguro { get; set; }
    public bool Activo { get; set; }
}
