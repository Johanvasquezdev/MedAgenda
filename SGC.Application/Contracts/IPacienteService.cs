using SGC.Application.DTOs.Medical;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de gestion de pacientes
    public interface IPacienteService
    {
        // Registra un nuevo paciente en el sistema
        Task<PacienteResponse> CrearAsync(CrearPacienteRequest request);

        // Obtiene un paciente por su identificador
        Task<PacienteResponse> GetByIdAsync(int id);

        // Obtiene todos los pacientes
        Task<IEnumerable<PacienteResponse>> GetAllAsync();

        // Busca un paciente por su numero de cedula
        Task<PacienteResponse> GetByCedulaAsync(string cedula);

        // Actualiza la informacion de un paciente
        Task ActualizarAsync(ActualizarPacienteRequest request);

        // Desactiva un paciente del sistema
        Task DesactivarAsync(int id);

        // Activa un paciente en el sistema
        Task ActivarAsync(int id);
    }
}
