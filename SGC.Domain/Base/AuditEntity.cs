using SGC.Domain.Entities.Security;

namespace SGC.Domain.Base
{
    // Entidad de auditoria para registrar acciones realizadas en el sistema
    public class AuditEntity : EntidadBase
    {
        public int? UsuarioId { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string? DireccionIP { get; set; }

        // Navegacion al usuario que realizo la accion (null si fue el sistema)
        public Usuario? Usuario { get; set; }
    }
}
