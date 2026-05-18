using System;

namespace SGC.Application.DTOs.Payments
{
    // DTO para representar la respuesta de un pago realizado, con detalles relevantes para el cliente o sistema consumidor.
    public class PagoResponse
    {
        public int Id { get; set; }
        public int CitaId { get; set; }
        public int PacienteId { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? StripePaymentIntentId { get; set; }
        public DateTime? FechaPago { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}