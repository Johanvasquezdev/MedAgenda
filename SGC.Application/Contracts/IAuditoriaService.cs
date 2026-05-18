using SGC.Application.DTOs.Audit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Contracts
{
    // Interfaz para el servicio de auditoria, que registra y consulta las acciones realizadas en el sistema para fines de seguridad y seguimiento.
    public interface IAuditoriaService
    {
        // Registrar una accion en el sistema, incluyendo detalles como el usuario que realizo la accion, la entidad afectada, la accion realizada, los valores anteriores y nuevos, y la direccion IP desde donde se realizo la accion.
        Task RegistrarAsync(int? usuarioId, string entidad,
            string accion, string? valorAnterior,
            string? valorNuevo, string? direccionIP);

        // Metodos para consultar los registros de auditoria, filtrando por diferentes criterios como entidad o usuario.
        Task<IEnumerable<AuditoriaResponse>> GetByEntidadAsync(string entidad);

        // Obtener los registros de auditoria realizados por un usuario especifico, identificando por su ID.
        Task<IEnumerable<AuditoriaResponse>> GetByUsuarioAsync(int usuarioId);
    }
}