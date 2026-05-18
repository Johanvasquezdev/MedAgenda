using System;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Exceptions;

namespace SGC.Domain.Services
{
    // Servicio de dominio para validar reglas de negocio de las citas
    public class CitaDomainService
    {
        // Valida si se puede agendar una cita con el medico en la fecha solicitada
        public void ValidarAgendamiento(Cita cita, Medico medico)
        {
            if (!medico.MedicoActivo)
                throw new CitaConflictoException(
                    "El medico no esta activo en el sistema.");

            if (!medico.TieneDisponibilidad(cita.FechaHora))
                throw new HorarioNoDisponibleException(
                    medico.Id, cita.FechaHora);
        }

        // Valida si se puede reprogramar una cita a la nueva fecha solicitada
        public void ValidarReprogramacion(
            Cita cita,
            Medico medico,
            DateTime nuevaFecha)
        {
            if (!medico.MedicoActivo)
                throw new CitaConflictoException(
                    "El medico no esta activo en el sistema.");

            if (!medico.TieneDisponibilidad(nuevaFecha))
                throw new HorarioNoDisponibleException(
                    medico.Id, nuevaFecha);
        }
    }
}
