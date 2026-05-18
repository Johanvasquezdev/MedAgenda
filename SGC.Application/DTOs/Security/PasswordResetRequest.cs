namespace SGC.Application.DTOs.Security
{
    // DTO para solicitar el restablecimiento de contraseña
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    // DTO para restablecer la contraseña con el token recibido
    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
    }
}
