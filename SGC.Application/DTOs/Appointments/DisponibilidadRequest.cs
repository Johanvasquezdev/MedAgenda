namespace SGC.Application.DTOs.Appointments
{
    // Datos para crear o modificar un horario de disponibilidad de un medico
    public class DisponibilidadRequest
    {
        // Identificador del medico al que pertenece el horario
        public int MedicoId { get; set; }

        // Dia de la semana (0=Lunes, 1=Martes, ..., 6=Domingo)
        public int DiaSemana { get; set; }

        // Hora de inicio del horario disponible
        public TimeSpan HoraInicio { get; set; }

        // Hora de fin del horario disponible
        public TimeSpan HoraFin { get; set; }

        // Duracion de cada cita en minutos
        public int DuracionCitaMin { get; set; }

        // Indica si el horario se repite cada semana
        public bool EsRecurrente { get; set; } = true;
    }
}
