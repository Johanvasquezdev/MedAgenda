namespace SGC.Application.DTOs.Medical
{
    // Datos de respuesta con la informacion de un medico
    public class MedicoResponse
    {
        // Identificador unico del medico
        public int Id { get; set; }

        // Nombre completo del medico
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico
        public string Email { get; set; } = string.Empty;

        // Numero de exequatur (licencia medica)
        public string? Exequatur { get; set; }

        // Identificador de la especialidad
        public int? EspecialidadId { get; set; }

        // Nombre de la especialidad
        public string? NombreEspecialidad { get; set; }

        // Identificador del proveedor de salud
        public int? ProveedorSaludId { get; set; }

        // Nombre de la especialidad medica
        public string? Especialidad { get; set; }

        // Nombre del proveedor de salud donde trabaja
        public string? ProveedorSalud { get; set; }

        // Telefono del consultorio
        public string? TelefonoConsultorio { get; set; }

        // URL o ruta de la foto
        public string? Foto { get; set; }

        // Indica si el medico esta activo como usuario
        public bool Activo { get; set; }

        // Indica si el medico esta habilitado para ejercer
        public bool MedicoActivo { get; set; }
    }
}
