using SGC.Application.DTOs.Payments;
using SGC.Domain.Entities.Payments;

namespace SGC.Application.Mappers
{
    // Mapper para convertir entre la entidad Pago y el DTO PagoResponse. Facilita la transformación de datos para la capa de presentación.
    public static class PagoMapper
    {
        // Convierte una entidad Pago a un DTO PagoResponse. Esto es útil para enviar solo los datos necesarios a la capa de presentación, ocultando detalles internos de la entidad.
        public static PagoResponse ToResponse(Pago pago)
        {
            return new PagoResponse
            {
                Id = pago.Id,
                CitaId = pago.CitaId,
                PacienteId = pago.PacienteId,
                Monto = pago.Monto,
                Moneda = pago.Moneda,
                Estado = pago.Estado.ToString(),
                StripePaymentIntentId = pago.StripePaymentIntentId,
                FechaPago = pago.FechaPago,
                FechaCreacion = pago.FechaCreacion
            };
        }
    }
}