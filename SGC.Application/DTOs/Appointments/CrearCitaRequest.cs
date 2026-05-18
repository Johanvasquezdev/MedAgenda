namespace SGC.Application.DTOs.Appointments
{
    // Datos necesarios para agendar una nueva cita medica
    public class CrearCitaRequest
    {
        // Identificador del paciente que solicita la cita
        public int PacienteId { get; set; }

        // Identificador del medico con quien se agenda la cita
        public int MedicoId { get; set; }

        // Identificador de la disponibilidad horaria seleccionada
        public int? DisponibilidadId { get; set; }

        // Fecha y hora de la cita
        public DateTime FechaHora { get; set; }

        // Motivo o razon de la consulta
        public string? Motivo { get; set; }
    }
}
