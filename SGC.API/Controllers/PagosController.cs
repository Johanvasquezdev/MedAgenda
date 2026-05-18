using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Infraestructure.Pagos;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Payments;
using Stripe;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PagosController : ControllerBase // Controlador para gestionar los pagos de las citas, con acciones para crear intentos de pago, confirmar pagos, recibir webhooks de Stripe, reembolsar pagos y obtener pagos por cita o paciente. Se utiliza el servicio de pagos para la logica de negocio.
    {
        private readonly IPagoService _pagoService;
        private readonly StripeWebhookService _stripeWebhookService;

        public PagosController(
            IPagoService pagoService,
            StripeWebhookService stripeWebhookService)
        {
            _pagoService = pagoService;
            _stripeWebhookService = stripeWebhookService;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(rawUserId, out userId);
        }

        [HttpPost("crear-intento")]
        public async Task<IActionResult> CrearIntento( // POST api/pagos/crear-intento - Crea un intento de pago para una cita, recibiendo los detalles del pago en el cuerpo de la solicitud
            [FromBody] CrearPagoRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador"))
                request.PacienteId = userId;
            else if (request.PacienteId <= 0)
                return BadRequest("PacienteId es requerido para administradores.");

            var paymentIntentId = await _pagoService
                .CrearIntentoPagoAsync(request);
            try
            {
                var intentService = new PaymentIntentService();
                var intent = await intentService.GetAsync(paymentIntentId);
                if (string.IsNullOrWhiteSpace(intent.ClientSecret))
                    return StatusCode(502, "No se pudo obtener client secret de Stripe.");

                return Ok(new { clientSecret = intent.ClientSecret });
            }
            catch
            {
                return StatusCode(502, "No se pudo consultar Stripe para completar el intento de pago.");
            }
        }

        [HttpPost("confirmar")]
        public async Task<IActionResult> Confirmar( // POST api/pagos/confirmar - Confirma un pago, recibiendo el ID del intento de pago de Stripe en el cuerpo de la solicitud
            [FromBody] string stripePaymentIntentId)
        {
            var confirmado = await _pagoService
                .ConfirmarPagoAsync(stripePaymentIntentId);

            if (!confirmado)
                return BadRequest("No se pudo confirmar el pago.");

            return Ok(new { confirmado });
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook() // POST api/pagos/webhook - Recibe los webhooks de Stripe para eventos relacionados con los pagos, como confirmaciones o reembolsos. Se valida la firma del webhook y se procesa el evento correspondiente.
        {
            var payload = await new StreamReader(
                Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            if (string.IsNullOrEmpty(stripeSignature))
                return BadRequest("Firma de Stripe no encontrada.");

            Event stripeEvent;
            try
            {
                stripeEvent = _stripeWebhookService.ValidarWebhook(payload, stripeSignature!);
            }
            catch
            {
                return BadRequest("Webhook de Stripe inválido.");
            }

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null)
                return Ok();

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                await _pagoService.ConfirmarPagoAsync(paymentIntent.Id);
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                await _pagoService.MarcarPagoFallidoAsync(paymentIntent.Id);
            }
            else if (stripeEvent.Type == "charge.refunded")
            {
                // el flujo administrativo ya maneja reembolsos explícitos
            }

            return Ok();
        }

        [HttpPost("reembolsar/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Reembolsar(int id) // POST api/pagos/reembolsar/{id} - Reembolsa un pago por su ID, solo accesible para usuarios con rol de Administrador
        {
            var reembolsado = await _pagoService.ReembolsarPagoAsync(id);

            if (!reembolsado)
                return BadRequest("No se pudo procesar el reembolso.");

            return Ok(new { reembolsado });
        }

        [HttpGet("cita/{citaId}")]
        public async Task<IActionResult> GetByCita(int citaId) // GET api/pagos/cita/{citaId} - Obtiene el pago asociado a una cita por su ID, accesible para el paciente o medico relacionado con la cita
        {
            var pago = await _pagoService.GetByCitaAsync(citaId);

            if (pago == null)
                return NotFound(
                    $"No se encontró pago para la cita {citaId}.");

            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador") && pago.PacienteId != userId)
                return Forbid();

            return Ok(pago);
        }

        [HttpGet("paciente/{pacienteId}")] 
        public async Task<IActionResult> GetByPaciente(int pacienteId) // GET api/pagos/paciente/{pacienteId} - Obtiene los pagos realizados por un paciente, solo accesible para el paciente autenticado
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!User.IsInRole("Administrador") && userId != pacienteId)
                return Forbid();

            var pagos = await _pagoService.GetByPacienteAsync(pacienteId);
            return Ok(pagos);
        }
    }
}
