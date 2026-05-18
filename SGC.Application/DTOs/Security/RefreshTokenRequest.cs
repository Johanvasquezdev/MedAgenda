namespace SGC.Application.DTOs.Security
{
    // Datos para solicitar la renovacion de un token JWT
    public class RefreshTokenRequest
    {
        // Token de refresco emitido en el login
        public string RefreshToken { get; set; } = string.Empty;
    }
}
