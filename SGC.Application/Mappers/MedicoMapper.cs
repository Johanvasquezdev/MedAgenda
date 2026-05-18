using SGC.Application.DTOs.Medical;
using SGC.Domain.Entities.Medical;

namespace SGC.Application.Mappers
{
    // Mapper para convertir entre la entidad Medico y los DTOs de solicitud/respuesta
    public static class MedicoMapper
    {
        // Convierte una entidad Medico a un DTO de respuesta MedicoResponse
        public static MedicoResponse ToResponse(Medico medico)
        {
            return new MedicoResponse
            {
                Id = medico.Id,
                Nombre = medico.Nombre,
                Email = medico.Email,
                Exequatur = medico.Exequatur,
                EspecialidadId = medico.EspecialidadId,
                NombreEspecialidad = medico.Especialidad?.Nombre,
                ProveedorSaludId = medico.ProveedorSaludId,
                Especialidad = medico.Especialidad?.Nombre,
                ProveedorSalud = medico.ProveedorSalud?.Nombre,
                TelefonoConsultorio = medico.TelefonoConsultorio,
                Foto = medico.Foto,
                Activo = medico.Activo,
                MedicoActivo = medico.MedicoActivo
            };
        }

        // Convierte un DTO de solicitud CrearMedicoRequest a una entidad Medico para crear un nuevo medico
        public static Medico ToEntity(CrearMedicoRequest request)
        {
            return new Medico
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = request.Password,
                Exequatur = request.Exequatur,
                EspecialidadId = request.EspecialidadId,
                ProveedorSaludId = request.ProveedorSaludId,
                TelefonoConsultorio = request.TelefonoConsultorio
            };
        }

        // Actualiza una entidad Medico existente con los datos de un DTO de solicitud ActualizarMedicoRequest, solo si los campos no son nulos
        public static void UpdateEntity(Medico medico,
            ActualizarMedicoRequest request)
        {
            medico.Nombre = request.Nombre ?? medico.Nombre;
            medico.Exequatur = request.Exequatur ?? medico.Exequatur;
            medico.EspecialidadId = request.EspecialidadId ?? medico.EspecialidadId;
            medico.TelefonoConsultorio = request.TelefonoConsultorio ?? medico.TelefonoConsultorio;
            medico.Foto = request.Foto ?? medico.Foto;
        }
    }
}
