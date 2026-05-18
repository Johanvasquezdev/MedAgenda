using DoctorApp.Services.ApiClient;

namespace DoctorApp.Services.Mock;

/// <summary>
/// Mock ApiClient que no hace llamadas HTTP reales
/// </summary>
public class MockApiClient : IApiClient
{
    public Task<T> GetAsync<T>(string endpoint)
    {
        // Retorna default para mocks
        return Task.FromResult(default(T));
    }

    public Task<T> PostAsync<T>(string endpoint, object? data = null)
    {
        return Task.FromResult(default(T));
    }

    public Task<T> PutAsync<T>(string endpoint, object? data = null)
    {
        return Task.FromResult(default(T));
    }

    public Task DeleteAsync(string endpoint)
    {
        return Task.CompletedTask;
    }

    public Task<HttpResponseMessage> GetRawAsync(string endpoint)
    {
        return Task.FromResult(new HttpResponseMessage());
    }
}
