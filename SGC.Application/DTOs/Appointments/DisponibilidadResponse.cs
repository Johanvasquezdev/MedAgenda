namespace SGC.Application.DTOs.Appointments
{
    // Datos de respuesta de un horario de disponibilidad
    public class DisponibilidadResponse
    {
        // Identificador unico de la disponibilidad
        public int Id { get; set; }

        // Identificador del medico
        public int MedicoId { get; set; }

        // Nombre del medico
        public string MedicoNombre { get; set; } = string.Empty;

        // Dia de la semana en texto (Lunes, Martes, etc.)
        public string DiaSemana { get; set; } = string.Empty;

        // Hora de inicio del horario
        public TimeSpan HoraInicio { get; set; }

        // Hora de fin del horario
        public TimeSpan HoraFin { get; set; }

        // Duracion de cada cita en minutos
        public int DuracionCitaMin { get; set; }

        // Indica si el horario se repite semanalmente
        public bool EsRecurrente { get; set; }
    }
}
