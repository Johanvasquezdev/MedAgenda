using SGC.Application.Contracts;
using SGC.Application.DTOs.Notifications;
using SGC.Application.Services.Base;
using SGC.Domain.Entities.Notifications;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio para gestionar las preferencias de notificacion de los usuarios
    public class PrefNotificacionService : BaseService, IPrefNotificacionService
    {
        private readonly IPrefNotificacionRepository _prefRepository;
        private readonly PrefNotificacionValidator _validator;

        public PrefNotificacionService(
            IPrefNotificacionRepository prefRepository,
            PrefNotificacionValidator validator,
            ISGCLogger logger) : base(logger)
        {
            _prefRepository = prefRepository;
            _validator = validator;
        }

        // Obtiene las preferencias de notificacion de un usuario por su ID
        public async Task<PrefNotificacionResponse> GetByUsuarioAsync(
            int usuarioId)
        {
            return await ExecuteOperacionAsync(
                "GetPrefNotificacionByUsuario",
                async () =>
                {
                    var pref = await _prefRepository.GetByUsuarioIdAsync(usuarioId);
                    return MapToResponse(pref);
                },
                $"UsuarioId: {usuarioId}");
        }

        // Guarda o actualiza las preferencias de notificacion de un usuario
        public async Task<PrefNotificacionResponse> GuardarAsync(
            PrefNotificacionRequest request)
        {
            return await ExecuteOperacionAsync(
                "GuardarPreferencias",
                async () =>
                {
                    PrefNotificacion pref;
                    try
                    {
                        pref = await _prefRepository
                            .GetByUsuarioIdAsync(request.UsuarioId);
                        pref.RecibirEmail = request.RecibirEmail;
                        pref.RecibirSMS = request.RecibirSMS;
                        pref.RecibirPush = request.RecibirPush;
                        pref.HorasAntesRecordatorio = request.HorasAntesRecordatorio;
                        _validator.Validar(pref);
                        await _prefRepository.UpdateAsync(pref);
                    }
                    catch (KeyNotFoundException)
                    {
                        pref = new PrefNotificacion
                        {
                            UsuarioId = request.UsuarioId,
                            RecibirEmail = request.RecibirEmail,
                            RecibirSMS = request.RecibirSMS,
                            RecibirPush = request.RecibirPush,
                            HorasAntesRecordatorio = request.HorasAntesRecordatorio
                        };
                        _validator.Validar(pref);
                        await _prefRepository.AddAsync(pref);
                    }

                    return MapToResponse(pref);
                },
                $"UsuarioId: {request.UsuarioId}");
        }

        // Mapea la entidad PrefNotificacion a su DTO de respuesta
        private static PrefNotificacionResponse MapToResponse(
            PrefNotificacion p)
        {
            return new PrefNotificacionResponse
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId,
                RecibirEmail = p.RecibirEmail,
                RecibirSMS = p.RecibirSMS,
                RecibirPush = p.RecibirPush,
                HorasAntesRecordatorio = p.HorasAntesRecordatorio
            };
        }
    }
}
