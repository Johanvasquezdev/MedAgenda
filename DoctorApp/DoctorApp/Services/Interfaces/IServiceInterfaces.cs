using DoctorApp.DTOs.Requests;
using DoctorApp.DTOs.Responses;

namespace DoctorApp.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de Citas
/// </summary>
public interface ICitasService
{
    Task<ObtenerAgendaResponse> ObtenerAgendaAsync(DateTime fecha);
    Task<List<CitaResponseDto>> ObtenerCitasMedicoAsync();
    Task<CitaResponseDto> ConfirmarCitaAsync(int citaId, bool confirmada, string? notas = null);
    Task<CitaResponseDto> CancelarCitaAsync(int citaId, string? motivo = null);
    Task<CitaResponseDto> IniciarConsultaAsync(int citaId);
    Task<CitaResponseDto> MarcarAsistenciaAsync(int citaId, bool asistio, string? observaciones = null);
    Task<List<CitaResponseDto>> ObtenerCitasDelDiaAsync();
    Task<CitaResponseDto?> ObtenerCitaPorIdAsync(int citaId);
}

/// <summary>
/// Interfaz para el servicio de Disponibilidad
/// </summary>
public interface IDisponibilidadService
{
    Task<List<DisponibilidadResponseDto>> ObtenerDisponibilidadesAsync(int medicoId);
    Task<DisponibilidadResponseDto> CrearDisponibilidadAsync(CrearDisponibilidadRequest request);
    Task<DisponibilidadResponseDto> ActualizarDisponibilidadAsync(ActualizarDisponibilidadRequest request);
    Task EliminarDisponibilidadAsync(int disponibilidadId);
    Task<DisponibilidadResponseDto?> ObtenerDisponibilidadPorIdAsync(int disponibilidadId);
}

/// <summary>
/// Interfaz para autenticación
/// </summary>
public interface IAuthService
{
    Task<AuthTokenResponse> LoginAsync(string usuario, string contrasena);
    Task LogoutAsync();
    Task<bool> EstaAutenticadoAsync();
}

/// <summary>
/// Interfaz para el servicio de Doctor - Registro, gestión y consulta de citas
/// Todos los métodos tienen manejo robusto de errores HTTP (404, 500, 503, etc.)
/// </summary>
public interface IDoctorService
{
    /// <summary>
    /// Registra un nuevo doctor en el sistema
    /// </summary>
    /// <param name="datos">Datos del doctor a registrar</param>
    /// <returns>Respuesta con los datos del doctor registrado y su ID</returns>
    Task<DoctorResponseDto> RegistrarDoctorAsync(DoctorRequestDto datos);

    /// <summary>
    /// Obtiene los datos del doctor actual
    /// </summary>
    /// <returns>Datos del doctor</returns>
    Task<DoctorResponseDto> ObtenerDoctorActualAsync();

    /// <summary>
    /// Actualiza los datos del doctor
    /// </summary>
    /// <param name="doctorId">ID del doctor</param>
    /// <param name="datos">Nuevos datos del doctor</param>
    /// <returns>Datos actualizados del doctor</returns>
    Task<DoctorResponseDto> ActualizarDoctorAsync(int doctorId, DoctorRequestDto datos);

    /// <summary>
    /// Obtiene todas las citas de un doctor desde la base de datos real
    /// Maneja errores 404 (doctor no existe) y 500 (error servidor)
    /// </summary>
    /// <param name="doctorId">ID del doctor para filtrar citas</param>
    /// <returns>Lista de citas del doctor, vacía si no hay citas</returns>
    Task<List<CitaResponseDto>> GetCitasByDoctorIdAsync(int doctorId);

    /// <summary>
    /// Establece el ID del doctor en caché (útil después de login o cambio de doctor)
    /// </summary>
    /// <param name="doctorId">ID del doctor a cachear</param>
    void EstablecerDoctorId(int doctorId);

    /// <summary>
    /// Obtiene el ID del doctor cacheado
    /// </summary>
    /// <returns>ID del doctor o null si no hay ninguno</returns>
    int? ObtenerDoctorIdCacheado();
}

/// <summary>
/// Interfaz para el servicio de Pacientes
/// </summary>
public interface IPacienteService
{
    Task<PacienteResponseDto?> ObtenerPorIdAsync(int id);
    Task<PacienteResponseDto?> ObtenerPorCedulaAsync(string cedula);
}
