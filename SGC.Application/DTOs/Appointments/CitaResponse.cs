namespace SGC.Application.DTOs.Appointments
{
    // Datos de respuesta de una cita medica
    public class CitaResponse
    {
        // Identificador unico de la cita
        public int Id { get; set; }

        // Identificador del paciente
        public int PacienteId { get; set; }

        // Nombre del paciente
        public string PacienteNombre { get; set; } = string.Empty;

        // Identificador del medico
        public int MedicoId { get; set; }

        // Nombre del medico
        public string MedicoNombre { get; set; } = string.Empty;

        // Identificador de la disponibilidad asociada (si aplica)
        public int? DisponibilidadId { get; set; }

        // Fecha y hora de la cita
        public DateTime FechaHora { get; set; }

        // Estado actual de la cita (Solicitada, Confirmada, etc.)
        public string Estado { get; set; } = string.Empty;

        // Motivo de la consulta
        public string? Motivo { get; set; }

        // Notas adicionales
        public string? Notas { get; set; }

        // Fecha en que se creo la cita
        public DateTime FechaCreacion { get; set; }
    }
}
