namespace SGC.Domain.Exceptions
{
    public sealed class ValidationDomainException : DomainException
    {
        public ValidationDomainException(string message, Exception? innerException = null)
            : base("validation_error", message, innerException)
        {
        }
    }
}
