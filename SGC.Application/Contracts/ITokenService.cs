using SGC.Domain.Entities.Security;

namespace SGC.Application.Contracts
{
    // Servicio para generar y validar tokens JWT de autenticacion
    public interface ITokenService
    {
        // Genera el token JWT del usuario autenticado
        string GenerarToken(Usuario usuario);

        // Valida el token y retorna el ID del usuario si es valido
        int? ValidarToken(string token);
    }
}
