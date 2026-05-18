namespace SGC.Application.DTOs.Medical
{
    // Datos para actualizar la informacion de un medico existente
    public class ActualizarMedicoRequest
    {
        // Identificador del medico a actualizar
        public int Id { get; set; }

        // Nombre completo del medico
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico del medico
        public string Email { get; set; } = string.Empty;

        // Numero de exequatur (licencia medica)
        public string? Exequatur { get; set; }

        // Identificador de la especialidad medica
        public int? EspecialidadId { get; set; }

        // Identificador del proveedor de salud
        public int? ProveedorSaludId { get; set; }

        // Telefono del consultorio
        public string? TelefonoConsultorio { get; set; }

        // URL o ruta de la foto del medico
        public string? Foto { get; set; }
    }
}
