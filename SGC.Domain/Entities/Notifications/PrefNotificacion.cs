using SGC.Domain.Base;
using SGC.Domain.Entities.Security;

namespace SGC.Domain.Entities.Notifications
{
    // Preferencias de notificacion de un usuario
    public sealed class PrefNotificacion : EntidadBase
    {
        public int UsuarioId { get; set; }
        public bool RecibirEmail { get; set; } = true;
        public bool RecibirSMS { get; set; } = true;
        public bool RecibirPush { get; set; } = true;
        public int HorasAntesRecordatorio { get; set; } = 24;

        // Navegacion al usuario
        public Usuario Usuario { get; set; } = null!;

        // Verifica si el usuario tiene al menos un canal de notificacion activo
        public bool TieneAlgunCanalActivo()
        {
            return RecibirEmail || RecibirSMS || RecibirPush;
        }
    }
}
