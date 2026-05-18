using System.Net;
using System.Text.Json;
using SGC.Domain.Exceptions;

namespace SGC.API.Middleware
{
    // Middleware global para capturar excepciones y devolver respuestas HTTP consistentes
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        // Mapea cada tipo de excepcion a su codigo HTTP correspondiente
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            var (statusCode, code, message) = ResolveError(exception);

            // Solo registrar errores internos como Error; los demas como Warning
            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Error interno. TraceId: {TraceId}", traceId);
            else
                _logger.LogWarning(
                    "Excepcion controlada ({StatusCode}/{Code}). TraceId: {TraceId}. Mensaje: {Mensaje}",
                    (int)statusCode,
                    code,
                    traceId,
                    message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var respuesta = new
            {
                status = (int)statusCode,
                code,
                message,
                traceId
            };

            var json = JsonSerializer.Serialize(respuesta, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private static (HttpStatusCode StatusCode, string Code, string Message) ResolveError(Exception exception)
        {
            return exception switch
            {
                DomainException ex when ex is CitaConflictoException or HorarioNoDisponibleException
                    => (HttpStatusCode.Conflict, ex.Code, ex.Message),

                NotFoundDomainException ex
                    => (HttpStatusCode.NotFound, ex.Code, ex.Message),

                DomainException ex when ex is ValidationDomainException or CitaNotFoundException
                    => (HttpStatusCode.BadRequest, ex.Code, ex.Message),

                DomainException ex when ex is InfrastructureException
                    => (HttpStatusCode.InternalServerError, ex.Code, "Ocurrio un error interno en el servidor."),

                DomainException ex
                    => (HttpStatusCode.BadRequest, ex.Code, ex.Message),

                KeyNotFoundException ex
                    => (HttpStatusCode.NotFound, "not_found", ex.Message),

                InvalidOperationException ex
                    => (HttpStatusCode.BadRequest, "invalid_operation", ex.Message),

                ArgumentException ex
                    => (HttpStatusCode.BadRequest, "validation_error", ex.Message),

                UnauthorizedAccessException ex
                    => (HttpStatusCode.Unauthorized, "unauthorized", ex.Message),

                _
                    => (HttpStatusCode.InternalServerError, "internal_error", "Ocurrio un error interno en el servidor.")
            };
        }
    }
}
