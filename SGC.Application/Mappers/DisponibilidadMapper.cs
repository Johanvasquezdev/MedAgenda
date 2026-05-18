using SGC.Application.DTOs.Appointments;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Enums;

namespace SGC.Application.Mappers
{
    // Mapper para convertir entre la entidad Disponibilidad y los DTOs de solicitud y respuesta
    public static class DisponibilidadMapper
    {
        // Convierte una entidad Disponibilidad a un DTO de respuesta para la API
        public static DisponibilidadResponse ToResponse(
            Disponibilidad disponibilidad)
        {
            return new DisponibilidadResponse
            {
                Id = disponibilidad.Id,
                MedicoId = disponibilidad.MedicoId,
                DiaSemana = disponibilidad.DiaSemana.ToString(),
                HoraInicio = disponibilidad.HoraInicio,
                HoraFin = disponibilidad.HoraFin,
                DuracionCitaMin = disponibilidad.DuracionCitaMin,
                EsRecurrente = disponibilidad.EsRecurrente
            };
        }

        // Convierte un DTO de solicitud a una entidad Disponibilidad para la creación o actualización
        public static Disponibilidad ToEntity(
            DisponibilidadRequest request)
        {
            return new Disponibilidad
            {
                MedicoId = request.MedicoId,
                // Mapea el int del request al enum de dominio.
                DiaSemana = (DiaSemana)request.DiaSemana,
                HoraInicio = request.HoraInicio,
                HoraFin = request.HoraFin,
                DuracionCitaMin = request.DuracionCitaMin,
                EsRecurrente = request.EsRecurrente
            };
        }
    }
}
