using SGC.Domain.Entities.Notifications;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio para las preferencias de notificacion de un usuario
    public class PrefNotificacionValidator
    {
        public void Validar(PrefNotificacion pref)
        {
            // Regla: debe tener un usuario asociado
            if (pref.UsuarioId <= 0)
                throw new InvalidOperationException(
                    "Las preferencias de notificacion deben estar asociadas a un usuario.");

            // Regla: las horas antes del recordatorio deben ser positivas
            if (pref.HorasAntesRecordatorio <= 0)
                throw new InvalidOperationException(
                    "Las horas antes del recordatorio deben ser mayores a 0.");

            // Regla: las horas antes del recordatorio no deben exceder 72 horas
            if (pref.HorasAntesRecordatorio > 72)
                throw new InvalidOperationException(
                    "Las horas antes del recordatorio no pueden exceder 72 horas.");
        }
    }
}
