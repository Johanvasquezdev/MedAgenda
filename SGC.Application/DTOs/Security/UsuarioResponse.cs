namespace SGC.Application.DTOs.Security
{
    // Datos de respuesta con la informacion de un usuario
    public class UsuarioResponse
    {
        // Identificador unico del usuario
        public int Id { get; set; }

        // Nombre completo del usuario
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico
        public string Email { get; set; } = string.Empty;

        // Rol del usuario en el sistema
        public string Rol { get; set; } = string.Empty;

        // Fecha de creacion de la cuenta
        public DateTime FechaCreacion { get; set; }

        // Indica si el usuario esta activo
        public bool Activo { get; set; }
    }
}
