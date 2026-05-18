using System;

namespace SGC.Application.DTOs.Chat
{
    public class ChatResponse // DTO para representar la respuesta del chat, con el mensaje y la fecha de respuesta.
    {
        public string Respuesta { get; set; } = string.Empty;
        public DateTime FechaRespuesta { get; set; } = DateTime.UtcNow;
    }
}