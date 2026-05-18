using SGC.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

namespace SGC.Infraestructure.Cache
{
    // Implementación concreta del servicio de caché utilizando Redis con fallback en memoria resiliente ante fallas de conexión o timeouts
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase? _db;
        private static readonly ConcurrentDictionary<string, (string Json, DateTime? ExpireAt)> _localFallback = new();
        private readonly bool _useFallbackOnly;

        public RedisCacheService(IConnectionMultiplexer? redis = null)
        {
            try
            {
                if (redis != null)
                {
                    _db = redis.GetDatabase();
                }
                else
                {
                    _useFallbackOnly = true;
                    Console.WriteLine("[WARN] Instancia de Redis nula. Se usará el fallback en memoria.");
                }
            }
            catch (Exception ex)
            {
                _useFallbackOnly = true;
                Console.WriteLine($"[WARN] Error al inicializar base de datos de Redis: {ex.Message}. Usando fallback en memoria.");
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (_useFallbackOnly || _db == null)
            {
                return GetFromLocalFallback<T>(key);
            }

            try
            {
                var value = await _db.StringGetAsync(key);
                if (value.IsNullOrEmpty) return default;
                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Falló la lectura en Redis para la clave '{key}' ({ex.Message}). Usando fallback en memoria.");
                return GetFromLocalFallback<T>(key);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            
            if (_useFallbackOnly || _db == null)
            {
                SetInLocalFallback(key, json, expiration);
                return;
            }

            try
            {
                await _db.StringSetAsync(
                    key,
                    json,
                    expiration,
                    false,
                    When.Always,
                    CommandFlags.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Falló la escritura en Redis para la clave '{key}' ({ex.Message}). Guardando en fallback en memoria.");
                SetInLocalFallback(key, json, expiration);
            }
        }

        public async Task RemoveAsync(string key)
        {
            _localFallback.TryRemove(key, out _);

            if (_useFallbackOnly || _db == null) return;

            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Falló la eliminación en Redis para la clave '{key}' ({ex.Message}).");
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (_useFallbackOnly || _db == null)
            {
                return ExistsInLocalFallback(key);
            }

            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Falló la verificación de existencia en Redis para la clave '{key}' ({ex.Message}). Usando fallback en memoria.");
                return ExistsInLocalFallback(key);
            }
        }

        // --- Helper Métodos para Local Fallback ---

        private T? GetFromLocalFallback<T>(string key)
        {
            if (_localFallback.TryGetValue(key, out var entry))
            {
                if (entry.ExpireAt == null || entry.ExpireAt > DateTime.UtcNow)
                {
                    try
                    {
                        return JsonSerializer.Deserialize<T>(entry.Json);
                    }
                    catch
                    {
                        return default;
                    }
                }
                else
                {
                    _localFallback.TryRemove(key, out _);
                }
            }
            return default;
        }

        private void SetInLocalFallback(string key, string json, TimeSpan? expiration)
        {
            var expireAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : (DateTime?)null;
            _localFallback[key] = (json, expireAt);
        }

        private bool ExistsInLocalFallback(string key)
        {
            if (_localFallback.TryGetValue(key, out var entry))
            {
                if (entry.ExpireAt == null || entry.ExpireAt > DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    _localFallback.TryRemove(key, out _);
                }
            }
            return false;
        }
    }
}
