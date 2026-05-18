using SGC.Application.DTOs.Catalog;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de gestion de especialidades medicas
    public interface IEspecialidadService
    {
        // Crea una nueva especialidad medica
        Task<EspecialidadResponse> CrearAsync(EspecialidadRequest request);

        // Obtiene una especialidad por su identificador
        Task<EspecialidadResponse> GetByIdAsync(int id);

        // Obtiene todas las especialidades
        Task<IEnumerable<EspecialidadResponse>> GetAllAsync();

        // Obtiene solo las especialidades activas
        Task<IEnumerable<EspecialidadResponse>> GetActivasAsync();

        // Actualiza una especialidad existente
        Task ActualizarAsync(int id, EspecialidadRequest request);

        // Elimina (desactiva) una especialidad
        Task EliminarAsync(int id);
    }
}
