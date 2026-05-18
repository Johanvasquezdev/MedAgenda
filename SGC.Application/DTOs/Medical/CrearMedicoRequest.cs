namespace SGC.Application.DTOs.Medical
{
    // Datos necesarios para registrar un nuevo medico en el sistema
    public class CrearMedicoRequest
    {
        // Nombre completo del medico
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico del medico
        public string Email { get; set; } = string.Empty;

        // Contrasena del medico
        public string Password { get; set; } = string.Empty;

        // Numero de exequatur (licencia medica)
        public string? Exequatur { get; set; }

        // Identificador de la especialidad medica
        public int? EspecialidadId { get; set; }

        // Identificador del proveedor de salud donde trabaja
        public int? ProveedorSaludId { get; set; }

        // Telefono del consultorio
        public string? TelefonoConsultorio { get; set; }

        // URL o ruta de la foto del medico
        public string? Foto { get; set; }
    }
}
