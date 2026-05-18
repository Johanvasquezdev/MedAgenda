using Microsoft.EntityFrameworkCore;
using SGC.Domain.Exceptions;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Context;

namespace SGC.Persistence.Base
{
    /// <summary>
    /// Repositorio genérico base que implementa las operaciones CRUD comunes.
    /// Todos los repositorios específicos heredan de esta clase.
    /// </summary>
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly SGCDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ISGCLogger _logger;

        protected BaseRepository(SGCDbContext context, ISGCLogger logger)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _logger = logger;
        }

        /// <summary>
        /// Expone el contexto a los repositorios hijos para consultas personalizadas.
        /// </summary>
        protected SGCDbContext Context => _context;

        protected ISGCLogger Logger => _logger;

        protected async Task<T> ExecuteReadAsync<T>(string operation, Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"[{GetType().Name}] {operation} not found: {ex.Message}");
                throw new NotFoundDomainException(ex.Message, ex);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError($"[{GetType().Name}] Timeout in {operation}", ex);
                throw new InfrastructureException("No fue posible completar la operación de lectura.", ex);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"[{GetType().Name}] Operation cancelled in {operation}", ex);
                throw new InfrastructureException("La operación fue cancelada durante la lectura.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{GetType().Name}] Unexpected read error in {operation}", ex);
                throw new InfrastructureException("Ocurrió un error al consultar datos.", ex);
            }
        }

        protected async Task ExecuteWriteAsync(string operation, Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning($"[{GetType().Name}] Concurrency conflict in {operation}");
                throw new ValidationDomainException("La información fue modificada por otro proceso. Intente nuevamente.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"[{GetType().Name}] DbUpdateException in {operation}", ex);
                throw new InfrastructureException("No fue posible guardar los cambios en base de datos.", ex);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError($"[{GetType().Name}] Timeout in {operation}", ex);
                throw new InfrastructureException("Se agotó el tiempo al intentar guardar cambios.", ex);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"[{GetType().Name}] Operation cancelled in {operation}", ex);
                throw new InfrastructureException("La operación fue cancelada durante la persistencia.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{GetType().Name}] Unexpected write error in {operation}", ex);
                throw new InfrastructureException("Ocurrió un error al persistir datos.", ex);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await ExecuteReadAsync("GetByIdAsync", async () =>
                await _dbSet.FindAsync(id)
                ?? throw new KeyNotFoundException($"No se encontró la entidad con Id {id}."));
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await ExecuteReadAsync("GetAllAsync", async () => await _dbSet.ToListAsync());
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await ExecuteWriteAsync("AddAsync", async () =>
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            });
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await ExecuteWriteAsync("UpdateAsync", async () =>
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            });
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            await ExecuteWriteAsync("DeleteAsync", async () =>
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            });
        }
    }
}
