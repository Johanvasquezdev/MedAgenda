using SGC.Domain.Entities.Notifications;

namespace SGC.Domain.Interfaces.Repository
{
    public interface IPrefNotificacionRepository : IBaseRepository<PrefNotificacion> // Interfaz especifica para la entidad PrefNotificacion, que hereda de la interfaz generica IBaseRepository.
    {
        Task<PrefNotificacion> GetByUsuarioIdAsync(int usuarioId); // Obtener las preferencias de notificacion de un usuario especifico. Cada usuario tiene un unico registro de preferencias.
    }
}
