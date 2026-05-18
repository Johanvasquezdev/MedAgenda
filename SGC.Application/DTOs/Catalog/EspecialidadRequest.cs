namespace SGC.Application.DTOs.Catalog
{
    // Datos para crear o modificar una especialidad medica
    public class EspecialidadRequest
    {
        // Nombre de la especialidad (ej: Cardiologia, Dermatologia)
        public string Nombre { get; set; } = string.Empty;

        // Descripcion de la especialidad
        public string? Descripcion { get; set; }
    }
}
