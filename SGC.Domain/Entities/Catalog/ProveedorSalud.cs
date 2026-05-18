using SGC.Domain.Base;

namespace SGC.Domain.Entities.Catalog
{

    // Proveedor de salud (hospital, clinica, laboratorio). Sellada para evitar herencia.
    public sealed class ProveedorSalud : EntidadBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Activo { get; set; } = true;

        // Navegacion inversa: todos los medicos asociados a este proveedor
        public ICollection<Medical.Medico> Medicos { get; set; } = new List<Medical.Medico>();
    }
}