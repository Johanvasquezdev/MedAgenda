using SGC.Application.DTOs.Audit;
using SGC.Domain.Base;

namespace SGC.Application.Mappers
{
    public static class AuditoriaMapper // Mapper para convertir entidades de auditoria a respuestas DTO. Sellada para evitar herencia.
    {
        public static AuditoriaResponse ToResponse(AuditEntity auditEntity) // Convierte una entidad de auditoria a un DTO de respuesta para ser enviado al cliente o utilizado en la capa de aplicacion.
        {
            return new AuditoriaResponse
            {
                Id = auditEntity.Id,
                UsuarioId = auditEntity.UsuarioId,
                Entidad = auditEntity.Entidad,
                Accion = auditEntity.Accion,
                ValorAnterior = auditEntity.ValorAnterior,
                ValorNuevo = auditEntity.ValorNuevo,
                Fecha = auditEntity.Fecha,
                DireccionIP = auditEntity.DireccionIP
            };
        }
    }
}