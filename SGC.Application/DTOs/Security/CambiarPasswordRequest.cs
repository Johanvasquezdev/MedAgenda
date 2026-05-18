using System.ComponentModel.DataAnnotations;

namespace SGC.Application.DTOs.Security
{
    // Datos requeridos para cambiar la contrasena del usuario autenticado
    public class CambiarPasswordRequest
    {
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        public string PasswordActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "La nueva contraseña debe tener entre 8 y 128 caracteres.")]
        public string PasswordNueva { get; set; } = string.Empty;
    }
}
