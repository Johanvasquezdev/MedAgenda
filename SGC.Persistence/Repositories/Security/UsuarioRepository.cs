using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Security;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Persistence.Base;
using SGC.Persistence.Context;

namespace SGC.Persistence.Repositories.Security
{
    // Repositorio para operaciones de persistencia de usuarios
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SGCDbContext context, ISGCLogger logger) : base(context, logger) { }

        // Busca un usuario por su direccion de email
        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await ExecuteReadAsync("GetByEmailAsync", async () =>
                await Context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new KeyNotFoundException($"No se encontró un usuario con email {email}."));
        }

        // Permite iniciar sesion con email o con el nombre de la cuenta existente.
        public async Task<Usuario> GetByLoginIdentifierAsync(string identifier)
        {
            var normalizedIdentifier = identifier.Trim().ToLowerInvariant();

            return await ExecuteReadAsync("GetByLoginIdentifierAsync", async () =>
                await Context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedIdentifier)
                ?? await Context.Usuarios
                    .FirstOrDefaultAsync(u => u.Nombre.ToLower() == normalizedIdentifier)
                ?? throw new KeyNotFoundException("No se encontró una cuenta con ese usuario o correo."));
        }

        // Obtiene todos los usuarios que tienen un rol especifico
        public async Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol)
        {
            return await ExecuteReadAsync("GetByRolAsync", async () =>
                await Context.Usuarios
                    .Where(u => u.Rol == rol)
                    .ToListAsync());
        }
    }
}
