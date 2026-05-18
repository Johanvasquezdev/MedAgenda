using Serilog.Context;

namespace SGC.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string CorrelationHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers[CorrelationHeader].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId))
                correlationId = Guid.NewGuid().ToString("N");

            context.TraceIdentifier = correlationId;
            context.Response.Headers[CorrelationHeader] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
