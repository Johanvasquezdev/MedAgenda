namespace SGC.Application.DTOs.Security
{
    // Datos para la solicitud de inicio de sesion
    public class LoginRequest
    {
        // Correo electronico del usuario
        public string Email { get; set; } = string.Empty;

        // Contrasena del usuario
        public string Password { get; set; } = string.Empty;
    }
}
