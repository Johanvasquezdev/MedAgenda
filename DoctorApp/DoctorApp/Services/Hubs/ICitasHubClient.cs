namespace DoctorApp.Services.Hubs
{
    /// <summary>
    /// Interfaz para el cliente SignalR del hub de citas
    /// Maneja conexión a tiempo real y eventos de citas
    /// </summary>
    public interface ICitasHubClient
    {
        /// <summary>
        /// Evento que se dispara cuando se crea una nueva cita
        /// </summary>
        event EventHandler<CitaHubEventArgs>? OnNuevaEnCita;

        /// <summary>
        /// Evento que se dispara cuando se actualiza una cita
        /// </summary>
        event EventHandler<CitaHubEventArgs>? OnCitaActualizada;

        /// <summary>
        /// Evento que se dispara cuando se confirma una cita
        /// </summary>
        event EventHandler<CitaHubEventArgs>? OnCitaConfirmada;

        /// <summary>
        /// Conecta al hub de citas usando el token JWT
        /// </summary>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Desconecta del hub de citas
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Verifica si está conectado al hub
        /// </summary>
        bool IsConnected { get; }
    }

    /// <summary>
    /// Argumentos de evento para eventos del hub de citas
    /// </summary>
    public class CitaHubEventArgs : EventArgs
    {
        public int CitaId { get; set; }
        public DateTime FechaHora { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
