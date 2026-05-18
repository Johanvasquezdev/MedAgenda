using SGC.Application.Contracts;
using SGC.Application.DTOs.Security;
using SGC.Application.Services.Base;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGC.Application.Services
{
    // Servicio para gestionar usuarios del sistema
    public class UsuarioService : BaseService, IUsuarioService
    {
        // Repositorio para acceso a datos de usuarios
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IMedicoRepository _medicoRepository;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IPacienteRepository pacienteRepository,
            IMedicoRepository medicoRepository,
            ISGCLogger logger) : base(logger)
        {
            _usuarioRepository = usuarioRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
        }

        // Obtiene un usuario por su ID
        public async Task<UsuarioResponse> GetByIdAsync(int id)
        {
            return await ExecuteOperacionAsync(
                "GetUsuarioById",
                async () =>
                {
                    var usuario = await _usuarioRepository.GetByIdAsync(id);
                    return MapToResponse(usuario);
                },
                $"UsuarioId: {id}");
        }

        // Obtiene todos los usuarios del sistema
        public async Task<IEnumerable<UsuarioResponse>> GetAllAsync()
        {
            return await ExecuteOperacionAsync(
                "GetAllUsuarios",
                async () =>
                {
                    var usuarios = await _usuarioRepository.GetAllAsync();
                    return usuarios.Select(MapToResponse);
                });
        }

        // Obtiene un usuario por su email
        public async Task<UsuarioResponse> GetByEmailAsync(string email)
        {
            return await ExecuteOperacionAsync(
                "GetUsuarioByEmail",
                async () =>
                {
                    var usuario = await _usuarioRepository.GetByEmailAsync(email);
                    return MapToResponse(usuario);
                },
                $"Email: {email}");
        }

        // Obtiene usuarios filtrados por rol
        public async Task<IEnumerable<UsuarioResponse>> GetByRolAsync(string rol)
        {
            return await ExecuteOperacionAsync(
                "GetUsuariosByRol",
                async () =>
                {
                    if (!Enum.TryParse<RolUsuario>(rol, true, out var rolEnum))
                    {
                        throw new ArgumentException($"El rol '{rol}' no es válido.");
                    }

                    var usuarios = await _usuarioRepository.GetByRolAsync(rolEnum);
                    return usuarios.Select(MapToResponse);
                },
                $"Rol: {rol}");
        }

        // Desactiva un usuario usando la regla de dominio
        public async Task DesactivarAsync(int id)
        {
            await ExecuteOperacionAsync(
                "DesactivarUsuario",
                async () =>
                {
                    var usuario = await _usuarioRepository.GetByIdAsync(id);
                    usuario.Desactivar();
                    await _usuarioRepository.UpdateAsync(usuario);
                },
                $"Id: {id}");
        }

        // Activa un usuario usando la regla de dominio
        public async Task ActivarAsync(int id)
        {
            await ExecuteOperacionAsync(
                "ActivarUsuario",
                async () =>
                {
                    var usuario = await _usuarioRepository.GetByIdAsync(id);
                    usuario.Activar();
                    await _usuarioRepository.UpdateAsync(usuario);
                },
                $"Id: {id}");
        }

        // Obtiene el perfil editable del usuario autenticado
        public async Task<PerfilUsuarioResponse> GetPerfilAsync(int userId)
        {
            return await ExecuteOperacionAsync(
                "GetPerfilUsuario",
                async () =>
                {
                    var usuario = await _usuarioRepository.GetByIdAsync(userId);
                    var telefono = await ObtenerTelefonoPorRolAsync(usuario);

                    return new PerfilUsuarioResponse
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Email = usuario.Email,
                        Rol = usuario.Rol.ToString(),
                        Telefono = telefono
                    };
                },
                $"UsuarioId: {userId}");
        }

        // Actualiza datos basicos del perfil del usuario autenticado
        public async Task<PerfilUsuarioResponse> ActualizarPerfilAsync(
            int userId,
            ActualizarPerfilRequest request)
        {
            return await ExecuteOperacionAsync(
                "ActualizarPerfilUsuario",
                async () =>
                {
                    if (string.IsNullOrWhiteSpace(request.Nombre))
                        throw new ArgumentException("El nombre es requerido.");

                    if (string.IsNullOrWhiteSpace(request.Email))
                        throw new ArgumentException("El email es requerido.");

                    var usuario = await _usuarioRepository.GetByIdAsync(userId);
                    var nombreNormalizado = request.Nombre.Trim();
                    var emailNormalizado = request.Email.Trim().ToLowerInvariant();

                    var existenteConEmail = await _usuarioRepository.GetByEmailAsync(emailNormalizado);
                    if (existenteConEmail != null && existenteConEmail.Id != userId)
                        throw new Domain.Exceptions.ValidationDomainException("El email ya esta en uso por otro usuario.");

                    usuario.Nombre = nombreNormalizado;
                    usuario.Email = emailNormalizado;
                    await _usuarioRepository.UpdateAsync(usuario);

                    if (usuario.Rol == RolUsuario.Paciente)
                    {
                        var paciente = await _pacienteRepository.GetByIdAsync(userId);
                        paciente.Telefono = string.IsNullOrWhiteSpace(request.Telefono)
                            ? null
                            : request.Telefono.Trim();
                        await _pacienteRepository.UpdateAsync(paciente);
                    }
                    else if (usuario.Rol == RolUsuario.Medico)
                    {
                        var medico = await _medicoRepository.GetByIdAsync(userId);
                        medico.TelefonoConsultorio = string.IsNullOrWhiteSpace(request.Telefono)
                            ? null
                            : request.Telefono.Trim();
                        await _medicoRepository.UpdateAsync(medico);
                    }

                    return await GetPerfilAsync(userId);
                },
                $"UsuarioId: {userId}");
        }

        // Cambia la contrasena del usuario autenticado validando la actual
        public async Task CambiarPasswordAsync(int userId, CambiarPasswordRequest request)
        {
            await ExecuteOperacionAsync(
                "CambiarPasswordUsuario",
                async () =>
                {
                    if (string.IsNullOrWhiteSpace(request.PasswordActual))
                        throw new ArgumentException("La contraseña actual es requerida.");

                    if (string.IsNullOrWhiteSpace(request.PasswordNueva))
                        throw new ArgumentException("La nueva contraseña es requerida.");

                    if (request.PasswordNueva.Length < 8)
                        throw new ArgumentException("La nueva contraseña debe tener al menos 8 caracteres.");

                    if (request.PasswordNueva == request.PasswordActual)
                        throw new ArgumentException("La nueva contraseña debe ser diferente a la actual.");

                    var usuario = await _usuarioRepository.GetByIdAsync(userId);

                    if (!BCrypt.Net.BCrypt.Verify(request.PasswordActual, usuario.PasswordHash))
                        throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");

                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordNueva);
                    await _usuarioRepository.UpdateAsync(usuario);
                },
                $"UsuarioId: {userId}");
        }

        private async Task<string?> ObtenerTelefonoPorRolAsync(Domain.Entities.Security.Usuario usuario)
        {
            if (usuario.Rol == RolUsuario.Paciente)
            {
                var paciente = await _pacienteRepository.GetByIdAsync(usuario.Id);
                return paciente.Telefono;
            }

            if (usuario.Rol == RolUsuario.Medico)
            {
                var medico = await _medicoRepository.GetByIdAsync(usuario.Id);
                return medico.TelefonoConsultorio;
            }

            return null;
        }

        // Convierte una entidad Usuario a su DTO de respuesta
        private static UsuarioResponse MapToResponse(
            Domain.Entities.Security.Usuario usuario)
        {
            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString(),
                FechaCreacion = usuario.FechaCreacion,
                Activo = usuario.Activo
            };
        }
    }
}
