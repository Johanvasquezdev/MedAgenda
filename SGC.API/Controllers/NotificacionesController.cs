using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SGC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    [Authorize]
    public class NotificacionesController : ControllerBase // Controlador para gestionar las notificaciones de los usuarios, con acciones para obtener notificaciones por usuario, marcar como leidas y gestionar preferencias de notificaciones. Se utiliza el servicio de notificaciones para la logica de negocio.
    {
        private readonly INotificacionService _notificacionService;
        private readonly IPrefNotificacionService _prefService;

        public NotificacionesController(
            INotificacionService notificacionService,
            IPrefNotificacionService prefService)
        {
            _notificacionService = notificacionService;
            _prefService = prefService;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(rawUserId, out userId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) // GET api/notificaciones/{id} - Obtiene una notificacion por su ID, accesible para el usuario propietario de la notificacion
        {
            var notificacion = await _notificacionService
                .GetByIdAsync(id);

            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador") && notificacion.UsuarioId != userId)
                return Forbid();

            return Ok(notificacion);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuario(int usuarioId) // GET api/notificaciones/usuario/{usuarioId} - Obtiene las notificaciones de un usuario, accesible para el usuario propietario de las notificaciones
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador") && userId != usuarioId)
                return Forbid();

            var notificaciones = await _notificacionService
                .GetByUsuarioAsync(usuarioId);
            return Ok(notificaciones);
        }

        [HttpGet("usuario/{usuarioId}/no-leidas")]
        public async Task<IActionResult> GetNoLeidas(int usuarioId) // GET api/notificaciones/usuario/{usuarioId}/no-leidas - Obtiene las notificaciones no leidas de un usuario, accesible para el usuario propietario de las notificaciones
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador") && userId != usuarioId)
                return Forbid();

            var notificaciones = await _notificacionService
                .GetNoLeidasAsync(usuarioId);
            return Ok(notificaciones);
        }

        [HttpPut("{id}/leer")]
        public async Task<IActionResult> MarcarLeida(int id) // PUT api/notificaciones/{id}/leer - Marca una notificacion como leida, accesible para el usuario propietario de la notificacion
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var notificacion = await _notificacionService.GetByIdAsync(id);
            if (!User.IsInRole("Administrador") && notificacion.UsuarioId != userId)
                return Forbid();

            await _notificacionService.MarcarLeidaAsync(id); // Marca la notificacion como leida
            return NoContent();
        }

        [HttpGet("preferencias/{usuarioId}")]
        public async Task<IActionResult> GetPreferencias(int usuarioId) // GET api/notificaciones/preferencias/{usuarioId} - Obtiene las preferencias de notificaciones de un usuario, accesible para el usuario propietario de las preferencias
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador") && userId != usuarioId)
                return Forbid();

            var preferencias = await _prefService
                .GetByUsuarioAsync(usuarioId);
            return Ok(preferencias);
        }

        [HttpPost("preferencias")]
        public async Task<IActionResult> GuardarPreferencias( // POST api/notificaciones/preferencias - Guarda las preferencias de notificaciones de un usuario, accesible para el usuario propietario de las preferencias
            [FromBody] Application.DTOs.Notifications.PrefNotificacionRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador"))
                request.UsuarioId = userId;

            var preferencias = await _prefService.GuardarAsync(request); // Guarda las preferencias de notificaciones y devuelve el resultado
            return Ok(preferencias);
        }
    }
}
