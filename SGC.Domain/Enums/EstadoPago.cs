namespace SGC.Domain.Enums
{
    // Enum para representar el estado de un pago asociado a una cita médica
    public enum EstadoPago
    {
        Pendiente,
        Completado,
        Fallido,
        Reembolsado,
        Cancelado
    }
}