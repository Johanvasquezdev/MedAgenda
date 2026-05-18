using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Notifications;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Notifications
{
    // Repositorio para operaciones de persistencia de notificaciones
    public class NotificacionRepository : BaseRepository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Obtiene todas las notificaciones de un usuario ordenadas por fecha de envio
        public async Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await ExecuteReadAsync("GetByUsuarioIdAsync", async () =>
                await Context.Notificaciones
                    .Where(n => n.UsuarioId == usuarioId)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToListAsync());
        }

        // Obtiene solo las notificaciones no leidas de un usuario
        public async Task<IEnumerable<Notificacion>> GetNoLeidasAsync(int usuarioId)
        {
            return await ExecuteReadAsync("GetNoLeidasAsync", async () =>
                await Context.Notificaciones
                    .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToListAsync());
        }
    }
}
