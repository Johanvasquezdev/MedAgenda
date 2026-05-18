using SGC.Domain.Entities.Appointments;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio para la creacion o actualizacion de una disponibilidad horaria
    public class DisponibilidadValidator
    {
        public void Validar(Disponibilidad disponibilidad)
        {
            // Regla: debe tener un medico asignado
            if (disponibilidad.MedicoId <= 0)
                throw new InvalidOperationException(
                    "La disponibilidad debe estar asociada a un medico.");

            // Regla: la hora de fin debe ser mayor que la de inicio
            if (disponibilidad.HoraFin <= disponibilidad.HoraInicio)
                throw new InvalidOperationException(
                    "La hora de fin debe ser mayor que la hora de inicio.");

            // Regla: la duracion de la cita debe ser mayor a 0
            if (disponibilidad.DuracionCitaMin <= 0)
                throw new InvalidOperationException(
                    "La duracion de la cita debe ser mayor a 0 minutos.");

            // Regla: el horario debe estar dentro del rango permitido (6am - 10pm)
            if (disponibilidad.HoraInicio < TimeSpan.FromHours(6) ||
                disponibilidad.HoraFin > TimeSpan.FromHours(22))
                throw new InvalidOperationException(
                    "El horario debe estar entre las 6:00 AM y las 10:00 PM.");
        }
    }
}
