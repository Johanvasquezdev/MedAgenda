using SGC.Domain.Base;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Security;
using SGC.Domain.Enums;

namespace SGC.Domain.Entities.Notifications
{
    // Notificacion enviada a un usuario del sistema
    public sealed class Notificacion : EntidadBase
    {
        public int UsuarioId { get; set; }
        public int? CitaId { get; set; }
        public TipoNotificacion Tipo { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool Leida { get; set; } = false;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

        // Propiedades de navegacion
        public Usuario Usuario { get; set; } = null!;
        public Cita? Cita { get; set; }

        // Marcar notificacion como leida
        public void MarcarLeida()
        {
            if (Leida)
                throw new InvalidOperationException(
                    "La notificacion ya fue marcada como leida.");
            Leida = true;
        }
    }
}
