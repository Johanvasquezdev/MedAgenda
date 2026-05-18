using System;
using System.Threading.Tasks;

namespace SGC.Domain.Interfaces
{
    public interface ICacheService // Interfaz para el servicio de caché que almacena datos temporalmente para mejorar el rendimiento
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}