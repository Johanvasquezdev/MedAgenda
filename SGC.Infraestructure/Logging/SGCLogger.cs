using Microsoft.Extensions.Logging;
using SGC.Domain.Interfaces.ILogger;

namespace SGC.Infraestructure.Logging
{
    // Implementación concreta del logger usando el sistema de logging de .NET.
    public class SGCLogger : ISGCLogger
    {
        private readonly ILogger<SGCLogger> _logger;

        public SGCLogger(ILogger<SGCLogger> logger)
        {
            _logger = logger;
        }

        // Registra mensajes informativos.
        public void LogInfo(string mensaje) =>
            _logger.LogInformation(mensaje);

        // Registra advertencias.
        public void LogWarning(string mensaje) =>
            _logger.LogWarning(mensaje);

        // Registra errores y su excepción asociada si existe.
        public void LogError(string mensaje, Exception? ex = null) =>
            _logger.LogError(ex, mensaje);

        // Registra mensajes de depuración.
        public void LogDebug(string mensaje) =>
            _logger.LogDebug(mensaje);
    }
}
