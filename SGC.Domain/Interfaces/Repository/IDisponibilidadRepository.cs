using SGC.Domain.Entities.Appointments;
using SGC.Domain.Enums;

namespace SGC.Domain.Interfaces.Repository
{
    // Interfaz especifica para la entidad Disponibilidad, con metodos para consultar horarios por medico o dia de la semana.
    public interface IDisponibilidadRepository : IBaseRepository<Disponibilidad>
    {
        Task<IEnumerable<Disponibilidad>> GetByMedicoIdAsync(int medicoId);
        Task<IEnumerable<Disponibilidad>> GetByDiaAsync(DiaSemana diaSemana);
    }
}
