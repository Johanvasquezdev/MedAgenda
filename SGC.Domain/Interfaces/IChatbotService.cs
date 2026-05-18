using System;
using System.Threading.Tasks;

namespace SGC.Domain.Interfaces
{
    public interface IChatbotService // Interfaz para el servicio de chatbot que interactúa con los pacientes
    {
        Task<string> EnviarMensajeAsync(string mensaje, string? contexto = null);
        Task<string> ConsultarDisponibilidadAsync(int medicoId, DateTime fecha); 
        Task<string> ObtenerInfoCitaAsync(int citaId);
    }
}