using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Payments;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Payments
{
    // Repositorio para operaciones de persistencia de pagos.
    public class PagoRepository : BaseRepository<Pago>, IPagoRepository
    {
        public PagoRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Obtiene un pago por el Id de la cita asociada.
        public async Task<Pago?> GetByCitaAsync(int citaId)
        {
            return await ExecuteReadAsync("GetByCitaAsync", async () =>
                await Context.Set<Pago>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CitaId == citaId));
        }

        // Obtiene un pago por su StripePaymentIntentId.
        public async Task<Pago?> GetByStripeIdAsync(string stripePaymentIntentId)
        {
            return await ExecuteReadAsync("GetByStripeIdAsync", async () =>
                await Context.Set<Pago>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.StripePaymentIntentId == stripePaymentIntentId));
        }

        // Obtiene todos los pagos de un paciente.
        public async Task<IEnumerable<Pago>> GetByPacienteAsync(int pacienteId)
        {
            return await ExecuteReadAsync("GetByPacienteAsync", async () =>
                await Context.Set<Pago>()
                    .AsNoTracking()
                    .Where(p => p.PacienteId == pacienteId)
                    .ToListAsync());
        }

        // Obtiene todos los pagos por estado.
        public async Task<IEnumerable<Pago>> GetByEstadoAsync(EstadoPago estado)
        {
            return await ExecuteReadAsync("GetByEstadoAsync", async () =>
                await Context.Set<Pago>()
                    .AsNoTracking()
                    .Where(p => p.Estado == estado)
                    .ToListAsync());
        }
    }
}
