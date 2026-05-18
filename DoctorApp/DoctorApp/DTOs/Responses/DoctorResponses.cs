namespace DoctorApp.DTOs.Responses;

/// <summary>
/// DTO para la respuesta al registrar un doctor
/// </summary>
public class DoctorResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Exequatur { get; set; }
    public int? EspecialidadId { get; set; }
    public string? NombreEspecialidad { get; set; }
    public int? ProveedorSaludId { get; set; }
    public string? Especialidad { get; set; }
    public string? ProveedorSalud { get; set; }
    public string? TelefonoConsultorio { get; set; }
    public string? Foto { get; set; }
    public bool Activo { get; set; }
    public bool MedicoActivo { get; set; }
}

/// <summary>
/// DTO para la respuesta de login de doctor
/// </summary>
public class DoctorLoginResponseDto
{
    public int DoctorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
}
