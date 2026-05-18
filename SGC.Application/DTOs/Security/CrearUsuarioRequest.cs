namespace SGC.Application.DTOs.Security
{
    // Datos necesarios para registrar un nuevo usuario en el sistema
    public class CrearUsuarioRequest
    {
        // Nombre completo del usuario
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico del usuario
        public string Email { get; set; } = string.Empty;

        // Contrasena del usuario
        public string Password { get; set; } = string.Empty;

        // Rol del usuario (Administrador, Medico, Paciente)
        public string Rol { get; set; } = string.Empty;
    }
}
