using SGC.Application.Contracts;
using SGC.Application.DTOs.Notifications;
using SGC.Application.Mappers;
using SGC.Application.Services.Base;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio de notificaciones para manejar la logica de negocio relacionada con las notificaciones del sistema
    public class NotificacionService : BaseService, INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;

        public NotificacionService(
            INotificacionRepository notificacionRepository,
            ISGCLogger logger) : base(logger)
        {
            _notificacionRepository = notificacionRepository;
        }

        // Crea una nueva notificacion para un usuario
        public async Task<NotificacionResponse> GetByIdAsync(int id)
        {
            return await ExecuteOperacionAsync(
                "GetNotificacionById",
                async () =>
                {
                    var notificacion = await _notificacionRepository.GetByIdAsync(id);
                    return NotificacionMapper.ToResponse(notificacion);
                },
                $"NotificacionId: {id}");
        }

        // Obtiene todas las notificaciones de un usuario
        public async Task<IEnumerable<NotificacionResponse>> GetByUsuarioAsync(
            int usuarioId)
        {
            return await ExecuteOperacionAsync(
                "GetNotificacionesByUsuario",
                async () =>
                {
                    var notificaciones = await _notificacionRepository
                        .GetByUsuarioIdAsync(usuarioId);
                    return notificaciones.Select(NotificacionMapper.ToResponse);
                },
                $"UsuarioId: {usuarioId}");
        }

        // Obtiene solo las notificaciones no leidas de un usuario
        public async Task<IEnumerable<NotificacionResponse>> GetNoLeidasAsync(
            int usuarioId)
        {
            return await ExecuteOperacionAsync(
                "GetNotificacionesNoLeidas",
                async () =>
                {
                    var notificaciones = await _notificacionRepository
                        .GetNoLeidasAsync(usuarioId);
                    return notificaciones.Select(NotificacionMapper.ToResponse);
                },
                $"UsuarioId: {usuarioId}");
        }

        // Marca una notificacion como leida
        public async Task MarcarLeidaAsync(int notificacionId)
        {
            await ExecuteOperacionAsync(
                "MarcarLeida",
                async () =>
                {
                    var notificacion = await _notificacionRepository
                        .GetByIdAsync(notificacionId);
                    notificacion.MarcarLeida();
                    await _notificacionRepository.UpdateAsync(notificacion);
                },
                $"NotificacionId: {notificacionId}");
        }
    }
}
