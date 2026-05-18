namespace DoctorApp.Services.Hubs
{
    /// <summary>
    /// Interfaz para el cliente SignalR del hub de disponibilidad
    /// Maneja conexión a tiempo real y eventos de disponibilidad
    /// </summary>
    public interface IDisponibilidadHubClient
    {
        /// <summary>
        /// Evento que se dispara cuando se crea una nueva disponibilidad
        /// </summary>
        event EventHandler<DisponibilidadHubEventArgs>? OnDisponibilidadCreada;

        /// <summary>
        /// Evento que se dispara cuando se actualiza una disponibilidad
        /// </summary>
        event EventHandler<DisponibilidadHubEventArgs>? OnDisponibilidadActualizada;

        /// <summary>
        /// Evento que se dispara cuando se elimina una disponibilidad
        /// </summary>
        event EventHandler<DisponibilidadHubEventArgs>? OnDisponibilidadEliminada;

        /// <summary>
        /// Conecta al hub de disponibilidad usando el token JWT
        /// </summary>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Desconecta del hub de disponibilidad
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Verifica si está conectado al hub
        /// </summary>
        bool IsConnected { get; }
    }

    /// <summary>
    /// Argumentos de evento para eventos del hub de disponibilidad
    /// </summary>
    public class DisponibilidadHubEventArgs : EventArgs
    {
        public int DisponibilidadId { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int DuracionMinutos { get; set; }
        public bool Activo { get; set; }
        public int CitasAsignadas { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
