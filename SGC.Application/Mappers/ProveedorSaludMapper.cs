using SGC.Application.DTOs.Catalog;
using SGC.Domain.Entities.Catalog;

namespace SGC.Application.Mappers
{
    // Mapper para convertir entre la entidad ProveedorSalud y sus DTOs de solicitud y respuesta.
    public static class ProveedorSaludMapper
    {
        public static ProveedorSaludResponse ToResponse(
            ProveedorSalud proveedor)
        {
            return new ProveedorSaludResponse
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                Tipo = proveedor.Tipo,
                Telefono = proveedor.Telefono,
                Email = proveedor.Email,
                Activo = proveedor.Activo
            };
        }

        // Para actualizar una entidad existente con los datos de la solicitud. El Id se mantiene igual.
        public static ProveedorSalud ToEntity(
            ProveedorSaludRequest request)
        {
            return new ProveedorSalud
            {
                Nombre = request.Nombre,
                Tipo = request.Tipo,
                Telefono = request.Telefono,
                Email = request.Email
            };
        }
    }
}