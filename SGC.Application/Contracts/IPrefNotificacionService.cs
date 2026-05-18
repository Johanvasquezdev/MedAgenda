using SGC.Application.DTOs.Notifications;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de preferencias de notificacion
    public interface IPrefNotificacionService
    {
        // Obtiene las preferencias de notificacion de un usuario
        Task<PrefNotificacionResponse> GetByUsuarioAsync(int usuarioId);

        // Crea o actualiza las preferencias de notificacion de un usuario
        Task<PrefNotificacionResponse> GuardarAsync(PrefNotificacionRequest request);
    }
}
