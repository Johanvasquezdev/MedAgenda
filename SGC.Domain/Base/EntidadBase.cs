namespace SGC.Domain.Base
{

    // Clase base para todas las entidades del sistema, proporcionando la propiedad Id comun. FechaCreacion se agrega solo en las entidades cuyas tablas en la base de datos tienen esa columna (USUARIO, CITA).
    public abstract class EntidadBase
    {
        public int Id { get; set; } // Identificador unico para cada entidad

    }
}