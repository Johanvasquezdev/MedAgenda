using SGC.Domain.Base;
using SGC.Domain.Entities.Notifications;
using SGC.Domain.Enums;

namespace SGC.Domain.Entities.Security
{
    // Usuario base abstracto del sistema (TPT: Medico, Paciente, Administrador heredan de aqui)
    public abstract class Usuario : EntidadBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool Activo { get; set; } = true;

        // Navegacion inversa
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public PrefNotificacion? PrefNotificacion { get; set; }

        // Reglas de negocio
        public void Desactivar()
        {
            if (!Activo)
                throw new InvalidOperationException("El usuario ya esta desactivado.");
            Activo = false;
        }

        public void Activar()
        {
            if (Activo)
                throw new InvalidOperationException("El usuario ya esta activo.");
            Activo = true;
        }
    }
}
