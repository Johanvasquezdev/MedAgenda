using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGC.Domain.Interfaces;
using SGC.Domain.Interfaces.ILogger;
using SGC.Infraestructure.Cache;
using SGC.Infraestructure.Email;
using SGC.Infraestructure.IA;
using SGC.Infraestructure.Logging;
using SGC.Infraestructure.Pagos;
using SGC.Infraestructure.SignalR.Hubs;
using SGC.Infraestructure.SignalR.Services;
using SGC.Infraestructure.SMS;
using StackExchange.Redis;

namespace SGC.Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // Email
            services.AddScoped<IEmailService, EmailService>();

            // SMS
            services.AddScoped<ISmsService, SmsService>();

            // Logger
            services.AddScoped<ISGCLogger, SGCLogger>();

            // Stripe
            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<StripeWebhookService>();

            // AI Chatbot
            services.AddHttpClient<IChatbotService, AnthropicChatService>();

            // Redis (fallback local en Development)
            var redisConnection = config.GetConnectionString("Redis");
            var redisToken = config["ConnectionStrings:RedisToken"];
            var isDevelopment = string.Equals(
                config["ASPNETCORE_ENVIRONMENT"],
                "Development",
                StringComparison.OrdinalIgnoreCase);

            var resolvedRedisConnection = BuildRedisConnectionString(redisConnection, redisToken);
            if (!string.IsNullOrWhiteSpace(resolvedRedisConnection))
            {
                try
                {
                    // Intentar conectar sincrónicamente con un timeout corto para desarrollo
                    var options = ConfigurationOptions.Parse(resolvedRedisConnection);
                    options.ConnectTimeout = 3000; // 3 segundos max
                    options.SyncTimeout = 3000;
                    
                    var multiplexer = ConnectionMultiplexer.Connect(options);
                    services.AddSingleton<IConnectionMultiplexer>(multiplexer);
                    services.AddScoped<ICacheService, RedisCacheService>();
                    Console.WriteLine("[INFO] Conectado exitosamente a Redis Cache.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARN] Error al conectar a Redis: {ex.Message}. Se usará caché en memoria (InMemoryCacheService) como fallback.");
                    services.AddScoped<ICacheService, InMemoryCacheService>();
                }
            }
            else if (isDevelopment)
            {
                Console.WriteLine("[WARN] Redis no configurado. Se usará caché en memoria (InMemoryCacheService) en Development.");
                services.AddScoped<ICacheService, InMemoryCacheService>();
            }
            else
            {
                throw new InvalidOperationException("ConnectionStrings:Redis no configurado");
            }

            // SignalR
            services.AddSignalR();
            services.AddScoped<SignalRNotificacionService>();

            return services;
        }

        private static string? BuildRedisConnectionString(string? redisConnection, string? redisToken)
        {
            if (string.IsNullOrWhiteSpace(redisConnection))
                return null;

            var raw = redisConnection.Trim();
            if (!(raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                return raw;
            }

            if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri) || string.IsNullOrWhiteSpace(uri.Host))
                return null;

            var password = string.IsNullOrWhiteSpace(redisToken)
                ? string.Empty
                : $",password={redisToken.Trim()}";

            var port = uri.Port > 0 ? uri.Port : 6379;
            return $"{uri.Host}:{port},ssl=true,abortConnect=false{password}";
        }
    }
}
