using SGC.Domain.Entities.Catalog;

namespace SGC.Domain.Interfaces.Repository
{
    public interface IEspecialidadRepository : IBaseRepository<Especialidad> // Interfaz especifica para la entidad Especialidad, que hereda de la interfaz generica IBaseRepository.
    {
        Task<IEnumerable<Especialidad>> GetActivasAsync(); // Obtener solo las especialidades activas en el sistema.
    }
}
