namespace SGC.Application.DTOs.Notifications
{
    // Datos para actualizar las preferencias de notificacion de un usuario
    public class PrefNotificacionRequest
    {
        // Identificador del usuario al que pertenecen las preferencias
        public int UsuarioId { get; set; }

        // Indica si desea recibir notificaciones por email
        public bool RecibirEmail { get; set; } = true;

        // Indica si desea recibir notificaciones por SMS
        public bool RecibirSMS { get; set; } = true;

        // Indica si desea recibir notificaciones push
        public bool RecibirPush { get; set; } = true;

        // Horas antes de la cita para enviar recordatorio
        public int HorasAntesRecordatorio { get; set; } = 24;
    }
}
