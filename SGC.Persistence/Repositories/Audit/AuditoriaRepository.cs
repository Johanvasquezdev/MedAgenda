using Microsoft.EntityFrameworkCore;
using SGC.Domain.Base;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Audit
{
    // Repositorio para operaciones de persistencia de eventos de auditoria
    public class AuditoriaRepository : BaseRepository<AuditEntity>, IAuditoriaRepository
    {
        public AuditoriaRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Obtiene todos los eventos de auditoria de una entidad especifica (ej: "Cita", "Medico")
        public async Task<IEnumerable<AuditEntity>> GetByEntidadAsync(string entidad)
        {
            return await ExecuteReadAsync("GetByEntidadAsync", async () =>
                await Context.EventosAuditoria
                    .Where(a => a.Entidad == entidad)
                    .OrderByDescending(a => a.Fecha)
                    .ToListAsync());
        }

        // Obtiene todos los eventos de auditoria realizados por un usuario
        public async Task<IEnumerable<AuditEntity>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await ExecuteReadAsync("GetByUsuarioIdAsync", async () =>
                await Context.EventosAuditoria
                    .Where(a => a.UsuarioId == usuarioId)
                    .OrderByDescending(a => a.Fecha)
                    .ToListAsync());
        }

        // Obtiene todos los eventos de auditoria de una fecha especifica
        public async Task<IEnumerable<AuditEntity>> GetByFechaAsync(DateTime fecha)
        {
            return await ExecuteReadAsync("GetByFechaAsync", async () =>
                await Context.EventosAuditoria
                    .Where(a => a.Fecha.Date == fecha.Date)
                    .OrderByDescending(a => a.Fecha)
                    .ToListAsync());
        }
    }
}
