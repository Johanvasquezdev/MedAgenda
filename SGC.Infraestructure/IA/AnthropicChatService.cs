using Microsoft.Extensions.Configuration;
using SGC.Domain.Interfaces;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SGC.Infraestructure.IA
{
    // Servicio de chatbot utilizando la API de Groq (OpenAI-compatible).
    public class AnthropicChatService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _baseUrl;
        private readonly int _maxTokens;
        private readonly decimal _temperature;

        public AnthropicChatService(HttpClient httpClient,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["AI:ApiKey"]
                ?? throw new InvalidOperationException("AI:ApiKey no configurado");
            _model = config["AI:Model"]
                ?? "llama-3.1-8b-instant";
            _baseUrl = config["AI:BaseUrl"]
                ?? "https://api.groq.com/openai/v1";
            _maxTokens = int.TryParse(config["AI:MaxTokens"], out var mt) ? mt : 1000;
            _temperature = decimal.TryParse(config["AI:Temperature"], out var t) ? t : 0.2m;
        }

        // Método privado para enviar la solicitud a la API de Groq y obtener la respuesta
        private async Task<string> EnviarRequestAsync(
            string mensaje, string sistema)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var request = new
            {
                model = _model,
                max_tokens = _maxTokens,
                temperature = _temperature,
                messages = new[]
                {
                    new { role = "system", content = sistema },
                    new { role = "user", content = mensaje }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_baseUrl.TrimEnd('/')}/chat/completions", request);

            var json = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<AnthropicResponse>(
                json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result?.Choices?.FirstOrDefault()?.Message?.Content
                ?? "No pude procesar tu consulta.";
        }

        // Método público para enviar un mensaje al chatbot con un contexto opcional y obtener la respuesta generada por la IA
        public async Task<string> EnviarMensajeAsync(
            string mensaje, string? contexto = null)
        {
            var sistema = contexto ??
                "Eres un asistente médico de MedAgenda. " +
                "Ayudas a pacientes a agendar citas, consultar " +
                "disponibilidad y resolver dudas sobre el sistema.";

            return await EnviarRequestAsync(mensaje, sistema);
        }

        // Métodos específicos para consultas comunes de los pacientes sobre citas médicas, que formatean el mensaje con el contexto adecuado antes de enviarlo al chatbot
        public async Task<string> ConsultarDisponibilidadAsync(
            int medicoId, DateTime fecha)
        {
            var mensaje = $"El paciente consulta disponibilidad " +
                $"para el médico {medicoId} " +
                $"en la fecha {fecha:dd/MM/yyyy}";
            return await EnviarMensajeAsync(mensaje);
        }

        // Consulta sobre el estado de una cita específica, formateando el mensaje con el ID de la cita para que el chatbot pueda proporcionar la información relevante
        public async Task<string> ObtenerInfoCitaAsync(int citaId)
        {
            var mensaje = $"El paciente consulta información " +
                $"sobre su cita número {citaId}";
            return await EnviarMensajeAsync(mensaje);
        }

        // Consulta general sobre cómo usar el sistema, formateando el mensaje con la pregunta del paciente para que el chatbot pueda proporcionar una respuesta útil y orientativa
        private class AnthropicResponse
        {
            public List<Choice>? Choices { get; set; }

            public class Choice
            {
                public MessageBlock? Message { get; set; }
            }

            public class MessageBlock
            {
                public string? Content { get; set; }
            }
        }
    }
}
