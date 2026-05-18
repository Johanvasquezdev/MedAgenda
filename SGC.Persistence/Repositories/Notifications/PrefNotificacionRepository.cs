using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Notifications;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Notifications
{
    // Repositorio para operaciones de persistencia de preferencias de notificacion
    public class PrefNotificacionRepository : BaseRepository<PrefNotificacion>, IPrefNotificacionRepository
    {
        public PrefNotificacionRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Obtiene las preferencias de notificacion de un usuario (cada usuario tiene un unico registro)
        public async Task<PrefNotificacion> GetByUsuarioIdAsync(int usuarioId)
        {
            return await ExecuteReadAsync("GetByUsuarioIdAsync", async () =>
                await Context.PrefNotificaciones
                    .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId)
                ?? throw new KeyNotFoundException($"No se encontraron preferencias de notificación para el usuario {usuarioId}."));
        }
    }
}
