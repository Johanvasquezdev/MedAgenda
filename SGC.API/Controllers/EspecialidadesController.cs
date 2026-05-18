using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Catalog;

namespace SGC.API.Controllers
{
    // Controlador para la gestion de especialidades medicas
    [Route("api/especialidades")]
    [ApiController]
    public class EspecialidadesController : ControllerBase
    {
        private readonly IEspecialidadService _especialidadService;

        public EspecialidadesController(IEspecialidadService especialidadService)
        {
            _especialidadService = especialidadService;
        }

        // GET api/especialidades - Obtiene todas las especialidades activas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var especialidades = await _especialidadService.GetActivasAsync();
            return Ok(especialidades);
        }

        // POST api/especialidades - Crea una nueva especialidad (solo admin)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([FromBody] EspecialidadRequest request)
        {
            var especialidad = await _especialidadService.CrearAsync(request);
            return CreatedAtAction(nameof(GetAll), null, especialidad);
        }

        // PUT api/especialidades/{id} - Actualiza una especialidad existente
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EspecialidadRequest request)
        {
            await _especialidadService.ActualizarAsync(id, request);
            return NoContent();
        }
    }
}
