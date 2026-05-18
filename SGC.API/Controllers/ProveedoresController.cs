using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Catalog;

namespace SGC.API.Controllers
{
    // Controlador para la gestion de proveedores de salud
    [Route("api/proveedores")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorSaludService _proveedorService;

        public ProveedoresController(IProveedorSaludService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        // GET api/proveedores - Obtiene todos los proveedores activos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proveedores = await _proveedorService.GetActivosAsync();
            return Ok(proveedores);
        }

        // POST api/proveedores - Crea un nuevo proveedor de salud (solo admin)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([FromBody] ProveedorSaludRequest request)
        {
            var proveedor = await _proveedorService.CrearAsync(request);
            return CreatedAtAction(nameof(GetAll), null, proveedor);
        }

        // PUT api/proveedores/{id} - Actualiza un proveedor existente
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProveedorSaludRequest request)
        {
            await _proveedorService.ActualizarAsync(id, request);
            return NoContent();
        }
    }
}
