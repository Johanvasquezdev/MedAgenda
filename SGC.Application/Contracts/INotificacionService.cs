using SGC.Application.DTOs.Notifications;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de gestion de notificaciones
    public interface INotificacionService
    {
        // Obtiene una notificacion por su identificador
        Task<NotificacionResponse> GetByIdAsync(int id);

        // Obtiene todas las notificaciones de un usuario
        Task<IEnumerable<NotificacionResponse>> GetByUsuarioAsync(int usuarioId);

        // Obtiene solo las notificaciones no leidas de un usuario
        Task<IEnumerable<NotificacionResponse>> GetNoLeidasAsync(int usuarioId);

        // Marca una notificacion como leida
        Task MarcarLeidaAsync(int notificacionId);
    }
}
