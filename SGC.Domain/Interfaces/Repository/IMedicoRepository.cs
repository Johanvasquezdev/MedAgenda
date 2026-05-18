using SGC.Domain.Entities.Medical;

namespace SGC.Domain.Interfaces.Repository
{
    // Interfaz para operaciones de persistencia de medicos
    public interface IMedicoRepository : IBaseRepository<Medico>
    {
        // Busca un medico por su numero de exequatur
        Task<Medico> GetByExequaturAsync(string exequatur);

        // Obtiene medicos filtrados por especialidad
        Task<IEnumerable<Medico>> GetByEspecialidadAsync(int especialidadId);

        // Obtiene un medico con sus horarios cargados para validar disponibilidad
        Task<Medico> GetByIdWithHorariosAsync(int id);
    }
}   