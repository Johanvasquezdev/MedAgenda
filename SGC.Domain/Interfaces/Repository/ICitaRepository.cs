using SGC.Domain.Entities.Appointments;

namespace SGC.Domain.Interfaces.Repository
{
    public interface ICitaRepository : IBaseRepository<Cita> // Hereda de la interfaz generica IBaseRepository para aprovechar las operaciones CRUD estandar, y agrega metodos especificos para la entidad Cita.
    {
        Task<IEnumerable<Cita>> GetByPacienteIdAsync(int pacienteId);
        Task<IEnumerable<Cita>> GetByMedicoIdAsync(int medicoId);
        Task<IEnumerable<Cita>> GetByFechaAsync(DateTime fecha);
        Task<bool> ExisteConflictoAsync(int medicoId, DateTime fechaHora);
    }
}

