using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Appointments
{
    // Repositorio para operaciones de persistencia de citas medicas
    public class CitaRepository : BaseRepository<Cita>, ICitaRepository
    {
        public CitaRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Obtiene todas las citas de un paciente con los datos del medico incluidos
        public async Task<IEnumerable<Cita>> GetByPacienteIdAsync(int pacienteId)
        {
            return await ExecuteReadAsync("GetByPacienteIdAsync", async () =>
                await Context.Citas
                    .Where(c => c.PacienteId == pacienteId)
                    .Include(c => c.Medico)
                    .ToListAsync());
        }

        // Obtiene todas las citas de un medico con los datos del paciente incluidos
        public async Task<IEnumerable<Cita>> GetByMedicoIdAsync(int medicoId)
        {
            return await ExecuteReadAsync("GetByMedicoIdAsync", async () =>
                await Context.Citas
                    .Where(c => c.MedicoId == medicoId)
                    .Include(c => c.Paciente)
                    .ToListAsync());
        }

        // Obtiene todas las citas de una fecha especifica con medico y paciente incluidos
        public async Task<IEnumerable<Cita>> GetByFechaAsync(DateTime fecha)
        {
            return await ExecuteReadAsync("GetByFechaAsync", async () =>
                await Context.Citas
                    .Where(c => c.FechaHora.Date == fecha.Date)
                    .Include(c => c.Medico)
                    .Include(c => c.Paciente)
                    .ToListAsync());
        }

        // Verifica si existe un conflicto de horario para un medico en una fecha y hora especifica
        public async Task<bool> ExisteConflictoAsync(int medicoId, DateTime fechaHora)
        {
            return await ExecuteReadAsync("ExisteConflictoAsync", async () =>
                await Context.Citas.AnyAsync(c =>
                    c.MedicoId == medicoId &&
                    c.FechaHora == fechaHora &&
                    c.Estado != EstadoCita.Cancelada &&
                    c.Estado != EstadoCita.Rechazada));
        }
    }
}
