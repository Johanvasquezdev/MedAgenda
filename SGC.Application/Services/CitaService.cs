using SGC.Application.Contracts;
using SGC.Application.DTOs.Appointments;
using SGC.Application.Mappers;
using SGC.Application.Services.Base;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Notifications;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Services;

namespace SGC.Application.Services
{
    // Servicio para gestionar el ciclo de vida de las citas medicas
    public class CitaService : BaseService, ICitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IMedicoRepository _medicoRepository;
        private readonly CitaDomainService _domainService;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IEmailService _emailService;

        public CitaService(
            ICitaRepository citaRepository,
            IMedicoRepository medicoRepository,
            INotificacionRepository notificacionRepository,
            IPacienteRepository pacienteRepository,
            IEmailService emailService,
            CitaDomainService domainService,
            ISGCLogger logger) : base(logger)
        {
            _citaRepository = citaRepository;
            _medicoRepository = medicoRepository;
            _notificacionRepository = notificacionRepository;
            _pacienteRepository = pacienteRepository;
            _emailService = emailService;
            _domainService = domainService;
        }
        // Agenda una nueva cita validando disponibilidad y conflictos de horario
        public async Task<CitaResponse> AgendarAsync(CrearCitaRequest request)
        {
            return await ExecuteOperacionAsync(
                "AgendarCita",
                async () =>
                {
                    var medico = await _medicoRepository
                        .GetByIdWithHorariosAsync(request.MedicoId);

                    var cita = CitaMapper.ToEntity(request);
                    _domainService.ValidarAgendamiento(cita, medico);

                    var existeConflicto = await _citaRepository
                        .ExisteConflictoAsync(request.MedicoId, request.FechaHora);
                    if (existeConflicto)
                    {
                        throw new Domain.Exceptions.CitaConflictoException(
                            "Ya existe una cita programada en ese horario.");
                    }

                    await _citaRepository.AddAsync(cita);
                    return CitaMapper.ToResponse(cita);
                },
                $"Paciente: {request.PacienteId}, Medico: {request.MedicoId}");
        }

        // Obtiene una cita por su identificador
        public async Task<CitaResponse> GetByIdAsync(int id)
        {
            return await ExecuteOperacionAsync(
                "GetCitaById",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(id);
                    return CitaMapper.ToResponse(cita);
                },
                $"CitaId: {id}");
        }

        // Obtiene todas las citas del sistema
        public async Task<IEnumerable<CitaResponse>> GetAllAsync()
        {
            return await ExecuteOperacionAsync(
                "GetAllCitas",
                async () =>
                {
                    var citas = await _citaRepository.GetAllAsync();
                    return citas.Select(CitaMapper.ToResponse);
                });
        }

        // Obtiene todas las citas de un paciente especifico
        public async Task<IEnumerable<CitaResponse>> GetByPacienteAsync(int pacienteId)
        {
            return await ExecuteOperacionAsync(
                "GetCitasByPaciente",
                async () =>
                {
                    var citas = await _citaRepository.GetByPacienteIdAsync(pacienteId);
                    return citas.Select(CitaMapper.ToResponse);
                },
                $"PacienteId: {pacienteId}");
        }

        // Obtiene todas las citas de un medico especifico
        public async Task<IEnumerable<CitaResponse>> GetByMedicoAsync(int medicoId)
        {
            return await ExecuteOperacionAsync(
                "GetCitasByMedico",
                async () =>
                {
                    var citas = await _citaRepository.GetByMedicoIdAsync(medicoId);
                    return citas.Select(CitaMapper.ToResponse);
                },
                $"MedicoId: {medicoId}");
        }

        // Obtiene todas las citas de una fecha especifica
        public async Task<IEnumerable<CitaResponse>> GetByFechaAsync(DateTime fecha)
        {
            return await ExecuteOperacionAsync(
                "GetCitasByFecha",
                async () =>
                {
                    var citas = await _citaRepository.GetByFechaAsync(fecha);
                    return citas.Select(CitaMapper.ToResponse);
                },
                $"Fecha: {fecha:O}");
        }

        // Confirma una cita cambiando su estado a Confirmada
        public async Task ConfirmarAsync(int citaId)
        {
            await ExecuteOperacionAsync(
                "ConfirmarCita",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    cita.Confirmar();
                    await _citaRepository.UpdateAsync(cita);
                    await CrearNotificacionCambioEstadoAsync(
                        cita,
                        "aceptó tu cita");
                },
                $"CitaId: {citaId}");
        }

        // Cancela una cita registrando el motivo
        public async Task CancelarAsync(int citaId, string motivo)
        {
            await ExecuteOperacionAsync(
                "CancelarCita",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    cita.Cancelar(motivo);
                    await _citaRepository.UpdateAsync(cita);
                },
                $"CitaId: {citaId}, Motivo: {motivo}");
        }

        // Cancela una cita por accion del medico y notifica al paciente
        public async Task CancelarPorMedicoAsync(int citaId, string motivo)
        {
            await ExecuteOperacionAsync(
                "CancelarCitaPorMedico",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    cita.Cancelar(motivo);
                    await _citaRepository.UpdateAsync(cita);
                    await CrearNotificacionCambioEstadoAsync(
                        cita,
                        "canceló tu cita");
                },
                $"CitaId: {citaId}, Motivo: {motivo}");
        }

        // Rechaza una cita solicitada registrando el motivo
        public async Task RechazarAsync(int citaId, string motivo)
        {
            await ExecuteOperacionAsync(
                "RechazarCita",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    cita.Rechazar(motivo);
                    await _citaRepository.UpdateAsync(cita);
                },
                $"CitaId: {citaId}, Motivo: {motivo}");
        }

        // Reprograma una cita a una nueva fecha validando disponibilidad
        public async Task ReprogramarAsync(int citaId, DateTime nuevaFecha)
        {
            await ExecuteOperacionAsync(
                "ReprogramarCita",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    var medico = await _medicoRepository
                        .GetByIdWithHorariosAsync(cita.MedicoId);

                    _domainService.ValidarReprogramacion(cita, medico, nuevaFecha);

                    var existeConflicto = await _citaRepository
                        .ExisteConflictoAsync(cita.MedicoId, nuevaFecha);
                    if (existeConflicto)
                    {
                        throw new Domain.Exceptions.CitaConflictoException(
                            "Ya existe una cita en ese horario.");
                    }

                    var fechaAnterior = cita.FechaHora;
                    cita.Reprogramar(nuevaFecha);
                    await _citaRepository.UpdateAsync(cita);

                    // Notificar al paciente del cambio de horario via email (fire-and-forget)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var paciente = await _pacienteRepository.GetByIdAsync(cita.PacienteId);
                            var medico = await _medicoRepository.GetByIdAsync(cita.MedicoId);
                            await _emailService.EnviarCambioHorarioAsync(
                                paciente.Email, paciente.Nombre,
                                medico?.Nombre ?? "Tu médico",
                                fechaAnterior, nuevaFecha);
                        }
                        catch { /* No bloquear si el email falla */ }
                    });
                },
                $"CitaId: {citaId}, NuevaFecha: {nuevaFecha:O}");
        }

        // Marca una cita como no asistida por el paciente
        public async Task MarcarNoAsistioAsync(int citaId)
        {
            await ExecuteOperacionAsync(
                "MarcarNoAsistio",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    cita.MarcarNoAsistio();
                    await _citaRepository.UpdateAsync(cita);
                    await CrearNotificacionCambioEstadoAsync(
                        cita,
                        "te marcó como no asistido");
                },
                $"CitaId: {citaId}");
        }

        // Inicia la consulta medica de una cita confirmada
        public async Task IniciarConsultaAsync(int citaId)
        {
            await ExecuteOperacionAsync(
                "IniciarConsulta",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    cita.IniciarConsulta();
                    await _citaRepository.UpdateAsync(cita);
                    await CrearNotificacionCambioEstadoAsync(
                        cita,
                        "inició tu cita");
                },
                $"CitaId: {citaId}");
        }

        // Completa una cita que esta en progreso
        public async Task CompletarAsync(int citaId)
        {
            await ExecuteOperacionAsync(
                "CompletarCita",
                async () =>
                {
                    var cita = await _citaRepository.GetByIdAsync(citaId);
                    cita.Completar();
                    await _citaRepository.UpdateAsync(cita);
                    await CrearNotificacionCambioEstadoAsync(
                        cita,
                        "completó tu cita");
                },
                $"CitaId: {citaId}");
        }

        // Crea una notificacion persistente para el paciente con el nombre del medico
        private async Task CrearNotificacionCambioEstadoAsync(
            Cita cita,
            string accion)
        {
            var medico = await _medicoRepository.GetByIdAsync(cita.MedicoId);
            var nombreMedico = medico == null || string.IsNullOrWhiteSpace(medico.Nombre)
                ? "Tu médico"
                : medico.Nombre.Trim();

            var notificacion = new Notificacion
            {
                UsuarioId = cita.PacienteId,
                CitaId = cita.Id,
                Tipo = TipoNotificacion.Push,
                Mensaje = $"{nombreMedico} {accion}",
                Leida = false,
                FechaEnvio = DateTime.UtcNow
            };

            await _notificacionRepository.AddAsync(notificacion);
        }
    }
}
