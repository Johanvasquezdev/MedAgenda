using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.Text.Json;
using DoctorApp.Security;
using DoctorApp.Exceptions;

namespace DoctorApp.Services.Hubs
{
    /// <summary>
    /// Cliente SignalR para el hub de disponibilidad
    /// Administra conexión a tiempo real con backend para eventos de disponibilidad
    /// </summary>
    public class DisponibilidadHubClient : IDisponibilidadHubClient
    {
        private HubConnection? _hubConnection;
        private readonly ITokenManager _tokenManager;
        private readonly string _hubEndpoint;

        public event EventHandler<DisponibilidadHubEventArgs>? OnDisponibilidadCreada;
        public event EventHandler<DisponibilidadHubEventArgs>? OnDisponibilidadActualizada;
        public event EventHandler<DisponibilidadHubEventArgs>? OnDisponibilidadEliminada;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public DisponibilidadHubClient(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
            _hubEndpoint = ResolveHubEndpoint();
        }

        private static string ResolveHubEndpoint()
        {
            var apiBaseUrl = Environment.GetEnvironmentVariable("SGC_API_BASE_URL");
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                apiBaseUrl = "http://localhost:5189";

            apiBaseUrl = apiBaseUrl.Replace("https://localhost:7224", "http://localhost:5189");

            return $"{apiBaseUrl.TrimEnd('/')}/disponibilidadhub";
        }

        /// <summary>
        /// Conecta al hub de disponibilidad de forma asincrónica
        /// Configura handlers para los eventos del servidor
        /// </summary>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Evitar múltiples conexiones
                if (_hubConnection?.State == HubConnectionState.Connected)
                    return;

                // Obtener token para autenticación
                var token = await _tokenManager.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    throw new UnauthorizedException("No hay token de autenticación disponible");

                // Crear conexión con bearer token
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubEndpoint, options =>
                    {
                        options.AccessTokenProvider = async () => token;
                    })
                    .WithAutomaticReconnect(new[] 
                    { 
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    })
                    .Build();

                // Registrar handlers de eventos del servidor
                // El backend actual publica un único evento con (medicoId, disponibilidadDto)
                _hubConnection.On<int, object>("DisponibilidadActualizada",
                    (medicoId, payload) =>
                    {
                        var args = new DisponibilidadHubEventArgs
                        {
                            DisponibilidadId = TryReadInt(payload, "id") ?? TryReadInt(payload, "Id") ?? 0,
                            DiaSemana = TryReadString(payload, "diaSemana") ?? TryReadString(payload, "DiaSemana") ?? string.Empty,
                            HoraInicio = TryReadTimeSpan(payload, "horaInicio") ?? TryReadTimeSpan(payload, "HoraInicio") ?? TimeSpan.Zero,
                            HoraFin = TryReadTimeSpan(payload, "horaFin") ?? TryReadTimeSpan(payload, "HoraFin") ?? TimeSpan.Zero,
                            DuracionMinutos = TryReadInt(payload, "duracionMinutos") ?? TryReadInt(payload, "DuracionMinutos") ?? 0,
                            Activo = TryReadBool(payload, "activo") ?? TryReadBool(payload, "Activo") ?? true,
                            CitasAsignadas = TryReadInt(payload, "citasAsignadas") ?? TryReadInt(payload, "CitasAsignadas") ?? 0,
                            Timestamp = DateTime.UtcNow
                        };

                        // Mantenemos compatibilidad con los eventos existentes del cliente.
                        OnDisponibilidadActualizada?.Invoke(this, args);
                        if (args.DisponibilidadId == 0)
                        {
                            Debug.WriteLine($"[DisponibilidadHubClient] Evento recibido para medico {medicoId} sin id de disponibilidad");
                        }
                    });

                // Handlers de reconexión
                _hubConnection.Reconnecting += error =>
                {
                    Debug.WriteLine($"[DisponibilidadHubClient] Reconnecting: {error?.Message}");
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnected += connectionId =>
                {
                    Debug.WriteLine($"[DisponibilidadHubClient] Reconnected: {connectionId}");
                    return Task.CompletedTask;
                };

                _hubConnection.Closed += error =>
                {
                    Debug.WriteLine($"[DisponibilidadHubClient] Connection closed: {error?.Message}");
                    return Task.CompletedTask;
                };

                // Conectar al hub
                await _hubConnection.StartAsync(cancellationToken);
                Debug.WriteLine("[DisponibilidadHubClient] Connected successfully");
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConnectionException($"Error al conectar al hub de disponibilidad: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Desconecta del hub de disponibilidad
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                if (_hubConnection != null)
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DisponibilidadHubClient] Error during disconnect: {ex.Message}");
            }
        }

        private static JsonElement? AsObjectElement(object payload)
        {
            if (payload is JsonElement element && element.ValueKind == JsonValueKind.Object)
                return element;

            return null;
        }

        private static int? TryReadInt(object payload, string propertyName)
        {
            var obj = AsObjectElement(payload);
            if (obj == null || !obj.Value.TryGetProperty(propertyName, out var property))
                return null;

            if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var number))
                return number;

            if (property.ValueKind == JsonValueKind.String && int.TryParse(property.GetString(), out var parsed))
                return parsed;

            return null;
        }

        private static string? TryReadString(object payload, string propertyName)
        {
            var obj = AsObjectElement(payload);
            if (obj == null || !obj.Value.TryGetProperty(propertyName, out var property))
                return null;

            if (property.ValueKind == JsonValueKind.String)
                return property.GetString();

            return property.ToString();
        }

        private static bool? TryReadBool(object payload, string propertyName)
        {
            var obj = AsObjectElement(payload);
            if (obj == null || !obj.Value.TryGetProperty(propertyName, out var property))
                return null;

            if (property.ValueKind == JsonValueKind.True)
                return true;

            if (property.ValueKind == JsonValueKind.False)
                return false;

            if (property.ValueKind == JsonValueKind.String && bool.TryParse(property.GetString(), out var parsed))
                return parsed;

            return null;
        }

        private static TimeSpan? TryReadTimeSpan(object payload, string propertyName)
        {
            var raw = TryReadString(payload, propertyName);
            return TimeSpan.TryParse(raw, out var parsed) ? parsed : null;
        }
    }
}
