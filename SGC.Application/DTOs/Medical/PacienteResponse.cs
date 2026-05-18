namespace SGC.Application.DTOs.Medical
{
    // Datos de respuesta con la informacion de un paciente
    public class PacienteResponse
    {
        // Identificador unico del paciente
        public int Id { get; set; }

        // Nombre completo del paciente
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico
        public string Email { get; set; } = string.Empty;

        // Numero de cedula de identidad
        public string? Cedula { get; set; }

        // Numero de telefono
        public string? Telefono { get; set; }

        // Fecha de nacimiento
        public DateOnly? FechaNacimiento { get; set; }

        // Edad calculada del paciente
        public int? Edad { get; set; }

        // Tipo de seguro medico
        public string? TipoSeguro { get; set; }

        // Numero de poliza de seguro
        public string? NumeroSeguro { get; set; }

        // Indica si el paciente esta activo en el sistema
        public bool Activo { get; set; }
    }
}
