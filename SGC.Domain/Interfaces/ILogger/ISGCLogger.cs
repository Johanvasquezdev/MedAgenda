namespace SGC.Domain.Interfaces.ILogger
{
    public interface ISGCLogger // Interfaz de logging para el Sistema de Gestión de Citas Médicas .
    {
        void LogInfo(string mensaje); // Método para registrar mensajes informativos, como la creación de una nueva cita o la actualización de un perfil de usuario. Estos mensajes ayudan a rastrear el flujo normal de la aplicación y a entender qué acciones se han realizado.
        void LogWarning(string mensaje); // Método para registrar advertencias, como intentos de acceso no autorizados o datos ingresados incorrectamente. Estos mensajes son útiles para identificar posibles problemas o comportamientos inusuales que no necesariamente son errores, pero que podrían requerir atención.
        void LogError(string mensaje, Exception? ex = null); // Método para registrar errores, como fallos en la conexión a la base de datos o excepciones no manejadas. Este método puede incluir un parámetro opcional para capturar detalles de la excepción, lo que facilita la depuración y el análisis de problemas críticos en el sistema.
        void LogDebug(string mensaje); // Método para registrar mensajes de depuración, que son útiles durante el desarrollo y la resolución de problemas. Estos mensajes pueden incluir información detallada sobre el estado interno de la aplicación, como valores de variables o resultados de operaciones específicas, lo que ayuda a los desarrolladores a entender el comportamiento del sistema en situaciones complejas.
    }
}