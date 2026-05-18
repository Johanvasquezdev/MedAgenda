using Microsoft.Extensions.Configuration;
using SGC.Domain.Interfaces;
using Stripe;

namespace SGC.Infraestructure.Pagos
{
    // Servicio de pago utilizando Stripe para procesar pagos de citas médicas
    public class StripePaymentService : IPaymentService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly RefundService _refundService;

        public StripePaymentService(IConfiguration config)
        {
            var stripeSecretKey = config["Stripe:SecretKey"];
            if (string.IsNullOrWhiteSpace(stripeSecretKey))
                throw new InvalidOperationException("Stripe:SecretKey no configurado");

            StripeConfiguration.ApiKey = stripeSecretKey;
            _paymentIntentService = new PaymentIntentService();
            _refundService = new RefundService();
        }

        // Método para crear un intento de pago en Stripe y devolver el PaymentIntentId para persistencia local
        public async Task<string> CrearIntentoPagoAsync(
            decimal monto, string moneda, int citaId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(monto * 100),
                Currency = moneda.ToLower(),
                Metadata = new Dictionary<string, string>
                {
                    { "citaId", citaId.ToString() }
                }
            };
            var intent = await _paymentIntentService.CreateAsync(options);
            return intent.Id;
        }

        // Método para confirmar el estado del pago después de que el cliente complete el proceso de pago en el frontend
        public async Task<bool> ConfirmarPagoAsync(
            string paymentIntentId)
        {
            var intent = await _paymentIntentService.GetAsync(paymentIntentId);
            return intent.Status == "succeeded";
        }

        // metodo para reembolsar un pago en caso de cancelación de cita o devolución de dinero al paciente
        public async Task<bool> ReembolsarPagoAsync(
            string paymentIntentId)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };
            var refund = await _refundService.CreateAsync(options);
            return refund.Status == "succeeded";
        }
    }
}
