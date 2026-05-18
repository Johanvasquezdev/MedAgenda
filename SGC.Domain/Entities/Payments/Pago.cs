using SGC.Domain.Base;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using System;

namespace SGC.Domain.Entities.Payments
{
    // Representa un pago asociado a una cita médica, con información sobre el monto, estado
    public class Pago : EntidadAuditable
    {
        public int CitaId { get; set; }
        public int PacienteId { get; set; }
        public decimal Monto { get; set; } 
        public string Moneda { get; set; } = "DOP"; // Moneda por defecto
        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;
        public string? StripePaymentIntentId { get; set; }
        public DateTime? FechaPago { get; set; }

        // Navegación
        public Cita? Cita { get; set; }
        public Paciente? Paciente { get; set; }

        // Transiciones de estado
        public void Confirmar()
        {
            if (Estado != EstadoPago.Pendiente)
                throw new InvalidOperationException(
                    "Solo se puede confirmar un pago pendiente.");
            Estado = EstadoPago.Completado;
            FechaPago = DateTime.UtcNow;
        }

        // Permite reembolsar un pago solo si está completado, cambiando su estado a Reembolsado
        public void Reembolsar()
        {
            if (Estado != EstadoPago.Completado)
                throw new InvalidOperationException(
                    "Solo se puede reembolsar un pago completado.");
            Estado = EstadoPago.Reembolsado;
        }

        // Permite marcar un pago como fallido solo si está pendiente, cambiando su estado a Fallido
        public void MarcarFallido()
        {
            if (Estado != EstadoPago.Pendiente)
                throw new InvalidOperationException(
                    "Solo se puede marcar fallido un pago pendiente.");
            Estado = EstadoPago.Fallido;
        }
    }
}