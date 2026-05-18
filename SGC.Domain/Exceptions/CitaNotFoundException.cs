namespace SGC.Domain.Exceptions
{
    public sealed class CitaNotFoundException : DomainException // para indicar que no se encontró una cita con el Id especificado
    {

        // el constructor de la excepción recibe el ID de la cita que no se encontró, y construye un mensaje de error informativo que se puede usar para depurar o mostrar al usuario
        public CitaNotFoundException(int id)
            : base("cita_not_found", $"La cita con Id {id} no fue encontrada.") { }
    }
}
