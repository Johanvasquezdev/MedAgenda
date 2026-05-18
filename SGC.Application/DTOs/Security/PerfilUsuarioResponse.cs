namespace SGC.Application.DTOs.Security
{
    // Datos de perfil para la pagina de configuracion del usuario autenticado
    public class PerfilUsuarioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }
}
