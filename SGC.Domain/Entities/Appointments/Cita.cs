using SGC.Domain.Base;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using SGC.Domain.Exceptions;

namespace SGC.Domain.Entities.Appointments
{
    // Representa una cita medica entre un paciente y un medico con maquina de estados
    public class Cita : EntidadBase
    {
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public int? DisponibilidadId { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoCita Estado { get; set; } = EstadoCita.Solicitada;
        public string? Motivo { get; set; }
        public string? Notas { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Propiedades de navegacion
        public Paciente Paciente { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public Disponibilidad? Disponibilidad { get; set; }

        // Transiciones de estado
        public void Confirmar()
        {
            if (Estado != EstadoCita.Solicitada)
                throw new CitaConflictoException(
                    "Solo se puede confirmar una cita en estado Solicitada.");
            Estado = EstadoCita.Confirmada;
        }

        public void Cancelar(string motivo)
        {
            if (Estado == EstadoCita.Completada)
                throw new CitaConflictoException(
                    "No se puede cancelar una cita completada.");
            Estado = EstadoCita.Cancelada;
            Motivo = motivo;
        }

        public void Rechazar(string motivo)
        {
            if (Estado != EstadoCita.Solicitada)
                throw new CitaConflictoException(
                    "Solo se puede rechazar una cita en estado Solicitada.");
            Estado = EstadoCita.Rechazada;
            Motivo = motivo;
        }

        public void Reprogramar(DateTime nuevaFecha)
        {
            if (Estado == EstadoCita.Completada || Estado == EstadoCita.Cancelada)
                throw new CitaConflictoException(
                    "No se puede reprogramar una cita completada o cancelada.");
            FechaHora = nuevaFecha;
            Estado = EstadoCita.Solicitada;
        }

        public void MarcarNoAsistio()
        {
            if (Estado != EstadoCita.Confirmada)
                throw new CitaConflictoException(
                    "Solo se puede marcar NoAsistio en una cita Confirmada.");
            Estado = EstadoCita.NoAsistio;
        }

        public void IniciarConsulta()
        {
            if (Estado != EstadoCita.Confirmada)
                throw new CitaConflictoException(
                    "Solo se puede iniciar una cita en estado Confirmada.");
            Estado = EstadoCita.EnProgreso;
        }

        public void Completar()
        {
            if (Estado != EstadoCita.EnProgreso)
                throw new CitaConflictoException(
                    "Solo se puede completar una cita En Progreso.");
            Estado = EstadoCita.Completada;
        }
    }
}
