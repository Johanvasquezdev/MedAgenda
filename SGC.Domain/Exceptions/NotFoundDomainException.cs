namespace SGC.Domain.Exceptions
{
    public sealed class NotFoundDomainException : DomainException
    {
        public NotFoundDomainException(string message, Exception? innerException = null)
            : base("not_found", message, innerException)
        {
        }
    }
}
