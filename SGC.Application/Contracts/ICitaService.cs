using SGC.Application.DTOs.Appointments;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de gestion de citas medicas
    public interface ICitaService
    {
        // Agenda una nueva cita despues de validar reglas de negocio
        Task<CitaResponse> AgendarAsync(CrearCitaRequest request);

        // Obtiene una cita por su identificador
        Task<CitaResponse> GetByIdAsync(int id);

        // Obtiene todas las citas del sistema
        Task<IEnumerable<CitaResponse>> GetAllAsync();

        // Obtiene todas las citas de un paciente
        Task<IEnumerable<CitaResponse>> GetByPacienteAsync(int pacienteId);

        // Obtiene todas las citas de un medico
        Task<IEnumerable<CitaResponse>> GetByMedicoAsync(int medicoId);

        // Obtiene todas las citas de una fecha especifica
        Task<IEnumerable<CitaResponse>> GetByFechaAsync(DateTime fecha);

        // Confirma una cita solicitada
        Task ConfirmarAsync(int citaId);

        // Cancela una cita registrando el motivo
        Task CancelarAsync(int citaId, string motivo);

        // Cancela una cita por accion del medico y notifica al paciente
        Task CancelarPorMedicoAsync(int citaId, string motivo);

        // Rechaza una cita solicitada registrando el motivo
        Task RechazarAsync(int citaId, string motivo);

        // Reprograma una cita a una nueva fecha
        Task ReprogramarAsync(int citaId, DateTime nuevaFecha);

        // Marca una cita confirmada como no asistida
        Task MarcarNoAsistioAsync(int citaId);

        // Inicia una consulta medica
        Task IniciarConsultaAsync(int citaId);

        // Completa una cita en progreso
        Task CompletarAsync(int citaId);
    }
}
