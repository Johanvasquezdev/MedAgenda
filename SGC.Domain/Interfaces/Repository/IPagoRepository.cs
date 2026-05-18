using SGC.Domain.Entities.Payments;
using SGC.Domain.Enums;              // ← para EstadoPago

namespace SGC.Domain.Interfaces.Repository
{
    public interface IPagoRepository : IBaseRepository<Pago>
    {
        Task<Pago?> GetByCitaAsync(int citaId);
        Task<Pago?> GetByStripeIdAsync(string stripePaymentIntentId);
        Task<IEnumerable<Pago>> GetByPacienteAsync(int pacienteId);
        Task<IEnumerable<Pago>> GetByEstadoAsync(EstadoPago estado);
    }
}