namespace SGC.Application.DTOs.Notifications
{
    // Datos de respuesta de una notificacion del sistema
    public class NotificacionResponse
    {
        // Identificador unico de la notificacion
        public int Id { get; set; }

        // Identificador del usuario destinatario
        public int UsuarioId { get; set; }

        // Identificador de la cita asociada (si aplica)
        public int? CitaId { get; set; }

        // Tipo de notificacion (Email, SMS, Push)
        public string Tipo { get; set; } = string.Empty;

        // Contenido del mensaje de la notificacion
        public string Mensaje { get; set; } = string.Empty;

        // Indica si la notificacion fue leida
        public bool Leida { get; set; }

        // Fecha y hora de envio
        public DateTime FechaEnvio { get; set; }
    }
}
