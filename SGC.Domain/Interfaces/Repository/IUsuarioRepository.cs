using SGC.Domain.Entities.Security;
using SGC.Domain.Enums;

namespace SGC.Domain.Interfaces.Repository
{
    // Interfaz especifica para la entidad Usuario, con metodos para consultar por email o rol.
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol);
    }
}
