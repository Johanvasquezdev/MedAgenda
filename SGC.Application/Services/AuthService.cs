using BCrypt.Net;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Security;
using SGC.Application.Services.Base;
using SGC.Domain.Exceptions;
using SGC.Domain.Interfaces;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio de autenticacion para el sistema
    public class AuthService : BaseService, IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService,
            IEmailService emailService,
            ICacheService cacheService,
            ISGCLogger logger) : base(logger)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _cacheService = cacheService;
        }

        // Metodo para autenticar a un usuario y generar un token JWT
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            return await ExecuteOperacionAsync(
                "Login",
                async () =>
                {
                    var usuario = await _usuarioRepository
                        .GetByEmailAsync(request.Email);

                    if (!usuario.Activo)
                        throw new UnauthorizedAccessException(
                            "El usuario está desactivado.");

                    try
                    {
                        if (string.IsNullOrWhiteSpace(usuario.PasswordHash) ||
                            !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                            throw new UnauthorizedAccessException(
                                "Credenciales incorrectas.");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new ValidationDomainException(
                            "Credenciales incorrectas.", ex);
                    }

                    var token = _tokenService.GenerarToken(usuario);
                    LogOperacion("LoginExitoso", $"UsuarioId: {usuario.Id}");

                    return new LoginResponse
                    {
                        Token = token,
                        NombreUsuario = usuario.Nombre,
                        Rol = usuario.Rol.ToString(),
                        Expiracion = DateTime.UtcNow.AddMinutes(60)
                    };
                },
                $"Email: {request.Email}");
        }

        // Genera un token de reset, lo guarda en cache y envía el email
        public async Task SolicitarPasswordResetAsync(ForgotPasswordRequest request)
        {
            await ExecuteOperacionAsync(
                "SolicitarPasswordReset",
                async () =>
                {
                    try
                    {
                        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
                        if (usuario == null || !usuario.Activo) return;

                        // Generar token URL-safe
                        var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                            .Replace("+", "-").Replace("/", "_").Replace("=", "");

                        // Guardar en cache por 1 hora
                        var cacheKey = $"password_reset:{request.Email}";
                        await _cacheService.SetAsync(cacheKey, resetToken, TimeSpan.FromHours(1));

                        // Link de reset apuntando al frontend
                        var resetLink = $"http://localhost:3000/reset-password?token={resetToken}&email={Uri.EscapeDataString(request.Email)}";
                        await _emailService.EnviarPasswordResetAsync(request.Email, resetLink);
                        LogOperacion("PasswordResetSolicitado", $"Email: {request.Email}");
                    }
                    catch (Exception ex)
                    {
                        // Log el error real para diagnosticar (no se expone al cliente)
                        LogOperacion("PasswordResetError", $"Email: {request.Email} | Error: {ex.Message}");
                    }
                },
                $"Email: {request.Email}");
        }

        // Valida el token y actualiza la contraseña del usuario
        public async Task RestablecerPasswordAsync(ResetPasswordRequest request)
        {
            await ExecuteOperacionAsync(
                "RestablecerPassword",
                async () =>
                {
                    var cacheKey = $"password_reset:{request.Email}";
                    var tokenGuardado = await _cacheService.GetAsync<string>(cacheKey);

                    if (tokenGuardado == null || tokenGuardado != request.Token)
                        throw new ValidationDomainException("Token inválido o expirado.");

                    var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
                    if (usuario == null || !usuario.Activo)
                        throw new ValidationDomainException("Usuario no encontrado.");

                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NuevaPassword);
                    await _usuarioRepository.UpdateAsync(usuario);

                    // Invalidar token tras el uso exitoso
                    await _cacheService.RemoveAsync(cacheKey);
                    LogOperacion("PasswordRestablecido", $"UsuarioId: {usuario.Id}");
                },
                $"Email: {request.Email}");
        }
    }
}
