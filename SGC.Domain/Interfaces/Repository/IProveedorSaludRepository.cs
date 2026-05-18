using SGC.Domain.Entities.Catalog;

namespace SGC.Domain.Interfaces.Repository
{
    public interface IProveedorSaludRepository : IBaseRepository<ProveedorSalud> // Interfaz especifica para la entidad ProveedorSalud, que hereda de la interfaz generica IBaseRepository.
    {
        Task<IEnumerable<ProveedorSalud>> GetActivosAsync(); // Obtener solo los proveedores de salud activos en el sistema.
    }
}
