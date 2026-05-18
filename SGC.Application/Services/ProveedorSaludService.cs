using SGC.Application.Contracts;
using SGC.Application.DTOs.Catalog;
using SGC.Application.Mappers;
using SGC.Application.Services.Base;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio para gestionar proveedores de salud del sistema
    public class ProveedorSaludService : BaseService, IProveedorSaludService
    {
        // Repositorio para acceso a datos de proveedores
        private readonly IProveedorSaludRepository _proveedorRepository;

        // Validador de reglas de negocio para proveedores
        private readonly ProveedorSaludValidator _validator;

        public ProveedorSaludService(
            IProveedorSaludRepository proveedorRepository,
            ProveedorSaludValidator validator,
            ISGCLogger logger) : base(logger)
        {
            _proveedorRepository = proveedorRepository;
            _validator = validator;
        }

        // Crea un nuevo proveedor validando reglas de negocio
        public async Task<ProveedorSaludResponse> CrearAsync(
            ProveedorSaludRequest request)
        {
            return await ExecuteOperacionAsync(
                "CrearProveedor",
                async () =>
                {
                    var proveedor = ProveedorSaludMapper.ToEntity(request);
                    _validator.Validar(proveedor);
                    await _proveedorRepository.AddAsync(proveedor);
                    return ProveedorSaludMapper.ToResponse(proveedor);
                },
                $"Nombre: {request.Nombre}");
        }

        // Obtiene un proveedor por su ID
        public async Task<ProveedorSaludResponse> GetByIdAsync(int id)
        {
            return await ExecuteOperacionAsync(
                "GetProveedorById",
                async () =>
                {
                    var proveedor = await _proveedorRepository.GetByIdAsync(id);
                    return ProveedorSaludMapper.ToResponse(proveedor);
                },
                $"ProveedorId: {id}");
        }

        // Obtiene todos los proveedores del sistema
        public async Task<IEnumerable<ProveedorSaludResponse>> GetAllAsync()
        {
            return await ExecuteOperacionAsync(
                "GetAllProveedores",
                async () =>
                {
                    var proveedores = await _proveedorRepository.GetAllAsync();
                    return proveedores.Select(ProveedorSaludMapper.ToResponse);
                });
        }

        // Obtiene solo los proveedores activos
        public async Task<IEnumerable<ProveedorSaludResponse>> GetActivosAsync()
        {
            return await ExecuteOperacionAsync(
                "GetProveedoresActivos",
                async () =>
                {
                    var proveedores = await _proveedorRepository.GetActivosAsync();
                    return proveedores.Select(ProveedorSaludMapper.ToResponse);
                });
        }

        // Actualiza los datos de un proveedor existente
        public async Task ActualizarAsync(int id, ProveedorSaludRequest request)
        {
            await ExecuteOperacionAsync(
                "ActualizarProveedor",
                async () =>
                {
                    var proveedor = await _proveedorRepository.GetByIdAsync(id);
                    proveedor.Nombre = request.Nombre;
                    proveedor.Tipo = request.Tipo;
                    proveedor.Telefono = request.Telefono;
                    proveedor.Email = request.Email;
                    _validator.Validar(proveedor);
                    await _proveedorRepository.UpdateAsync(proveedor);
                },
                $"Id: {id}");
        }

        // Desactiva un proveedor usando borrado l�gico
        public async Task EliminarAsync(int id)
        {
            await ExecuteOperacionAsync(
                "EliminarProveedor",
                async () =>
                {
                    var proveedor = await _proveedorRepository.GetByIdAsync(id);
                    proveedor.Activo = false;
                    await _proveedorRepository.UpdateAsync(proveedor);
                },
                $"Id: {id}");
        }
    }
}
