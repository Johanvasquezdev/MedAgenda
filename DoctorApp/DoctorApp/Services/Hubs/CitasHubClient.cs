using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.Text.Json;
using DoctorApp.Security;
using DoctorApp.Exceptions;

namespace DoctorApp.Services.Hubs
{
    /// <summary>
    /// Cliente SignalR para el hub de citas
    /// Administra conexión a tiempo real con backend para eventos de citas
    /// </summary>
    public class CitasHubClient : ICitasHubClient
    {
        private HubConnection? _hubConnection;
        private readonly ITokenManager _tokenManager;
        private readonly string _hubEndpoint;

        public event EventHandler<CitaHubEventArgs>? OnNuevaEnCita;
        public event EventHandler<CitaHubEventArgs>? OnCitaActualizada;
        public event EventHandler<CitaHubEventArgs>? OnCitaConfirmada;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public CitasHubClient(ITokenManager tokenManager)
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

            return $"{apiBaseUrl.TrimEnd('/')}/citahub";
        }

        /// <summary>
        /// Conecta al hub de citas de forma asincrónica
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
                _hubConnection.On<object>("NuevaCita",
                    payload =>
                    {
                        var citaId = TryReadInt(payload, "id") ?? TryReadInt(payload, "Id") ?? 0;
                        var fecha = TryReadDateTime(payload, "fechaHora") ?? TryReadDateTime(payload, "FechaHora");
                        var pacienteNombre = TryReadString(payload, "pacienteNombre") ?? TryReadString(payload, "PacienteNombre") ?? string.Empty;

                        OnNuevaEnCita?.Invoke(this, new CitaHubEventArgs
                        {
                            CitaId = citaId,
                            PacienteNombre = pacienteNombre,
                            Estado = "Nueva",
                            FechaHora = fecha ?? DateTime.UtcNow
                        });
                    });

                _hubConnection.On<object>("EstadoCitaActualizado",
                    payload =>
                    {
                        var citaId = TryReadInt(payload, "id") ?? TryReadInt(payload, "Id") ?? 0;
                        var estado = TryReadString(payload, "estado") ?? TryReadString(payload, "Estado") ?? "Actualizada";

                        OnCitaActualizada?.Invoke(this, new CitaHubEventArgs
                        {
                            CitaId = citaId,
                            Estado = estado
                        });

                        if (string.Equals(estado, "Confirmada", StringComparison.OrdinalIgnoreCase))
                        {
                            OnCitaConfirmada?.Invoke(this, new CitaHubEventArgs
                            {
                                CitaId = citaId,
                                Estado = "Confirmada"
                            });
                        }
                    });

                // Handlers de reconexión
                _hubConnection.Reconnecting += error =>
                {
                    Debug.WriteLine($"[CitasHubClient] Reconnecting: {error?.Message}");
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnected += connectionId =>
                {
                    Debug.WriteLine($"[CitasHubClient] Reconnected: {connectionId}");
                    return Task.CompletedTask;
                };

                _hubConnection.Closed += error =>
                {
                    Debug.WriteLine($"[CitasHubClient] Connection closed: {error?.Message}");
                    return Task.CompletedTask;
                };

                // Conectar al hub
                await _hubConnection.StartAsync(cancellationToken);
                Debug.WriteLine("[CitasHubClient] Connected successfully");
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConnectionException($"Error al conectar al hub de citas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Desconecta del hub de citas
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
                Debug.WriteLine($"[CitasHubClient] Error during disconnect: {ex.Message}");
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

        private static DateTime? TryReadDateTime(object payload, string propertyName)
        {
            var raw = TryReadString(payload, propertyName);
            return DateTime.TryParse(raw, out var parsed) ? parsed : null;
        }
    }
}
