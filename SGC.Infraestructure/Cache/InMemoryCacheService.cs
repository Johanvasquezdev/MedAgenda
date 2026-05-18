using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using SGC.Domain.Interfaces;

namespace SGC.Infraestructure.Cache
{
    // Servicio de caché en memoria para desarrollo local o como fallback cuando Redis no esté disponible
    public class InMemoryCacheService : ICacheService
    {
        private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

        private class CacheEntry
        {
            public string JsonValue { get; set; } = null!;
            public DateTime? ExpireAt { get; set; }
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.ExpireAt == null || entry.ExpireAt > DateTime.UtcNow)
                {
                    try
                    {
                        var value = JsonSerializer.Deserialize<T>(entry.JsonValue);
                        return Task.FromResult(value);
                    }
                    catch
                    {
                        return Task.FromResult<T?>(default);
                    }
                }
                else
                {
                    _cache.TryRemove(key, out _);
                }
            }
            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            var entry = new CacheEntry
            {
                JsonValue = json,
                ExpireAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null
            };
            _cache[key] = entry;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.ExpireAt == null || entry.ExpireAt > DateTime.UtcNow)
                {
                    return Task.FromResult(true);
                }
                else
                {
                    _cache.TryRemove(key, out _);
                }
            }
            return Task.FromResult(false);
        }
    }
}
