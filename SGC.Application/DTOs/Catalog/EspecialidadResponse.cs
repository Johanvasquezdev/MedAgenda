namespace SGC.Application.DTOs.Catalog
{
    // Datos de respuesta con la informacion de una especialidad medica
    public class EspecialidadResponse
    {
        // Identificador unico de la especialidad
        public int Id { get; set; }

        // Nombre de la especialidad
        public string Nombre { get; set; } = string.Empty;

        // Descripcion de la especialidad
        public string? Descripcion { get; set; }

        // Indica si la especialidad esta activa
        public bool Activo { get; set; }
    }
}
