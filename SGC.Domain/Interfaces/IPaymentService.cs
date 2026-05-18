using System.Threading.Tasks;

namespace SGC.Domain.Interfaces
{
    public interface IPaymentService // Interfaz para el servicio de pagos
    {
        Task<string> CrearIntentoPagoAsync(decimal monto, string moneda, int citaId); 
        Task<bool> ConfirmarPagoAsync(string paymentIntentId);
        Task<bool> ReembolsarPagoAsync(string paymentIntentId);
    }
}