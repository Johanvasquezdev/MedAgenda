using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Security;
using System.Security.Claims;

namespace SGC.API.Controllers
{
    [Route("api/perfil")]
    [ApiController]
    [Authorize]
    public class PerfilController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public PerfilController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(rawUserId, out userId);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMiPerfil()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var perfil = await _usuarioService.GetPerfilAsync(userId);
            return Ok(perfil);
        }

        [HttpPut("me")]
        public async Task<IActionResult> ActualizarMiPerfil(
            [FromBody] ActualizarPerfilRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var perfil = await _usuarioService.ActualizarPerfilAsync(userId, request);
            return Ok(perfil);
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> CambiarMiPassword(
            [FromBody] CambiarPasswordRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            await _usuarioService.CambiarPasswordAsync(userId, request);
            return NoContent();
        }
    }
}
