
using SGC.Application.Contracts;
using SGC.Application.DTOs.Audit;
using SGC.Application.Mappers;
using SGC.Application.Services.Base;
using SGC.Domain.Base;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio de auditoria para registrar eventos importantes del sistema, como cambios en entidades, acciones de usuarios, etc. Implementa la interfaz IAuditoriaService y utiliza un repositorio de auditoria para almacenar los eventos.
    public class AuditoriaService : BaseService, IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;

        public AuditoriaService(
            IAuditoriaRepository auditoriaRepository,
            ISGCLogger logger) : base(logger) 
        {
            _auditoriaRepository = auditoriaRepository;
        }

        // Registra un evento de auditoria con los detalles proporcionados, incluyendo el usuario que realizó la acción, la entidad afectada, la acción realizada, los valores anteriores y nuevos, y la dirección IP. El evento se guarda en la base de datos a través del repositorio de auditoria.
        public async Task RegistrarAsync(int? usuarioId, string entidad,
            string accion, string? valorAnterior,
            string? valorNuevo, string? direccionIP)
        {
            await ExecuteOperacionAsync(
                "RegistrarAuditoria",
                async () =>
                {
                    var evento = new AuditEntity
                    {
                        UsuarioId = usuarioId,
                        Entidad = entidad,
                        Accion = accion,
                        ValorAnterior = valorAnterior,
                        ValorNuevo = valorNuevo,
                        DireccionIP = direccionIP,
                        Fecha = DateTime.UtcNow
                    };

                    await _auditoriaRepository.AddAsync(evento);
                },
                $"Entidad: {entidad}, Accion: {accion}");
        }

        // Obtiene los eventos de auditoria relacionados con una entidad específica, como "Usuario", "Cita", etc. El método consulta el repositorio de auditoria para recuperar los eventos que coinciden con la entidad dada y los convierte a objetos de respuesta utilizando el mapeador de auditoria.
        public async Task<IEnumerable<AuditoriaResponse>> GetByEntidadAsync(
            string entidad)
        {
            return await ExecuteOperacionAsync(
                "GetAuditoriaByEntidad",
                async () =>
                {
                    var eventos = await _auditoriaRepository
                        .GetByEntidadAsync(entidad);
                    return eventos.Select(AuditoriaMapper.ToResponse);
                },
                $"Entidad: {entidad}");
        }

        // Obtiene los eventos de auditoria relacionados con un usuario específico, identificados por su ID. El método consulta el repositorio de auditoria para recuperar los eventos que fueron realizados por el usuario dado y los convierte a objetos de respuesta utilizando el mapeador de auditoria.
        public async Task<IEnumerable<AuditoriaResponse>> GetByUsuarioAsync(
            int usuarioId)
        {
            return await ExecuteOperacionAsync(
                "GetAuditoriaByUsuario",
                async () =>
                {
                    var eventos = await _auditoriaRepository
                        .GetByUsuarioIdAsync(usuarioId);
                    return eventos.Select(AuditoriaMapper.ToResponse);
                },
                $"UsuarioId: {usuarioId}");
        }
    }
}
