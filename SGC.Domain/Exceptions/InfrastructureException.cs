namespace SGC.Domain.Exceptions
{
    public sealed class InfrastructureException : DomainException
    {
        public InfrastructureException(string message, Exception? innerException = null)
            : base("infrastructure_error", message, innerException)
        {
        }
    }
}
