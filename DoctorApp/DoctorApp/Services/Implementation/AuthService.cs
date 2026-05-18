using DoctorApp.DTOs.Responses;
using Microsoft.Maui.Storage;
using DoctorApp.Exceptions;
using DoctorApp.Services.ApiClient;
using DoctorApp.Services.Interfaces;
using DoctorApp.Security;

namespace DoctorApp.Services.Implementation;

/// <summary>
/// Servicio de Autenticacion - Maneja Login/Logout
/// </summary>
public class AuthService : IAuthService
{
    private readonly IApiClient _apiClient;
    private readonly ITokenManager _tokenManager;
    private const string LoginEndpoint = "/api/auth/login";

    public AuthService(IApiClient apiClient, ITokenManager tokenManager)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
    }

    public async Task<AuthTokenResponse> LoginAsync(string usuario, string contrasena)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            throw new ValidationException("Correo y contraseña son requeridos");

        try
        {
            var request = new LoginRequest
            {
                Email = usuario,
                Password = contrasena
            };

            var response = await _apiClient.PostAsync<AuthTokenResponse>(LoginEndpoint, request);

            if (response == null || string.IsNullOrEmpty(response.Token))
                throw new UnauthorizedException("Credenciales inválidas");

            var token = response.Token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? response.Token.Substring("Bearer ".Length).Trim()
                : response.Token.Trim();

            await _tokenManager.SaveTokenAsync(token);
            if (!string.IsNullOrWhiteSpace(response.NombreUsuario))
            {
                // Guarda el nombre para mostrarlo aunque el JWT no tenga el claim.
                Preferences.Set("auth_user_name", response.NombreUsuario.Trim());
            }

            return response;
        }
        catch (UnauthorizedException)
        {
            throw;
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error al iniciar sesión: {ex.Message}", ex);
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _tokenManager.RemoveTokenAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al cerrar sesion: {ex.Message}");
        }
    }

    public async Task<bool> EstaAutenticadoAsync()
    {
        return await _tokenManager.IsTokenValidAsync();
    }
}

/// <summary>
/// DTO para login request
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
