using SGC.Application.Contracts;
using SGC.Application.DTOs.Chat;
using SGC.Application.Services.Base;
using SGC.Domain.Interfaces;
using SGC.Domain.Interfaces.ILogger;
using System;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio de aplicacion para manejar la logica de negocio relacionada con el chatbot del sistema.
    public class ChatbotAppService : BaseService, IChatbotAppService
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotAppService(
            IChatbotService chatbotService,
            ISGCLogger logger) : base(logger)
        {
            _chatbotService = chatbotService;
        }

        // Envia un mensaje al chatbot y obtiene una respuesta asincronamente, registrando la operacion en el log.
        public async Task<ChatResponse> EnviarMensajeAsync(
            ChatRequest request)
        {
            return await ExecuteOperacionAsync(
                "EnviarMensaje",
                async () =>
                {
                    var respuesta = await _chatbotService
                        .EnviarMensajeAsync(request.Mensaje, request.Contexto);

                    return new ChatResponse
                    {
                        Respuesta = respuesta,
                        FechaRespuesta = DateTime.UtcNow
                    };
                },
                $"UsuarioId: {request.UsuarioId}");
        }
    }
}
