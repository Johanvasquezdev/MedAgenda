namespace SGC.Application.DTOs.Medical
{
    // Datos necesarios para registrar un nuevo paciente en el sistema
    public class CrearPacienteRequest
    {
        // Nombre completo del paciente
        public string Nombre { get; set; } = string.Empty;

        // Correo electronico del paciente
        public string Email { get; set; } = string.Empty;

        // Contrasena del paciente
        public string Password { get; set; } = string.Empty;

        // Numero de cedula de identidad
        public string? Cedula { get; set; }

        // Numero de telefono
        public string? Telefono { get; set; }

        // Fecha de nacimiento del paciente
        public DateOnly? FechaNacimiento { get; set; }

        // Tipo de seguro medico
        public string? TipoSeguro { get; set; }

        // Numero de poliza de seguro
        public string? NumeroSeguro { get; set; }
    }
}
