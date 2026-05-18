using SGC.Domain.Entities.Medical;

namespace SGC.Domain.Interfaces.Repository
{
    // Interfaz especifica para Paciente, con metodo para buscar por cedula.
    public interface IPacienteRepository : IBaseRepository<Paciente>
    {
        Task<Paciente> GetByCedulaAsync(string cedula);
    }
}
