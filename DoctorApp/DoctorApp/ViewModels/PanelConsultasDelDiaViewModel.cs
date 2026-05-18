using System.Collections.ObjectModel;
using System.Windows.Input;
using DoctorApp.Exceptions;
using DoctorApp.Models;
using DoctorApp.Services.Interfaces;
using DoctorApp.Security;
using DoctorApp.DTOs.Responses;
using System.Collections.Concurrent;

namespace DoctorApp.ViewModels;

/// <summary>
/// ViewModel para el Panel de Consultas del Dia.
/// Carga medico y consultas reales desde la API.
/// </summary>
public class PanelConsultasDelDiaViewModel : BaseViewModel
{
    private Medico _medicoActual = new();
    private ObservableCollection<PacienteConsultaModel> _consultasDelDia = new();
    private PacienteConsultaModel? _pacienteSeleccionado;
    private int _totalConsultas;
    private int _consultasCompletadas;
    private int _consultasEnCurso;
    private int _consultasEsperando;
    private EstadoConsulta _filtroEstado = EstadoConsulta.Esperando;
    private string _busquedaPaciente = string.Empty;

    // Servicios
    private readonly IDoctorService _doctorService;
    private readonly ITokenManager _tokenManager;
    private readonly ICitasService _citasService;
    private readonly IPacienteService _pacienteService;
    private readonly ConcurrentDictionary<int, PacienteResponseDto> _pacientesCache = new();

    public ObservableCollection<PacienteConsultaModel> ConsultasFiltradas
    {
        get
        {
            var filtradas = Consultasdeldia.Where(c =>
                (FiltroEstado == EstadoConsulta.Esperando || c.Estado == FiltroEstado) &&
                (string.IsNullOrEmpty(BusquedaPaciente) ||
                 c.NombreCompleto.Contains(BusquedaPaciente, StringComparison.OrdinalIgnoreCase) ||
                 c.Cedula.Contains(BusquedaPaciente))
            ).OrderBy(c => c.HoraConsulta).ToList();

            var result = new ObservableCollection<PacienteConsultaModel>();
            foreach (var item in filtradas)
                result.Add(item);
            return result;
        }
    }

    public Medico MedicoActual
    {
        get => _medicoActual;
        set
        {
            if (_medicoActual != value)
            {
                _medicoActual = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<PacienteConsultaModel> Consultasdeldia
    {
        get => _consultasDelDia;
        set
        {
            if (_consultasDelDia != value)
            {
                _consultasDelDia = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConsultasFiltradas));
            }
        }
    }

    public PacienteConsultaModel? PacienteSeleccionado
    {
        get => _pacienteSeleccionado;
        set
        {
            if (_pacienteSeleccionado != value)
            {
                _pacienteSeleccionado = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalConsultas
    {
        get => _totalConsultas;
        set
        {
            if (_totalConsultas != value)
            {
                _totalConsultas = value;
                OnPropertyChanged();
            }
        }
    }

    public int ConsultasCompletadas
    {
        get => _consultasCompletadas;
        set
        {
            if (_consultasCompletadas != value)
            {
                _consultasCompletadas = value;
                OnPropertyChanged();
            }
        }
    }

    public int ConsultasEnCurso
    {
        get => _consultasEnCurso;
        set
        {
            if (_consultasEnCurso != value)
            {
                _consultasEnCurso = value;
                OnPropertyChanged();
            }
        }
    }

    public int ConsultasEsperando
    {
        get => _consultasEsperando;
        set
        {
            if (_consultasEsperando != value)
            {
                _consultasEsperando = value;
                OnPropertyChanged();
            }
        }
    }

    public EstadoConsulta FiltroEstado
    {
        get => _filtroEstado;
        set
        {
            if (_filtroEstado != value)
            {
                _filtroEstado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConsultasFiltradas));
            }
        }
    }

    public string BusquedaPaciente
    {
        get => _busquedaPaciente;
        set
        {
            if (_busquedaPaciente != value)
            {
                _busquedaPaciente = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConsultasFiltradas));
            }
        }
    }

    public ICommand IniciarConsultaCommand { get; }
    public ICommand CompletarConsultaCommand { get; }
    public ICommand MarcarNoPrestoCommand { get; }
    public ICommand ReprogramarConsultaCommand { get; }
    public ICommand CargarConsultasCommand { get; }

    public PanelConsultasDelDiaViewModel(
        IDoctorService doctorService,
        ITokenManager tokenManager,
        ICitasService citasService,
        IPacienteService pacienteService)
    {
        Title = "Panel de Consultas del Dia";
        _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
        _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        _citasService = citasService ?? throw new ArgumentNullException(nameof(citasService));
        _pacienteService = pacienteService ?? throw new ArgumentNullException(nameof(pacienteService));

        IniciarConsultaCommand = new Command<PacienteConsultaModel>(async (paciente) => await IniciarConsulta(paciente));
        CompletarConsultaCommand = new Command<PacienteConsultaModel>(async (paciente) => await CompletarConsulta(paciente));
        MarcarNoPrestoCommand = new Command<PacienteConsultaModel>(async (paciente) => await MarcarNoPresentado(paciente));
        ReprogramarConsultaCommand = new Command<PacienteConsultaModel>(async (paciente) => await ReprogramarConsulta(paciente));
        CargarConsultasCommand = new Command(async () => await CargarConsultasDelDia());

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await InicializarMedicoAsync();
        await CargarConsultasDelDia();
    }

    private async Task InicializarMedicoAsync()
    {
        var medicoId = await _tokenManager.GetUserIdAsync();
        var nombreToken = await _tokenManager.GetUserNameAsync();

        if (!string.IsNullOrWhiteSpace(nombreToken))
        {
            var (nombre, apellido) = SepararNombre(nombreToken);
            MedicoActual = new Medico
            {
                Id = medicoId ?? 0,
                Nombre = nombre,
                Apellido = apellido
            };
        }

        if (!medicoId.HasValue)
            return;

        try
        {
            _doctorService.EstablecerDoctorId(medicoId.Value);
            var doctorDto = await _doctorService.ObtenerDoctorActualAsync();
            MedicoActual = MapearDoctorDto(doctorDto);
        }
        catch
        {
            // Si falla, dejamos el nombre del token.
        }
    }

    private async Task CargarConsultasDelDia()
    {
        IsBusy = true;
        try
        {
            var todas = await _citasService.ObtenerCitasMedicoAsync();
            var citas = (todas ?? new List<CitaResponseDto>())
                .OrderBy(c => c.FechaHora)
                .ToList();

            Consultasdeldia.Clear();
            foreach (var cita in citas)
            {
                Consultasdeldia.Add(MapearCitaAConsulta(cita));
            }

            await EnriquecerPacientesAsync();

            ActualizarEstadisticas();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                $"No se pudieron cargar las consultas del dia: {ex.Message}",
                "OK"
            );
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task IniciarConsulta(PacienteConsultaModel? paciente)
    {
        var consulta = paciente ?? PacienteSeleccionado;
        if (consulta == null) return;

        if (consulta.Estado != EstadoConsulta.Esperando)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Aviso",
                "Solo se puede iniciar consulta con pacientes en estado Esperando",
                "OK"
            );
            return;
        }

        try
        {
            await _citasService.IniciarConsultaAsync(consulta.Id);
            await CargarConsultasDelDia();

            await Application.Current!.MainPage!.DisplayAlert(
                "Consulta Iniciada",
                $"Iniciada consulta con {consulta.NombreCompleto}",
                "OK"
            );
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                $"No se pudo iniciar la consulta: {ex.Message}",
                "OK"
            );
        }
    }

    private async Task CompletarConsulta(PacienteConsultaModel? paciente)
    {
        var consulta = paciente ?? PacienteSeleccionado;
        if (consulta == null) return;

        try
        {
            if (consulta.Estado == EstadoConsulta.Completada)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Aviso",
                    "La consulta ya está completada.",
                    "OK"
                );
                return;
            }

            if (consulta.Estado is EstadoConsulta.NoPresento or EstadoConsulta.Reprogramada)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Aviso",
                    "No se puede completar una consulta en estado No Presentó o Reprogramada.",
                    "OK"
                );
                return;
            }

            if (consulta.Estado == EstadoConsulta.Esperando)
            {
                await _citasService.IniciarConsultaAsync(consulta.Id);
            }

            await _citasService.MarcarAsistenciaAsync(consulta.Id, true);
            await CargarConsultasDelDia();

            await Application.Current!.MainPage!.DisplayAlert(
                "Éxito",
                "Cita completada",
                "OK"
            );
        }
        catch (ConflictException)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Aviso",
                "No se pudo completar porque la cita está en un estado no válido.",
                "OK"
            );
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                $"No se pudo completar la consulta: {ex.Message}",
                "OK"
            );
        }
    }

    private async Task MarcarNoPresentado(PacienteConsultaModel? paciente)
    {
        var consulta = paciente ?? PacienteSeleccionado;
        if (consulta == null) return;

        if (consulta.Estado == EstadoConsulta.Completada)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Aviso",
                "No se puede marcar No Presentó en una consulta completada.",
                "OK"
            );
            return;
        }

        if (consulta.Estado == EstadoConsulta.NoPresento)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Aviso",
                "La consulta ya está marcada como No Presentó.",
                "OK"
            );
            return;
        }

        if (consulta.Estado == EstadoConsulta.Reprogramada)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Aviso",
                "No se puede marcar No Presentó en una consulta reprogramada.",
                "OK"
            );
            return;
        }

        bool confirmado = await Application.Current!.MainPage!.DisplayAlert(
            "Confirmar",
            $"Marcar a {consulta.NombreCompleto} como no presentado?",
            "Si",
            "No"
        );

        if (confirmado)
        {
            try
            {
                await _citasService.MarcarAsistenciaAsync(consulta.Id, false);
                await CargarConsultasDelDia();

                await Application.Current!.MainPage!.DisplayAlert(
                    "Registrado",
                    "Paciente marcado como no presentado",
                    "OK"
                );
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"No se pudo marcar la asistencia: {ex.Message}",
                    "OK"
                );
            }
        }
    }

    private async Task ReprogramarConsulta(PacienteConsultaModel? paciente)
    {
        var consulta = paciente ?? PacienteSeleccionado;
        if (consulta == null) return;

        consulta.Estado = EstadoConsulta.Reprogramada;

        await Application.Current!.MainPage!.DisplayAlert(
            "Reprogramada",
            $"Consulta de {consulta.NombreCompleto} marcada para reprogramacion",
            "OK"
        );

        ActualizarEstadisticas();
    }

    private void ActualizarEstadisticas()
    {
        TotalConsultas = Consultasdeldia.Count;
        ConsultasCompletadas = Consultasdeldia.Count(c => c.Estado == EstadoConsulta.Completada);
        ConsultasEnCurso = Consultasdeldia.Count(c => c.Estado == EstadoConsulta.EnConsulta);
        ConsultasEsperando = Consultasdeldia.Count(c => c.Estado == EstadoConsulta.Esperando);

        OnPropertyChanged(nameof(ConsultasFiltradas));
    }

    private static Medico MapearDoctorDto(DoctorResponseDto dto)
    {
        var (nombre, apellido) = SepararNombre(dto.Nombre);
        return new Medico
        {
            Id = dto.Id,
            Nombre = nombre,
            Apellido = apellido,
            Especialidad = dto.Especialidad ?? dto.NombreEspecialidad ?? string.Empty,
            Consultorio = dto.ProveedorSalud ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            Telefono = dto.TelefonoConsultorio ?? string.Empty,
            NumeroLicencia = dto.Exequatur ?? string.Empty,
            Activo = dto.Activo,
            FechaRegistro = DateTime.Now
        };
    }

    private static (string nombre, string apellido) SepararNombre(string nombreCompleto)
    {
        var limpio = (nombreCompleto ?? string.Empty).Trim();
        if (limpio.StartsWith("Dr. ", StringComparison.OrdinalIgnoreCase))
            limpio = limpio.Substring(4).Trim();
        if (limpio.StartsWith("Dra. ", StringComparison.OrdinalIgnoreCase))
            limpio = limpio.Substring(5).Trim();

        var partes = limpio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0)
            return ("Medico", string.Empty);

        var nombre = partes[0];
        var apellido = partes.Length > 1 ? string.Join(" ", partes.Skip(1)) : string.Empty;
        return (nombre, apellido);
    }

    private static PacienteConsultaModel MapearCitaAConsulta(CitaResponseDto dto)
    {
        var (nombre, apellido) = SepararNombrePaciente(dto.PacienteNombre);

        return new PacienteConsultaModel
        {
            Id = dto.Id,
            Nombre = nombre,
            Apellido = apellido,
            HoraConsulta = dto.FechaHora,
            Motivo = string.IsNullOrWhiteSpace(dto.Motivo) ? "Consulta general" : dto.Motivo,
            Estado = MapearEstadoConsulta(dto.Estado),
            Cedula = "N/D",
            Telefono = "No disponible",
            Email = "No disponible",
            Edad = "N/D",
            AntecedentesRelevantes = string.IsNullOrWhiteSpace(dto.Notas) ? "Sin antecedentes registrados" : dto.Notas,
            FechaCreacion = dto.FechaCreacion
        };
    }

    private async Task EnriquecerPacientesAsync()
    {
        var ids = Consultasdeldia
            .Select(c => c.Id)
            .Where(id => id > 0)
            .ToList();

        var citasPorId = (await _citasService.ObtenerCitasMedicoAsync())
            .ToDictionary(c => c.Id, c => c);

        foreach (var consulta in Consultasdeldia)
        {
            if (!citasPorId.TryGetValue(consulta.Id, out var citaDto))
                continue;

            var pacienteId = citaDto.PacienteId;
            if (pacienteId <= 0)
                continue;

            try
            {
                if (!_pacientesCache.ContainsKey(pacienteId))
                {
                    var pacienteDto = await _pacienteService.ObtenerPorIdAsync(pacienteId);
                    if (pacienteDto != null)
                    {
                        _pacientesCache[pacienteId] = pacienteDto;
                    }
                }

                if (_pacientesCache.TryGetValue(pacienteId, out var p))
                {
                    consulta.Cedula = string.IsNullOrWhiteSpace(p.Cedula) ? consulta.Cedula : p.Cedula;
                    consulta.Telefono = string.IsNullOrWhiteSpace(p.Telefono) ? consulta.Telefono : p.Telefono;
                    consulta.Email = string.IsNullOrWhiteSpace(p.Email) ? consulta.Email : p.Email;
                    consulta.Edad = p.Edad.HasValue ? p.Edad.Value.ToString() : consulta.Edad;
                }
            }
            catch
            {
                // Mantener datos parciales si falla un paciente.
            }
        }

        OnPropertyChanged(nameof(ConsultasFiltradas));
    }

    private static EstadoConsulta MapearEstadoConsulta(string? estado)
    {
        var normalizado = (estado ?? string.Empty).Trim().ToLowerInvariant().Replace(" ", string.Empty);

        return normalizado switch
        {
            "confirmada" => EstadoConsulta.Esperando,
            "pendiente" => EstadoConsulta.Esperando,
            "solicitada" => EstadoConsulta.Esperando,
            "encurso" => EstadoConsulta.EnConsulta,
            "enprogreso" => EstadoConsulta.EnConsulta,
            "en_progreso" => EstadoConsulta.EnConsulta,
            "completada" => EstadoConsulta.Completada,
            "noasistio" => EstadoConsulta.NoPresento,
            "no_asistio" => EstadoConsulta.NoPresento,
            "cancelada" => EstadoConsulta.Reprogramada,
            "rechazada" => EstadoConsulta.Reprogramada,
            "reprogramada" => EstadoConsulta.Reprogramada,
            _ => EstadoConsulta.Esperando
        };
    }

    private static (string nombre, string apellido) SepararNombrePaciente(string nombreCompleto)
    {
        var limpio = (nombreCompleto ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(limpio))
            return ("Paciente", string.Empty);

        var partes = limpio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 1)
            return (partes[0], string.Empty);

        return (partes[0], string.Join(" ", partes.Skip(1)));
    }
}
