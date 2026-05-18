using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Chat;
using System.Threading.Tasks;

namespace SGC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatbotController : ControllerBase // Controlador para gestionar las interacciones con el chatbot, permitiendo a los usuarios enviar mensajes y recibir respuestas generadas por el sistema de inteligencia artificial para mejorar la experiencia de usuario y proporcionar asistencia personalizada.
    {
        private readonly IChatbotAppService _chatbotService;

        public ChatbotController(IChatbotAppService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost("mensaje")]
        public async Task<IActionResult> EnviarMensaje( // Permite a los usuarios enviar un mensaje al chatbot y recibir una respuesta generada por el sistema de inteligencia artificial, mejorando la experiencia de usuario y proporcionando asistencia personalizada.
            [FromBody] ChatRequest request)
        {
            var respuesta = await _chatbotService
                .EnviarMensajeAsync(request);
            return Ok(respuesta);
        }
    }
}
