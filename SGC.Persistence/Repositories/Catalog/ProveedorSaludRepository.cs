using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Catalog;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Catalog
{
    // Repositorio para operaciones de persistencia de proveedores de salud
    public class ProveedorSaludRepository : BaseRepository<ProveedorSalud>, IProveedorSaludRepository
    {
        public ProveedorSaludRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Obtiene solo los proveedores de salud que estan activos en el sistema
        public async Task<IEnumerable<ProveedorSalud>> GetActivosAsync()
        {
            return await ExecuteReadAsync("GetActivosAsync", async () =>
                await Context.ProveedoresSalud
                    .Where(p => p.Activo)
                    .ToListAsync());
        }
    }
}
