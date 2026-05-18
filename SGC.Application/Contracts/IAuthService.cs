using SGC.Application.DTOs.Security;
using System.Threading.Tasks;

namespace SGC.Application.Contracts
{
    public interface IAuthService // Interfaz para el servicio de autenticacion, que define el contrato para el proceso de login.
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task SolicitarPasswordResetAsync(ForgotPasswordRequest request);
        Task RestablecerPasswordAsync(ResetPasswordRequest request);
    }
}