using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Domain.Interfaces.Repository
{

    // Interfaz genérica base para el patrón Repository.Define las operaciones CRUD estándar que todas las entidades del sistema compartirán.Esto permite desacoplar la lógica de negocio de la base de datos (PostgreSQL).
    public interface IBaseRepository<T> where T : class
    {
    
        Task<T> GetByIdAsync(int id);
        // Obtiene un registro único de forma asíncrona mediante su identificador primario.


        // Recupera todos los registros de la entidad de forma asíncrona.
        Task<IEnumerable<T>> GetAllAsync();


        // Prepara una nueva entidad para ser insertada en la base de datos de forma asíncrona.
        Task AddAsync(T entity);

        
        // Marca una entidad existente para ser actualizada de forma asincrona.
        // Las validaciones de negocio deben ocurrir en la capa de Application antes de llamar a este método.
        Task UpdateAsync(T entity);


        // Marca una entidad para ser eliminada de la persistencia de forma asincrona.
        // Para el SGCM se recomienda usar borrado lógico (cambiar estado a Inactivo) 
        Task DeleteAsync(T entity);
    }
}