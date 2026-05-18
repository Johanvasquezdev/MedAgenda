using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Medical
{
    // Repositorio para operaciones de persistencia de medicos
    public class MedicoRepository : BaseRepository<Medico>, IMedicoRepository
    {
        public MedicoRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Busca un medico por su numero de exequatur incluyendo su especialidad
        public async Task<Medico> GetByExequaturAsync(string exequatur)
        {
            return await ExecuteReadAsync("GetByExequaturAsync", async () =>
                await Context.Medicos
                    .Include(m => m.Especialidad)
                    .FirstOrDefaultAsync(m => m.Exequatur == exequatur)
                ?? throw new KeyNotFoundException(
                    $"No se encontró médico con exequatur {exequatur}."));
        }

        // Obtiene todos los medicos de una especialidad con sus datos incluidos
        public async Task<IEnumerable<Medico>> GetByEspecialidadAsync(int especialidadId)
        {
            return await ExecuteReadAsync("GetByEspecialidadAsync", async () =>
                await Context.Medicos
                    .Where(m => m.EspecialidadId == especialidadId)
                    .Include(m => m.Especialidad)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync());
        }

        // Obtiene un medico con sus horarios cargados para validar disponibilidad
        public async Task<Medico> GetByIdWithHorariosAsync(int id)
        {
            return await ExecuteReadAsync("GetByIdWithHorariosAsync", async () =>
                await Context.Medicos
                    .Include(m => m.Horarios)
                    .Include(m => m.Especialidad)
                    .FirstOrDefaultAsync(m => m.Id == id)
                ?? throw new KeyNotFoundException(
                    $"No se encontró médico con Id {id}."));
        }
    }
}
