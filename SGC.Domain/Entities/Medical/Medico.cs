using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Catalog;
using SGC.Domain.Entities.Security;
using SGC.Domain.Enums;
using SGC.Domain.Exceptions;

namespace SGC.Domain.Entities.Medical
{
    // El medico hereda de Usuario (TPT) con atributos especificos de su rol profesional
    public sealed class Medico : Usuario
    {
        public string? Exequatur { get; set; }
        public int? EspecialidadId { get; set; }
        public int? ProveedorSaludId { get; set; }
        public string? TelefonoConsultorio { get; set; }
        public string? Foto { get; set; }
        public bool MedicoActivo { get; set; } = true;

        // Propiedades de navegacion
        public Especialidad? Especialidad { get; set; }
        public ProveedorSalud? ProveedorSalud { get; set; }
        public ICollection<Disponibilidad> Horarios { get; set; } = new List<Disponibilidad>();
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();

        // Convierte el DayOfWeek de .NET al enum DiaSemana para coincidir con los valores de la BD
        private static DiaSemana ConvertirDiaSemana(DayOfWeek dia) => dia switch
        {
            DayOfWeek.Monday => DiaSemana.Lunes,
            DayOfWeek.Tuesday => DiaSemana.Martes,
            DayOfWeek.Wednesday => DiaSemana.Miercoles,
            DayOfWeek.Thursday => DiaSemana.Jueves,
            DayOfWeek.Friday => DiaSemana.Viernes,
            DayOfWeek.Saturday => DiaSemana.Sabado,
            DayOfWeek.Sunday => DiaSemana.Domingo,
            _ => throw new ArgumentOutOfRangeException(nameof(dia))
        };

        // Verifica si el medico tiene disponibilidad para la fecha y hora dada
        public bool TieneDisponibilidad(DateTime fechaHora)
        {
            var diaSemana = ConvertirDiaSemana(fechaHora.DayOfWeek);
            return Horarios.Any(h =>
                h.DiaSemana == diaSemana &&
                h.HoraInicio <= fechaHora.TimeOfDay &&
                h.HoraFin >= fechaHora.TimeOfDay &&
                h.EsValido());
        }

        // Agrega un horario validando que no se solape con los existentes
        public void AgregarHorario(Disponibilidad nuevaDisponibilidad)
        {
            if (!nuevaDisponibilidad.EsValido())
                throw new HorarioNoDisponibleException(Id, DateTime.UtcNow);

            bool seSolapa = Horarios.Any(h =>
                h.DiaSemana == nuevaDisponibilidad.DiaSemana &&
                h.HoraInicio < nuevaDisponibilidad.HoraFin &&
                h.HoraFin > nuevaDisponibilidad.HoraInicio);

            if (seSolapa)
                throw new CitaConflictoException("El horario se solapa con uno existente.");

            Horarios.Add(nuevaDisponibilidad);
        }
    }
}
