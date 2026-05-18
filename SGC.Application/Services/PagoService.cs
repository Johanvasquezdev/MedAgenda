using SGC.Application.Contracts;
using SGC.Application.DTOs.Payments;
using SGC.Application.Mappers;
using SGC.Application.Services.Base;
using SGC.Domain.Entities.Payments;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio de dominio para gestionar pagos relacionados con citas medicas, integrando con un servicio de pagos externo (ej. Stripe) y manejando la persistencia de datos a traves del repositorio de pagos.
    public class PagoService : BaseService, IPagoService
    {
        private readonly IPagoRepository _pagoRepo;
        private readonly IPaymentService _paymentService;

        public PagoService(
            IPagoRepository pagoRepo,
            IPaymentService paymentService,
            ISGCLogger logger) : base(logger)
        {
            _pagoRepo = pagoRepo;
            _paymentService = paymentService;
        }

        // Crea un intento de pago para una cita, generando un PaymentIntent en el servicio de pagos externo y guardando la informacion del pago en la base de datos con estado pendiente.
        public async Task<string> CrearIntentoPagoAsync(
            CrearPagoRequest request)
        {
            return await ExecuteOperacionAsync(
                "CrearIntentoPago",
                async () =>
                {
                    var stripePaymentIntentId = await _paymentService
                        .CrearIntentoPagoAsync(
                            request.Monto,
                            request.Moneda,
                            request.CitaId);

                    var pago = new Pago
                    {
                        CitaId = request.CitaId,
                        PacienteId = request.PacienteId,
                        Monto = request.Monto,
                        Moneda = request.Moneda,
                        StripePaymentIntentId = stripePaymentIntentId
                    };

                    await _pagoRepo.AddAsync(pago);
                    return stripePaymentIntentId;
                },
                $"CitaId: {request.CitaId}, Monto: {request.Monto}");
        }

        public async Task<bool> SimularConfirmacionAsync(int citaId)
        {
            return await ExecuteOperacionAsync(
                "SimularConfirmacion",
                async () =>
                {
                    var pago = await _pagoRepo.GetByCitaAsync(citaId);
                    if (pago != null && pago.Estado == EstadoPago.Pendiente)
                    {
                        pago.Confirmar();
                        await _pagoRepo.UpdateAsync(pago);
                        return true;
                    }
                    return false;
                },
                $"CitaId: {citaId}");
        }

        // Confirma el pago una vez que el cliente ha completado el proceso de pago en el servicio externo, actualizando el estado del pago en la base de datos a confirmado.
        public async Task<bool> ConfirmarPagoAsync(
            string stripePaymentIntentId)
        {
            return await ExecuteOperacionAsync(
                "ConfirmarPago",
                async () =>
                {
                    var confirmado = await _paymentService
                        .ConfirmarPagoAsync(stripePaymentIntentId);

                    if (confirmado)
                    {
                        var pago = await _pagoRepo
                            .GetByStripeIdAsync(stripePaymentIntentId);
                        if (pago != null)
                        {
                            pago.Confirmar();
                            await _pagoRepo.UpdateAsync(pago);
                        }
                    }

                    return confirmado;
                },
                $"StripeId: {stripePaymentIntentId}");
        }

        public async Task<bool> MarcarPagoFallidoAsync(string stripePaymentIntentId)
        {
            return await ExecuteOperacionAsync(
                "MarcarPagoFallido",
                async () =>
                {
                    var pago = await _pagoRepo.GetByStripeIdAsync(stripePaymentIntentId);
                    if (pago == null)
                        return false;

                    if (pago.Estado == EstadoPago.Pendiente)
                    {
                        pago.MarcarFallido();
                        await _pagoRepo.UpdateAsync(pago);
                    }

                    return true;
                },
                $"StripeId: {stripePaymentIntentId}");
        }

        // Reembolsa un pago previamente confirmado, interactuando con el servicio de pagos externo para procesar el reembolso y actualizando el estado del pago en la base de datos a reembolsado.
        public async Task<bool> ReembolsarPagoAsync(int pagoId)
        {
            return await ExecuteOperacionAsync(
                "ReembolsarPago",
                async () =>
                {
                    var pago = await _pagoRepo.GetByIdAsync(pagoId);
                    var reembolsado = await _paymentService
                        .ReembolsarPagoAsync(pago.StripePaymentIntentId!);

                    if (reembolsado)
                    {
                        pago.Reembolsar();
                        await _pagoRepo.UpdateAsync(pago);
                    }

                    return reembolsado;
                },
                $"PagoId: {pagoId}");
        }

        // Obtiene el pago asociado a una cita, devolviendo la informacion del pago si existe o null si no se ha registrado ningun pago para esa cita.
        public async Task<PagoResponse?> GetByCitaAsync(int citaId)
        {
            return await ExecuteOperacionAsync(
                "GetByCita",
                async () =>
                {
                    var pago = await _pagoRepo.GetByCitaAsync(citaId);
                    return pago == null ? null : PagoMapper.ToResponse(pago);
                },
                $"CitaId: {citaId}");
        }

        // Obtiene todos los pagos realizados por un paciente, devolviendo una lista de respuestas con la informacion de cada pago asociado a las citas del paciente.
        public async Task<IEnumerable<PagoResponse>> GetByPacienteAsync(
            int pacienteId)
        {
            return await ExecuteOperacionAsync(
                "GetByPaciente",
                async () =>
                {
                    var pagos = await _pagoRepo.GetByPacienteAsync(pacienteId);
                    return pagos.Select(PagoMapper.ToResponse);
                },
                $"PacienteId: {pacienteId}");
        }
    }
}
