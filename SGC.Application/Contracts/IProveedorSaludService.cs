using SGC.Application.DTOs.Catalog;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de gestion de proveedores de salud
    public interface IProveedorSaludService
    {
        // Crea un nuevo proveedor de salud
        Task<ProveedorSaludResponse> CrearAsync(ProveedorSaludRequest request);

        // Obtiene un proveedor por su identificador
        Task<ProveedorSaludResponse> GetByIdAsync(int id);

        // Obtiene todos los proveedores
        Task<IEnumerable<ProveedorSaludResponse>> GetAllAsync();

        // Obtiene solo los proveedores activos
        Task<IEnumerable<ProveedorSaludResponse>> GetActivosAsync();

        // Actualiza un proveedor existente
        Task ActualizarAsync(int id, ProveedorSaludRequest request);

        // Elimina (desactiva) un proveedor
        Task EliminarAsync(int id);
    }
}
