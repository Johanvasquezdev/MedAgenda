using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Appointments;

namespace SGC.API.Controllers
{
    // Controlador para la gestion de disponibilidad de horarios de medicos
    [Route("api/disponibilidad")]
    [ApiController]
    [Authorize]
    public class DisponibilidadController : ControllerBase
    {
        private readonly IDisponibilidadService _disponibilidadService;

        public DisponibilidadController(IDisponibilidadService disponibilidadService)
        {
            _disponibilidadService = disponibilidadService;
        }

        // GET api/disponibilidad/medico/{id}?fecha={fecha} - Obtiene la disponibilidad de un medico
        [HttpGet("medico/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByMedico(int id, [FromQuery] DateTime? fecha)
        {
            var disponibilidades = await _disponibilidadService.GetByMedicoAsync(id);

            // Si se proporciona fecha, filtrar por dia de la semana
            if (fecha.HasValue)
            {
                // Convertir DayOfWeek a nuestro indice (0=Lunes)
                var diaSemana = ((int)fecha.Value.DayOfWeek + 6) % 7;
                var nombreDia = ((SGC.Domain.Enums.DiaSemana)diaSemana).ToString();
                disponibilidades = disponibilidades.Where(d => d.DiaSemana == nombreDia);
            }

            return Ok(disponibilidades);
        }

        // POST api/disponibilidad - Crea un nuevo horario de disponibilidad (solo medico)
        [HttpPost]
        [Authorize(Roles = "Administrador,Medico")] // Permite crear disponibilidad desde Admin o Medico.
        public async Task<IActionResult> Crear([FromBody] DisponibilidadRequest request)
        {
            var disponibilidad = await _disponibilidadService.CrearAsync(request);
            return CreatedAtAction(nameof(GetByMedico), new { id = request.MedicoId }, disponibilidad);
        }

        // PUT api/disponibilidad/{id} - Actualiza un horario de disponibilidad
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Medico")] // Permite actualizar disponibilidad desde Admin o Medico.
        public async Task<IActionResult> Actualizar(int id, [FromBody] DisponibilidadRequest request)
        {
            await _disponibilidadService.ActualizarAsync(id, request);
            return NoContent();
        }

        // DELETE api/disponibilidad/{id} - Elimina un horario de disponibilidad
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Medico")] // Permite eliminar disponibilidad desde Admin o Medico.
        public async Task<IActionResult> Eliminar(int id)
        {
            await _disponibilidadService.EliminarAsync(id);
            return NoContent();
        }
    }
}
