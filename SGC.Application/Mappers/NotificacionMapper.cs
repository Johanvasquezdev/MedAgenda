using SGC.Application.DTOs.Notifications;
using SGC.Domain.Entities.Notifications;

namespace SGC.Application.Mappers
{
    // Mapper para convertir entidades de Notificacion a DTOs de respuesta
    public static class NotificacionMapper
    {
        // Convierte una entidad Notificacion a un DTO NotificacionResponse para ser enviado al cliente
        public static NotificacionResponse ToResponse(
            Notificacion notificacion)
        {
            return new NotificacionResponse
            {
                Id = notificacion.Id,
                UsuarioId = notificacion.UsuarioId,
                CitaId = notificacion.CitaId,
                Tipo = notificacion.Tipo.ToString(),
                Mensaje = notificacion.Mensaje,
                Leida = notificacion.Leida,
                FechaEnvio = notificacion.FechaEnvio
            };
        }
    }
}