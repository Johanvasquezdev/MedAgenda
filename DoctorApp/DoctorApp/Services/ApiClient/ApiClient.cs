using System.Net;
using System.Text.Json;
using DoctorApp.Exceptions;

namespace DoctorApp.Services.ApiClient;

/// <summary>
/// Generic API client with JSON serialization and error handling
/// </summary>
public interface IApiClient
{
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object? data = null);
    Task<T> PutAsync<T>(string endpoint, object? data = null);
    Task DeleteAsync(string endpoint);
    Task<HttpResponseMessage> GetRawAsync(string endpoint);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            throw new ConnectionException($"Error en GET {endpoint}: {ex.Message}", ex);
        }
    }

    public async Task<T> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            HttpContent? content = null;
            if (data != null)
            {
                content = new StringContent(
                    JsonSerializer.Serialize(data, _jsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
            }

            var response = await _httpClient.PostAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex) when (!(ex is AppException))
        {
            throw new ConnectionException($"Error en POST {endpoint}: {ex.Message}", ex);
        }
    }

    public async Task<T> PutAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            HttpContent? content = null;
            if (data != null)
            {
                content = new StringContent(
                    JsonSerializer.Serialize(data, _jsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
            }

            var response = await _httpClient.PutAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex) when (!(ex is AppException))
        {
            throw new ConnectionException($"Error en PUT {endpoint}: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }
        }
        catch (Exception ex) when (!(ex is AppException))
        {
            throw new ConnectionException($"Error en DELETE {endpoint}: {ex.Message}", ex);
        }
    }

    public Task<HttpResponseMessage> GetRawAsync(string endpoint)
    {
        return _httpClient.GetAsync(endpoint);
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                    return default!;

                return JsonSerializer.Deserialize<T>(json, _jsonOptions)
                    ?? throw new AppException("Respuesta vacia del servidor");
            }
            catch (JsonException ex)
            {
                throw new AppException($"Error deserializando respuesta: {ex.Message}", "JSON_ERROR", 500, ex);
            }
        }

        await HandleErrorResponse(response);
        throw new AppException("Error inesperado", "UNKNOWN_ERROR", (int?)response.StatusCode);
    }

    private async Task HandleErrorResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedException();

            case HttpStatusCode.NotFound:
                throw new AppException(content ?? "El recurso solicitado no fue encontrado", "NOT_FOUND", 404);

            case HttpStatusCode.Conflict:
                throw new ConflictException(content);

            case HttpStatusCode.BadRequest:
                try
                {
                    var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content, _jsonOptions);
                    var errors = problemDetails?.Errors ?? new Dictionary<string, string[]>();
                    throw new Exceptions.ValidationException("Error de validacion en el servidor", errors);
                }
                catch (Exceptions.ValidationException)
                {
                    throw;
                }
                catch
                {
                    throw new Exceptions.ValidationException(content ?? "Datos invalidos");
                }

            case HttpStatusCode.InternalServerError:
                throw new ConnectionException($"Error interno del servidor (500): {content}");

            case HttpStatusCode.ServiceUnavailable:
                throw new ConnectionException("El servidor no esta disponible (503). Intenta mas tarde.");

            case HttpStatusCode.GatewayTimeout:
                throw new ConnectionException("El servidor tardo demasiado en responder (504).");

            default:
                throw new AppException(
                    content ?? $"Error HTTP {(int)response.StatusCode}",
                    $"HTTP_{(int)response.StatusCode}",
                    (int)response.StatusCode
                );
        }
    }
}

/// <summary>
/// Validation error model for ASP.NET Core
/// </summary>
public class ValidationProblemDetails
{
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
