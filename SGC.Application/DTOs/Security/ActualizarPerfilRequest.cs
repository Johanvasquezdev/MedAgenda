using System.ComponentModel.DataAnnotations;

namespace SGC.Application.DTOs.Security
{
    // Datos para actualizar el perfil del usuario autenticado
    public class ActualizarPerfilRequest
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 120 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato valido.")]
        [StringLength(254, ErrorMessage = "El email no puede exceder 254 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El telefono no tiene un formato valido.")]
        [StringLength(20, ErrorMessage = "El telefono no puede exceder 20 caracteres.")]
        public string? Telefono { get; set; }
    }
}
