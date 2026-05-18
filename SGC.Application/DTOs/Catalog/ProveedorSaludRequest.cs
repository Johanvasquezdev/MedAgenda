namespace SGC.Application.DTOs.Catalog
{
    // Datos para crear o modificar un proveedor de salud
    public class ProveedorSaludRequest
    {
        // Nombre del proveedor (hospital, clinica, laboratorio)
        public string Nombre { get; set; } = string.Empty;

        // Tipo de proveedor (hospital, clinica, laboratorio, etc.)
        public string? Tipo { get; set; }

        // Numero de telefono del proveedor
        public string? Telefono { get; set; }

        // Correo electronico del proveedor
        public string? Email { get; set; }
    }
}
