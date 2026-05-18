using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Appointments
{
    // Repositorio para operaciones de persistencia de disponibilidad de medicos
    public class DisponibilidadRepository : BaseRepository<Disponibilidad>, IDisponibilidadRepository
    {
        public DisponibilidadRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Obtiene todos los horarios disponibles de un medico
        public async Task<IEnumerable<Disponibilidad>> GetByMedicoIdAsync(int medicoId)
        {
            return await ExecuteReadAsync("GetByMedicoIdAsync", async () =>
                await Context.Disponibilidades
                    .Where(d => d.MedicoId == medicoId)
                    .ToListAsync());
        }

        // Obtiene todas las disponibilidades de un dia de la semana con los datos del medico
        public async Task<IEnumerable<Disponibilidad>> GetByDiaAsync(DiaSemana diaSemana)
        {
            return await ExecuteReadAsync("GetByDiaAsync", async () =>
                await Context.Disponibilidades
                    .Where(d => d.DiaSemana == diaSemana)
                    .Include(d => d.Medico)
                    .ToListAsync());
        }
    }
}
