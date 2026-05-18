using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Medical
{
    // Repositorio para operaciones de persistencia de pacientes
    public class PacienteRepository : BaseRepository<Paciente>, IPacienteRepository
    {
        public PacienteRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Busca un paciente por su numero de cedula
        public async Task<Paciente> GetByCedulaAsync(string cedula)
        {
            return await ExecuteReadAsync("GetByCedulaAsync", async () =>
                await Context.Pacientes
                    .FirstOrDefaultAsync(p => p.Cedula == cedula)
                ?? throw new KeyNotFoundException($"No se encontró un paciente con cédula {cedula}."));
        }
    }
}
