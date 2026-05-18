using SGC.Application.DTOs.Medical;

namespace SGC.Application.Contracts
{
    // Contrato para las operaciones de gestion de medicos
    public interface IMedicoService
    {
        // Registra un nuevo medico en el sistema
        Task<MedicoResponse> CrearAsync(CrearMedicoRequest request);

        // Obtiene un medico por su identificador
        Task<MedicoResponse> GetByIdAsync(int id);

        // Obtiene todos los medicos
        Task<IEnumerable<MedicoResponse>> GetAllAsync();

        // Busca un medico por su numero de exequatur
        Task<MedicoResponse> GetByExequaturAsync(string exequatur);

        // Obtiene todos los medicos de una especialidad
        Task<IEnumerable<MedicoResponse>> GetByEspecialidadAsync(int especialidadId);

        // Actualiza la informacion de un medico
        Task ActualizarAsync(ActualizarMedicoRequest request);

        // Desactiva un medico del sistema
        Task DesactivarAsync(int id);

        // Activa un medico en el sistema
        Task ActivarAsync(int id);
    }
}
