
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Exceptions;
using System;

namespace SGC.Application.Services.Base
{
    // Clase base para todos los servicios de la aplicacion, proporcionando acceso al logger y metodos comunes de log.
    public abstract class BaseService
    {
        protected readonly ISGCLogger Logger;

        protected BaseService(ISGCLogger logger)
        {
            Logger = logger;
        }

        // Metodos de log comunes para las operaciones de los servicios, que incluyen el nombre del servicio en el mensaje para facilitar la identificacion en los logs.
        protected void LogOperacion(string operacion, string detalle) =>
            Logger.LogInfo($"[{GetType().Name}] {operacion}: {detalle}");

        protected void LogAdvertencia(string operacion, string detalle) =>
            Logger.LogWarning($"[{GetType().Name}] {operacion}: {detalle}");

        protected void LogError(string operacion, Exception ex) =>
            Logger.LogError($"[{GetType().Name}] Error en {operacion}", ex);

        protected async Task<T> ExecuteOperacionAsync<T>(
            string operacion,
            Func<Task<T>> action,
            string? detalle = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(detalle))
                {
                    LogOperacion(operacion, detalle);
                }

                return await action();
            }
            catch (DomainException ex)
            {
                LogAdvertencia(operacion, ex.Message);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                LogAdvertencia(operacion, ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                LogError(operacion, ex);
                throw new ValidationDomainException(ex.Message, ex);
            }
            catch (KeyNotFoundException ex)
            {
                LogError(operacion, ex);
                throw new NotFoundDomainException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                LogError(operacion, ex);
                throw new InfrastructureException("Ocurrió un error interno al procesar la operación.", ex);
            }
        }

        protected async Task ExecuteOperacionAsync(
            string operacion,
            Func<Task> action,
            string? detalle = null)
        {
            await ExecuteOperacionAsync<object?>(
                operacion,
                async () =>
                {
                    await action();
                    return null;
                },
                detalle);
        }
    }
}
