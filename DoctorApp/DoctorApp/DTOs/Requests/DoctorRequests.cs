namespace DoctorApp.DTOs.Requests;

/// <summary>
/// DTO para registrar un nuevo doctor
/// </summary>
public class DoctorRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}

/// <summary>
/// DTO para login de doctor (alternativa a registro)
/// </summary>
public class DoctorLoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}
