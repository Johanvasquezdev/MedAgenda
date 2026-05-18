namespace SGC.Domain.Exceptions
{
    public sealed class
        HorarioNoDisponibleException : DomainException // para indicar que el médico no tiene disponibilidad para la fecha y hora solicitada
    {

        // el constructor de la excepción recibe el ID del médico y la fecha y hora para la cual se intentó programar la cita, y construye un mensaje de error informativo que se puede usar para depurar o mostrar al usuario
        public HorarioNoDisponibleException(int medicoId, DateTime fechaHora)
            : base("horario_no_disponible", $"El médico {medicoId} no tiene disponibilidad para {fechaHora}.") { }
    }
}
