using SGC.Application.DTOs.Appointments;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de gestion de disponibilidad de medicos
    public interface IDisponibilidadService
    {
        // Crea un nuevo horario de disponibilidad para un medico
        Task<DisponibilidadResponse> CrearAsync(DisponibilidadRequest request);

        // Obtiene una disponibilidad por su identificador
        Task<DisponibilidadResponse> GetByIdAsync(int id);

        // Obtiene todas las disponibilidades
        Task<IEnumerable<DisponibilidadResponse>> GetAllAsync();

        // Obtiene todos los horarios disponibles de un medico
        Task<IEnumerable<DisponibilidadResponse>> GetByMedicoAsync(int medicoId);

        // Obtiene todas las disponibilidades de un dia de la semana
        Task<IEnumerable<DisponibilidadResponse>> GetByDiaAsync(int diaSemana);

        // Actualiza un horario de disponibilidad existente
        Task ActualizarAsync(int id, DisponibilidadRequest request);

        // Elimina un horario de disponibilidad
        Task EliminarAsync(int id);
    }
}
