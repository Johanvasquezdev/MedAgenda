using SGC.Domain.Entities.Appointments;
using SGC.Domain.Exceptions;

namespace SGC.Domain.Validators
{
    public class CitaValidator //  la clase valida las reglas de negocio para la creación o actualización de una cita médica, asegurando que se cumplan las restricciones de horario, la relación entre paciente y médico, y la asignación de disponibilidad. Si alguna de las reglas no se cumple, se lanzan excepciones específicas para informar sobre el conflicto o la falta de disponibilidad.
    {
        public void Validar(Cita cita)
        {
            // Regla: no se puede agendar en el pasado
            if (cita.FechaHora < DateTime.UtcNow)
                throw new CitaConflictoException(
                    "No se puede agendar una cita en el pasado.");

            // Regla: horario permitido entre 6am y 10pm
            if (cita.FechaHora.Hour < 6 || cita.FechaHora.Hour > 22)
                throw new HorarioNoDisponibleException(
                    cita.MedicoId, cita.FechaHora);

            // Regla: paciente y médico no pueden ser la misma persona
            if (cita.PacienteId == cita.MedicoId)
                throw new CitaConflictoException(
                    "El paciente y el médico no pueden ser la misma persona.");

            // Regla: debe tener disponibilidad asignada
            if (cita.DisponibilidadId == null)
                throw new HorarioNoDisponibleException(
                    cita.MedicoId, cita.FechaHora);
        }
    }
}