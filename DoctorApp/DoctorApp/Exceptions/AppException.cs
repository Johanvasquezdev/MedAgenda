namespace DoctorApp.Exceptions;

/// <summary>
/// Excepción base para la aplicación
/// </summary>
public class AppException : Exception
{
    public string? Code { get; set; }
    public int? HttpStatusCode { get; set; }

    public AppException(string message, string? code = null, int? httpStatusCode = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        HttpStatusCode = httpStatusCode;
    }
}

/// <summary>
/// Excepción de autenticación (401)
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "No autorizado. Verifica tu token.", Exception? innerException = null)
        : base(message, "UNAUTHORIZED", 401, innerException) { }
}

/// <summary>
/// Excepción de conflicto (409) - ej: horario ya existe
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message = "Conflicto en la operación.", Exception? innerException = null)
        : base(message, "CONFLICT", 409, innerException) { }
}

/// <summary>
/// Excepción de validación
/// </summary>
public class ValidationException : AppException
{
    public Dictionary<string, string[]> Errors { get; set; }

    public ValidationException(string message = "Error de validación.", Dictionary<string, string[]>? errors = null, Exception? innerException = null)
        : base(message, "VALIDATION_ERROR", 400, innerException)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}

/// <summary>
/// Excepción cuando el token expira
/// </summary>
public class TokenExpiredException : AppException
{
    public TokenExpiredException(string message = "Tu sesión ha expirado. Por favor inicia sesión nuevamente.", Exception? innerException = null)
        : base(message, "TOKEN_EXPIRED", 401, innerException) { }
}

/// <summary>
/// Excepción de error de conexión
/// </summary>
public class ConnectionException : AppException
{
    public ConnectionException(string message = "Error de conexión con el servidor.", Exception? innerException = null)
        : base(message, "CONNECTION_ERROR", 0, innerException) { }
}
