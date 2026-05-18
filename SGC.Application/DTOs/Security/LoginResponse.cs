using System;

namespace SGC.Application.DTOs.Security
{
    // DTO de respuesta para el login exitoso, contiene el token y datos basicos del usuario
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
    }
}
