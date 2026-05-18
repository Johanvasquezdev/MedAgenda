using SGC.Domain.Entities.Notifications;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio para la creacion de una notificacion
    public class NotificacionValidator
    {
        public void Validar(Notificacion notificacion)
        {
            // Regla: debe tener un usuario destinatario
            if (notificacion.UsuarioId <= 0)
                throw new InvalidOperationException(
                    "La notificacion debe tener un usuario destinatario.");

            // Regla: el mensaje es obligatorio
            if (string.IsNullOrWhiteSpace(notificacion.Mensaje))
                throw new InvalidOperationException(
                    "El mensaje de la notificacion es obligatorio.");
        }
    }
}
