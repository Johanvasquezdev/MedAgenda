using SGC.Application.DTOs.Chat;
using System.Threading.Tasks;

namespace SGC.Application.Contracts
{
    public interface IChatbotAppService // Interfaz para el servicio de chatbot, que define las operaciones disponibles para interactuar con el modelo de lenguaje.
    {
        Task<ChatResponse> EnviarMensajeAsync(ChatRequest request);
    }
}