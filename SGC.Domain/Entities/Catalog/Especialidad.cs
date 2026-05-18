using SGC.Domain.Base;

namespace SGC.Domain.Entities.Catalog
{
    // Especialidad medica (ej. Cardiologia, Dermatologia). Sellada para evitar herencia.
    public sealed class Especialidad : EntidadBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        // Navegacion inversa: todos los medicos con esta especialidad
        public ICollection<Medical.Medico> Medicos { get; set; } = new List<Medical.Medico>();
    }
}