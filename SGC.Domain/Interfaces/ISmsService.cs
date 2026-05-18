using System.Threading.Tasks;

namespace SGC.Domain.Interfaces
{
    public interface ISmsService // Interfaz para servicios de SMS, Twilio
    {
        Task EnviarRecordatorioAsync(string telefono, string mensaje);
        Task EnviarAlertaUrgente(string telefono, string mensaje);
        Task EnviarVerificacionAsync(string telefono, string codigo);
    }
}