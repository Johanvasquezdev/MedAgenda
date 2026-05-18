namespace SGC.Application.DTOs.Catalog
{
    // Datos de respuesta con la informacion de un proveedor de salud
    public class ProveedorSaludResponse
    {
        // Identificador unico del proveedor
        public int Id { get; set; }

        // Nombre del proveedor
        public string Nombre { get; set; } = string.Empty;

        // Tipo de proveedor
        public string? Tipo { get; set; }

        // Numero de telefono
        public string? Telefono { get; set; }

        // Correo electronico
        public string? Email { get; set; }

        // Indica si el proveedor esta activo
        public bool Activo { get; set; }
    }
}
