using SGC.Application.DTOs.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Contracts
{
    // Interfaz para la gestion de pagos relacionados con las citas medicas, utilizando Stripe como plataforma de pago.
    public interface IPagoService
    {
        Task<string> CrearIntentoPagoAsync(CrearPagoRequest request);
        // Crea un intento de pago en Stripe y devuelve el ID del PaymentIntent para que el cliente pueda completar el proceso de pago.

        Task<bool> SimularConfirmacionAsync(int citaId);

        Task<bool> ConfirmarPagoAsync(string stripePaymentIntentId);
        // Confirma el pago en Stripe utilizando el ID del PaymentIntent. Esto se llama desde el webhook de Stripe cuando se recibe la confirmacion de pago exitoso.

        Task<bool> MarcarPagoFallidoAsync(string stripePaymentIntentId);
        // Marca como fallido un pago pendiente identificado por el PaymentIntent de Stripe.

        Task<bool> ReembolsarPagoAsync(int pagoId);
        // Inicia un reembolso para un pago existente, devolviendo el dinero al paciente. Esto se puede usar en casos de cancelacion de citas o devoluciones.

        Task<PagoResponse?> GetByCitaAsync(int citaId);
        // Obtiene los detalles del pago asociado a una cita medica especifica, incluyendo el estado del pago y la informacion de la transaccion.

        Task<IEnumerable<PagoResponse>> GetByPacienteAsync(int pacienteId);
        // Obtiene una lista de pagos realizados por un paciente, permitiendo al paciente revisar su historial de pagos y transacciones.
    }
}
