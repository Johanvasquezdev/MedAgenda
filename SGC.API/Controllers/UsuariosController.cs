using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;

namespace SGC.API.Controllers
{
    // Controlador para la gestion de usuarios del sistema (solo administradores)
    [Route("api/usuarios")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET api/usuarios - Obtiene todos los usuarios
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? rol)
        {
            if (!string.IsNullOrEmpty(rol))
            {
                var porRol = await _usuarioService.GetByRolAsync(rol);
                return Ok(porRol);
            }

            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios);
        }

        // GET api/usuarios/{id} - Obtiene un usuario por su ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            return Ok(usuario);
        }

        // PUT api/usuarios/{id}/desactivar - Desactiva un usuario
        [HttpPut("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _usuarioService.DesactivarAsync(id);
            return NoContent();
        }

        // PUT api/usuarios/{id}/activar - Activa un usuario
        [HttpPut("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            await _usuarioService.ActivarAsync(id);
            return NoContent();
        }
    }
}
