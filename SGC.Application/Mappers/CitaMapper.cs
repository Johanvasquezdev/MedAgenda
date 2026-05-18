using SGC.Application.DTOs.Appointments;
using SGC.Domain.Entities.Appointments;

namespace SGC.Application.Mappers
{
    public static class CitaMapper // Mapper estatico para convertir entre la entidad Cita y los DTOs relacionados (CitaResponse, CrearCitaRequest, ActualizarCitaRequest).
    {
        // Convierte una entidad Cita a un DTO CitaResponse, incluyendo información adicional como el nombre del paciente y del médico para facilitar la presentación en la capa de aplicación.
        public static CitaResponse ToResponse(Cita cita)
        {
            return new CitaResponse
            {
                Id = cita.Id,
                FechaHora = cita.FechaHora,
                Estado = cita.Estado.ToString(),
                Motivo = cita.Motivo,
                Notas = cita.Notas,
                PacienteId = cita.PacienteId,
                MedicoId = cita.MedicoId,
                DisponibilidadId = cita.DisponibilidadId,
                FechaCreacion = cita.FechaCreacion,
                PacienteNombre = cita.Paciente?.Nombre ?? string.Empty,
                MedicoNombre = cita.Medico?.Nombre ?? string.Empty
            };
        }

        // Convierte un DTO CrearCitaRequest a una entidad Cita, asignando los valores correspondientes para crear una nueva cita en el sistema. Este método se utiliza en la capa de aplicación para recibir los datos de entrada y transformarlos en la entidad que se persistirá en la base de datos.
        public static Cita ToEntity(CrearCitaRequest request)
        {
            return new Cita
            {
                PacienteId = request.PacienteId,
                MedicoId = request.MedicoId,
                DisponibilidadId = request.DisponibilidadId,
                FechaHora = request.FechaHora,
                Motivo = request.Motivo
            };
        }

        // Actualiza una entidad Cita existente con los valores proporcionados en un DTO ActualizarCitaRequest. Este método permite modificar los atributos de una cita, como la fecha y hora, el motivo o las notas, sin afectar otros campos que no se desean cambiar. Es útil para la funcionalidad de actualización de citas en la capa de aplicación.
        public static void UpdateEntity(Cita cita,
            ActualizarCitaRequest request)
        {
            cita.FechaHora = request.FechaHora ?? cita.FechaHora;
            cita.Motivo = request.Motivo ?? cita.Motivo;
            cita.Notas = request.Notas ?? cita.Notas;
        }
    }
}
