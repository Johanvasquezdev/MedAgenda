namespace SGC.Application.DTOs.Chat
{
    public class ChatRequest
    {
        // DTO para enviar un mensaje al modelo de lenguaje. Incluye el mensaje, el ID del usuario que lo envía y un contexto opcional para mejorar la respuesta.
        public string Mensaje { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
        public string? Contexto { get; set; }
    }
}