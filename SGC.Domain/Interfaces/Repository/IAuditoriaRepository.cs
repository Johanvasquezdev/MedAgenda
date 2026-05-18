using SGC.Domain.Base;

namespace SGC.Domain.Interfaces.Repository
{
    public interface IAuditoriaRepository : IBaseRepository<AuditEntity>
    {
        Task<IEnumerable<AuditEntity>> GetByEntidadAsync(string entidad);
        Task<IEnumerable<AuditEntity>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<AuditEntity>> GetByFechaAsync(DateTime fecha);
    }
}
