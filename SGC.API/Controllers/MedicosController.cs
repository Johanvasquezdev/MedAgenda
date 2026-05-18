using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Medical;
using System.Security.Claims;

namespace SGC.API.Controllers
{
    [Route("api/medicos")]
    [ApiController]
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoService _medicoService; // Controlador para gestionar los medicos, con acciones para listar, crear, actualizar y activar/desactivar medicos segun su rol. Se utiliza el servicio de medicos para la logica de negocio.

        public MedicosController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(rawUserId, out userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll( // GET api/medicos?especialidadId={id} - Obtiene la lista de medicos, con opcion de filtrar por especialidad
            [FromQuery] int? especialidadId)
        {
            if (especialidadId.HasValue)
            {
                var porEsp = await _medicoService
                    .GetByEspecialidadAsync(especialidadId.Value);
                return Ok(porEsp);
            }
            var medicos = await _medicoService.GetAllAsync();
            return Ok(medicos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) // GET api/medicos/{id} - Obtiene un medico por su ID, accesible para todos los usuarios autenticados
        {
            var medico = await _medicoService.GetByIdAsync(id);
            return Ok(medico);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")] // POST api/medicos - Crea un nuevo medico, solo accesible para usuarios con rol de Administrador
        public async Task<IActionResult> Crear(
            [FromBody] CrearMedicoRequest request)
        {
            var medico = await _medicoService.CrearAsync(request);
            return CreatedAtAction(nameof(GetById),
                new { id = medico.Id }, medico);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Medico")] // PUT api/medicos/{id} - Actualiza un medico, accesible para Administradores y el propio Medico (si es su perfil)
        public async Task<IActionResult> Actualizar(int id,
            [FromBody] ActualizarMedicoRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador") && userId != id)
                return Forbid();

            request.Id = id;
            await _medicoService.ActualizarAsync(request);
            return NoContent();
        }

        [HttpPut("{id}/desactivar")] // PUT api/medicos/{id}/desactivar - Desactiva un medico, solo accesible para usuarios con rol de Administrador
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _medicoService.DesactivarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/activar")] // PUT api/medicos/{id}/activar - Activa un medico, solo accesible para usuarios con rol de Administrador
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Activar(int id)
        {
            await _medicoService.ActivarAsync(id);
            return NoContent();
        }
    }
}
