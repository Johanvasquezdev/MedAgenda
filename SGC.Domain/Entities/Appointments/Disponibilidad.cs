using SGC.Domain.Base;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;

namespace SGC.Domain.Entities.Appointments
{
    // Horarios disponibles de un medico para atender citas
    public class Disponibilidad : EntidadBase
    {
        public int MedicoId { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int DuracionCitaMin { get; set; }
        public bool EsRecurrente { get; set; } = true;

        // Navegacion inversa al medico
        public Medico Medico { get; set; } = null!;

        // Valida que el horario sea coherente
        public bool EsValido()
        {
            return HoraFin > HoraInicio && DuracionCitaMin > 0;
        }
    }
}
