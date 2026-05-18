using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using System.Threading.Tasks;

namespace SGC.API.Controllers
{
    // Controlador para gestionar las auditorias del sistema, permitiendo a los administradores consultar las acciones realizadas por los usuarios en el sistema para fines de seguridad y seguimiento.
    [Route("api/auditoria")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class AuditoriasController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriasController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll( // Permite obtener los registros de auditoria filtrados por entidad o usuario, proporcionando una vista detallada de las acciones realizadas en el sistema para fines de seguridad y seguimiento.
            [FromQuery] string? entidad,
            [FromQuery] int? usuarioId)
        {
            if (!string.IsNullOrEmpty(entidad)) // Si se especifica una entidad, se obtienen los registros de auditoria relacionados con esa entidad.
            {
                var porEntidad = await _auditoriaService
                    .GetByEntidadAsync(entidad);
                return Ok(porEntidad);
            }

            if (usuarioId.HasValue) // Si se especifica un usuarioId, se obtienen los registros de auditoria relacionados con ese usuario.
            {
                var porUsuario = await _auditoriaService
                    .GetByUsuarioAsync(usuarioId.Value);
                return Ok(porUsuario);
            }

            return BadRequest("Debe especificar entidad o usuarioId.");
        }
    }
}
