namespace SGC.Domain.Exceptions
{
    public sealed class CitaConflictoException : DomainException // para indicar que hay un conflicto de citas (ej: un médico ya tiene una cita programada en ese horario) 
    {

        // el constructor de la excepción recibe un mensaje personalizado que describe el conflicto específico, lo que permite una mayor flexibilidad para manejar diferentes tipos de conflictos de citas en el sistema
        public CitaConflictoException(string mensaje)
            : base("cita_conflicto", mensaje) { }
    }
}
