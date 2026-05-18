using Microsoft.Maui.Storage;

namespace DoctorApp.Security;

/// <summary>
/// Gestor de tokens JWT usando SecureStorage
/// Implementa almacenamiento seguro del token de autenticacion
/// </summary>
public interface ITokenManager
{
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
    Task RemoveTokenAsync();
    Task<bool> IsTokenValidAsync();
    bool IsTokenExpired(string token);
    Task<int?> GetUserIdAsync();
    Task<string?> GetUserNameAsync();
}

public class TokenManager : ITokenManager
{
    private const string TOKEN_KEY = "jwt_token";
    private const string LEGACY_TOKEN_KEY = "auth_token";
    private const string TOKEN_EXPIRY_KEY = "token_expiry";
    private const string USER_NAME_KEY = "auth_user_name";

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var token = Preferences.Get(TOKEN_KEY, string.Empty);
            if (string.IsNullOrEmpty(token))
                token = await SecureStorage.GetAsync(TOKEN_KEY);

            if (string.IsNullOrEmpty(token))
            {
                token = Preferences.Get(LEGACY_TOKEN_KEY, string.Empty);
                if (string.IsNullOrEmpty(token))
                    token = await SecureStorage.GetAsync(LEGACY_TOKEN_KEY);
            }

            if (string.IsNullOrEmpty(token))
                return null;

            if (IsTokenExpired(token))
            {
                await RemoveTokenAsync();
                return null;
            }

            return token;
        }
        catch
        {
            return Preferences.Get(TOKEN_KEY, null);
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync(TOKEN_KEY, token);
            Preferences.Set(TOKEN_KEY, token);

            var expiryTime = DateTime.UtcNow.AddHours(24).ToString("O");
            await SecureStorage.SetAsync(TOKEN_EXPIRY_KEY, expiryTime);
            Preferences.Set(TOKEN_EXPIRY_KEY, expiryTime);
        }
        catch (Exception ex)
        {
            Preferences.Set(TOKEN_KEY, token);
            Preferences.Set(TOKEN_EXPIRY_KEY, DateTime.UtcNow.AddHours(24).ToString("O"));
            System.Diagnostics.Debug.WriteLine($"Error saving token in SecureStorage, fallback to Preferences: {ex.Message}");
        }
    }

    public async Task RemoveTokenAsync()
    {
        try
        {
            SecureStorage.Remove(TOKEN_KEY);
            SecureStorage.Remove(LEGACY_TOKEN_KEY);
            SecureStorage.Remove(TOKEN_EXPIRY_KEY);
            Preferences.Remove(TOKEN_KEY);
            Preferences.Remove(LEGACY_TOKEN_KEY);
            Preferences.Remove(TOKEN_EXPIRY_KEY);
            Preferences.Remove(USER_NAME_KEY);
        }
        catch
        {
            // ignore
        }
    }

    public async Task<bool> IsTokenValidAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
    }

    public async Task<int?> GetUserIdAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        var value = GetClaimValue(token, "nameid")
            ?? GetClaimValue(token, "nameidentifier")
            ?? GetClaimValue(token, "sub")
            ?? GetClaimValue(token, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (int.TryParse(value, out var id))
            return id;

        return null;
    }

    public async Task<string?> GetUserNameAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return Preferences.Get(USER_NAME_KEY, null);

        return GetClaimValue(token, "unique_name")
            ?? GetClaimValue(token, "name")
            ?? GetClaimValue(token, "given_name")
            ?? GetClaimValue(token, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
            ?? Preferences.Get(USER_NAME_KEY, null);
    }

    /// <summary>
    /// Decodifica el JWT y verifica si esta expirado (sin validar firma)
    /// </summary>
    public bool IsTokenExpired(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                return true;

            var payload = DecodeBase64Url(parts[1]);
            if (payload == null)
                return true;

            var decodedBytes = payload;
            var decodedPayload = System.Text.Encoding.UTF8.GetString(decodedBytes);

            if (System.Text.Json.JsonDocument.Parse(decodedPayload).RootElement.TryGetProperty("exp", out var expElement))
            {
                var expirationUnix = expElement.GetInt64();
                var expirationDate = UnixTimeStampToDateTime(expirationUnix);
                return DateTime.UtcNow >= expirationDate;
            }

            return false;
        }
        catch
        {
            return true;
        }
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        // exp en JWT es UTC. No convertir a hora local para comparar con UtcNow.
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
    }

    private static string? GetClaimValue(string token, string claimName)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                return null;

            var decodedBytes = DecodeBase64Url(parts[1]);
            if (decodedBytes == null)
                return null;
            var decodedPayload = System.Text.Encoding.UTF8.GetString(decodedBytes);

            using var doc = System.Text.Json.JsonDocument.Parse(decodedPayload);
            if (doc.RootElement.TryGetProperty(claimName, out var claimValue))
                return claimValue.GetString();

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static byte[]? DecodeBase64Url(string input)
    {
        try
        {
            var normalized = input.Replace('-', '+').Replace('_', '/');
            normalized = normalized.PadRight(normalized.Length + (4 - normalized.Length % 4) % 4, '=');
            return Convert.FromBase64String(normalized);
        }
        catch
        {
            return null;
        }
    }
}
