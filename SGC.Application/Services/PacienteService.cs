using BCrypt.Net;
using SGC.Application.Contracts;
using SGC.Application.DTOs.Medical;
using SGC.Application.Mappers;
using SGC.Application.Services.Base;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio de aplicacion para gestionar pacientes, implementa la logica de negocio y validaciones antes de acceder al repositorio.
    public class PacienteService : BaseService, IPacienteService
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly PacienteValidator _validator;
        private readonly IEmailService _emailService;

        public PacienteService(
            IPacienteRepository pacienteRepository,
            PacienteValidator validator,
            IEmailService emailService,
            ISGCLogger logger) : base(logger)
        {
            _pacienteRepository = pacienteRepository;
            _validator = validator;
            _emailService = emailService;
        }

        // Crea un nuevo paciente, validando los datos y aplicando la logica de negocio antes de guardarlo en el repositorio.
        public async Task<PacienteResponse> CrearAsync(CrearPacienteRequest request)
        {
            return await ExecuteOperacionAsync(
                "CrearPaciente",
                async () =>
                {
                    var paciente = new Paciente
                    {
                        Nombre = request.Nombre,
                        Email = request.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                        Rol = RolUsuario.Paciente,
                        Cedula = request.Cedula,
                        Telefono = request.Telefono,
                        FechaNacimiento = request.FechaNacimiento,
                        TipoSeguro = request.TipoSeguro,
                        NumeroSeguro = request.NumeroSeguro
                    };

                    _validator.Validar(paciente);
                    await _pacienteRepository.AddAsync(paciente);

                    // Enviar email de bienvenida via Brevo (fire-and-forget, no bloquea el registro)
                    _ = Task.Run(async () =>
                    {
                        try { await _emailService.EnviarBienvenidaAsync(paciente.Email, paciente.Nombre); }
                        catch { /* No bloquear el registro si el email falla */ }
                    });

                    return PacienteMapper.ToResponse(paciente);
                },
                $"Email: {request.Email}");
        }

        // Obtiene un paciente por su ID, lanzando una excepcion si no se encuentra o esta desactivado.
        public async Task<PacienteResponse> GetByIdAsync(int id)
        {
            return await ExecuteOperacionAsync(
                "GetPacienteById",
                async () =>
                {
                    var paciente = await _pacienteRepository.GetByIdAsync(id);
                    return PacienteMapper.ToResponse(paciente);
                },
                $"PacienteId: {id}");
        }

        // Obtiene todos los pacientes, incluyendo solo los activos y mapeando a DTOs de respuesta.
        public async Task<IEnumerable<PacienteResponse>> GetAllAsync()
        {
            return await ExecuteOperacionAsync(
                "GetAllPacientes",
                async () =>
                {
                    var pacientes = await _pacienteRepository.GetAllAsync();
                    return pacientes.Select(PacienteMapper.ToResponse);
                });
        }

        // Obtiene un paciente por su cedula, lanzando una excepcion si no se encuentra o esta desactivado.
        public async Task<PacienteResponse> GetByCedulaAsync(string cedula)
        {
            return await ExecuteOperacionAsync(
                "GetPacienteByCedula",
                async () =>
                {
                    var paciente = await _pacienteRepository.GetByCedulaAsync(cedula);
                    return PacienteMapper.ToResponse(paciente);
                },
                $"Cedula: {cedula}");
        }

        // Actualiza los datos de un paciente existente, validando los cambios y aplicando la logica de negocio antes de guardarlo en el repositorio.
        public async Task ActualizarAsync(ActualizarPacienteRequest request)
        {
            await ExecuteOperacionAsync(
                "ActualizarPaciente",
                async () =>
                {
                    var paciente = await _pacienteRepository.GetByIdAsync(request.Id);
                    PacienteMapper.UpdateEntity(paciente, request);
                    _validator.Validar(paciente);
                    await _pacienteRepository.UpdateAsync(paciente);
                },
                $"Id: {request.Id}");
        }

        // Desactiva un paciente, aplicando la logica de negocio antes de guardarlo en el repositorio.
        public async Task DesactivarAsync(int id)
        {
            await ExecuteOperacionAsync(
                "DesactivarPaciente",
                async () =>
                {
                    var paciente = await _pacienteRepository.GetByIdAsync(id);
                    paciente.Desactivar();
                    await _pacienteRepository.UpdateAsync(paciente);
                },
                $"Id: {id}");
        }

        // Activa un paciente, aplicando la logica de negocio antes de guardarlo en el repositorio.
        public async Task ActivarAsync(int id)
        {
            await ExecuteOperacionAsync(
                "ActivarPaciente",
                async () =>
                {
                    var paciente = await _pacienteRepository.GetByIdAsync(id);
                    paciente.Activar();
                    await _pacienteRepository.UpdateAsync(paciente);
                },
                $"Id: {id}");
        }
    }
}
