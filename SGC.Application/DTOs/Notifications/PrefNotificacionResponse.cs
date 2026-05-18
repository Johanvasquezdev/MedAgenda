namespace SGC.Application.DTOs.Notifications
{
    // Datos de respuesta con las preferencias de notificacion de un usuario
    public class PrefNotificacionResponse
    {
        // Identificador unico de la preferencia
        public int Id { get; set; }

        // Identificador del usuario
        public int UsuarioId { get; set; }

        // Indica si recibe notificaciones por email
        public bool RecibirEmail { get; set; }

        // Indica si recibe notificaciones por SMS
        public bool RecibirSMS { get; set; }

        // Indica si recibe notificaciones push
        public bool RecibirPush { get; set; }

        // Horas antes de la cita para el recordatorio
        public int HorasAntesRecordatorio { get; set; }
    }
}
