namespace SGC.Application.DTOs.Medical
{
    // Datos para actualizar la informacion de un paciente existente
    public class ActualizarPacienteRequest
    {
        // Identificador del paciente a actualizar
        public int Id { get; set; }

        // Nombre completo del paciente
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico del paciente
        public string Email { get; set; } = string.Empty;

        // Numero de cedula de identidad
        public string? Cedula { get; set; }

        // Numero de telefono
        public string? Telefono { get; set; }

        // Fecha de nacimiento
        public DateOnly? FechaNacimiento { get; set; }

        // Tipo de seguro medico
        public string? TipoSeguro { get; set; }

        // Numero de poliza de seguro
        public string? NumeroSeguro { get; set; }
    }
}
