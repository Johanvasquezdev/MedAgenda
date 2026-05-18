using SGC.Application.DTOs.Catalog;
using SGC.Domain.Entities.Catalog;

namespace SGC.Application.Mappers
{
    // Mapper para convertir entre la entidad Especialidad y sus DTOs de solicitud y respuesta.
    public static class EspecialidadMapper
    {
        // Convierte una entidad Especialidad a un DTO de respuesta EspecialidadResponse.
        public static EspecialidadResponse ToResponse(
            Especialidad especialidad)
        {
            return new EspecialidadResponse
            {
                Id = especialidad.Id,
                Nombre = especialidad.Nombre,
                Descripcion = especialidad.Descripcion,
                Activo = especialidad.Activo
            };
        }

        // Convierte un DTO de solicitud EspecialidadRequest a una entidad Especialidad.
        public static Especialidad ToEntity(EspecialidadRequest request)
        {
            return new Especialidad
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion
            };
        }
    }
}