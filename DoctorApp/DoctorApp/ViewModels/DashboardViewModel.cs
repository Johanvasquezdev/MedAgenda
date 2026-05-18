using System.Collections.ObjectModel;
using System.Windows.Input;
using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using DoctorApp.Models;
using DoctorApp.Services.Hubs;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.ViewModels;

/// <summary>
/// Dashboard ViewModel - doctor data, agenda and patient search
/// </summary>
public class DashboardViewModel : BaseViewModel
{
    private Medico _medicoActual = new();
    private ObservableCollection<Medico> _doctoresDisponibles = new();
    private ObservableCollection<Cita> _citasHoy = new();
    private ObservableCollection<Cita> _citasProximas = new();
    private int _totalCitasHoy;
    private int _citasConfirmadas;
    private int _citasPendientes;
    private int _citasCanceladas;
    private DateTime _fechaSeleccionada = DateTime.Today;
    private string _busquedaCedula = string.Empty;
    private Paciente? _pacienteBuscado;
    private string _mensajeBusqueda = string.Empty;

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

    public ObservableCollection<Medico> DoctoresDisponibles
    {
        get => _doctoresDisponibles;
        set
        {
            if (_doctoresDisponibles != value)
            {
                _doctoresDisponibles = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Cita> CitasHoy
    {
        get => _citasHoy;
        set
        {
            if (_citasHoy != value)
            {
                _citasHoy = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Cita> CitasProximas
    {
        get => _citasProximas;
        set
        {
            if (_citasProximas != value)
            {
                _citasProximas = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalCitasHoy
    {
        get => _totalCitasHoy;
        set
        {
            if (_totalCitasHoy != value)
            {
                _totalCitasHoy = value;
                OnPropertyChanged();
            }
        }
    }

    public int CitasConfirmadas
    {
        get => _citasConfirmadas;
        set
        {
            if (_citasConfirmadas != value)
            {
                _citasConfirmadas = value;
                OnPropertyChanged();
            }
        }
    }

    public int CitasPendientes
    {
        get => _citasPendientes;
        set
        {
            if (_citasPendientes != value)
            {
                _citasPendientes = value;
                OnPropertyChanged();
            }
        }
    }

    public int CitasCanceladas
    {
        get => _citasCanceladas;
        set
        {
            if (_citasCanceladas != value)
            {
                _citasCanceladas = value;
                OnPropertyChanged();
            }
        }
    }

    public string BusquedaCedula
    {
        get => _busquedaCedula;
        set
        {
            if (_busquedaCedula != value)
            {
                _busquedaCedula = value;
                OnPropertyChanged();
            }
        }
    }

    public Paciente? PacienteBuscado
    {
        get => _pacienteBuscado;
        set
        {
            if (_pacienteBuscado != value)
            {
                _pacienteBuscado = value;
                OnPropertyChanged();
            }
        }
    }

    public string MensajeBusqueda
    {
        get => _mensajeBusqueda;
        set
        {
            if (_mensajeBusqueda != value)
            {
                _mensajeBusqueda = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime FechaSeleccionada
    {
        get => _fechaSeleccionada;
        set
        {
            if (_fechaSeleccionada != value)
            {
                _fechaSeleccionada = value;
                OnPropertyChanged();
                CargarCitasDelDia();
            }
        }
    }

    public ICommand CargarDashboardCommand { get; }
    public ICommand RefrescarCommand { get; }
    public ICommand SeleccionarDoctorCommand { get; }
    public ICommand BuscarPacientePorCedulaCommand { get; }
    public ICommand LimpiarBusquedaCommand { get; }

    private readonly ICitasService _citasService;
    private readonly IDoctorService _doctorService;
    private readonly IPacienteService _pacienteService;
    private readonly ICitasHubClient _citasHubClient;
    private readonly DoctorApp.Security.ITokenManager _tokenManager;

    public DashboardViewModel(
        ICitasService citasService,
        IDoctorService doctorService,
        IPacienteService pacienteService,
        ICitasHubClient citasHubClient,
        DoctorApp.Security.ITokenManager tokenManager)
    {
        Title = "Panel de Control";

        _citasService = citasService ?? throw new ArgumentNullException(nameof(citasService));
        _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
        _pacienteService = pacienteService ?? throw new ArgumentNullException(nameof(pacienteService));
        _citasHubClient = citasHubClient ?? throw new ArgumentNullException(nameof(citasHubClient));
        _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));

        CargarDashboardCommand = new Command(async () => await CargarDashboard());
        RefrescarCommand = new Command(async () => await CargarDashboard());
        SeleccionarDoctorCommand = new Command<Medico>(async (doctor) => await SeleccionarDoctor(doctor));
        BuscarPacientePorCedulaCommand = new Command(async () => await BuscarPacientePorCedula());
        LimpiarBusquedaCommand = new Command(LimpiarBusqueda);

        _citasHubClient.OnNuevaEnCita += OnNuevaEnCita;
        _citasHubClient.OnCitaConfirmada += OnCitaConfirmada;

        _ = CargarDatosIniciales();
        _ = ConectarAlHub();
    }

    private async Task CargarDatosIniciales()
    {
        await InicializarDoctorDesdeTokenAsync();
    }

    private async Task InicializarDoctorDesdeTokenAsync()
    {
        var medicoId = await _tokenManager.GetUserIdAsync();
        if (!medicoId.HasValue)
        {
            await MostrarNoAutenticado();
            return;
        }

        // Usa el nombre del token como fallback visual inmediato.
        await CargarNombreDesdeTokenAsync(medicoId.Value);

        try
        {
            _doctorService.EstablecerDoctorId(medicoId.Value);
            var doctorDto = await _doctorService.ObtenerDoctorActualAsync();
            var doctor = MapearDoctorDto(doctorDto);

            DoctoresDisponibles.Clear();
            DoctoresDisponibles.Add(doctor);
            await SeleccionarDoctor(doctor);
        }
        catch (UnauthorizedException)
        {
            await MostrarNoAutenticado();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                $"No se pudo cargar el perfil del doctor: {ex.Message}",
                "OK");
        }
    }

    private async Task CargarNombreDesdeTokenAsync(int medicoId)
    {
        var nombreToken = await _tokenManager.GetUserNameAsync();
        if (string.IsNullOrWhiteSpace(nombreToken))
            return;

        var (nombre, apellido) = SepararNombre(nombreToken);
        MedicoActual = new Medico
        {
            Id = medicoId,
            Nombre = nombre,
            Apellido = apellido
        };
    }

    private async Task SeleccionarDoctor(Medico doctor)
    {
        if (doctor == null)
            return;

        IsBusy = true;
        try
        {
            MedicoActual = doctor;
            if (doctor.Id > 0)
                _doctorService.EstablecerDoctorId(doctor.Id);

            var agenda = await _citasService.ObtenerAgendaAsync(FechaSeleccionada);
            var citas = agenda.Citas ?? new List<CitaResponseDto>();
            var citasMapeadas = citas.Select(MapearCitaDtoACita).ToList();
            var fechaBase = FechaSeleccionada.Date;

            CitasHoy = new ObservableCollection<Cita>(
                citasMapeadas
                    .Where(c => c.FechaHora.Date == fechaBase)
                    .ToList()
            );

            CitasProximas = new ObservableCollection<Cita>(
                citasMapeadas
                    .Where(c => c.FechaHora.Date > fechaBase && c.Estado is not EstadoCita.Cancelada and not EstadoCita.NoAsistio)
                    .ToList()
            );

            ActualizarEstadisticas();
        }
        catch (UnauthorizedException)
        {
            await MostrarNoAutenticado();
        }
        catch (ConnectionException)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error de Conexion",
                "No se pudo obtener las citas del doctor. Intenta mas tarde.",
                "Reintentar");
            CitasHoy.Clear();
            CitasProximas.Clear();
            ActualizarEstadisticas();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error",
                $"Ocurrio un error: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BuscarPacientePorCedula()
    {
        if (string.IsNullOrWhiteSpace(BusquedaCedula))
        {
            MensajeBusqueda = "Por favor ingresa una cedula";
            PacienteBuscado = null;
            return;
        }

        IsBusy = true;
        MensajeBusqueda = string.Empty;

        try
        {
            var pacienteDto = await _pacienteService.ObtenerPorCedulaAsync(BusquedaCedula);
            if (pacienteDto != null)
            {
                PacienteBuscado = MapearPacienteDto(pacienteDto);
                MensajeBusqueda = $"Paciente encontrado: {PacienteBuscado.NombreCompleto}";
            }
            else
            {
                MensajeBusqueda = "Paciente no encontrado";
                PacienteBuscado = null;
            }
        }
        catch (UnauthorizedException)
        {
            await MostrarNoAutenticado();
        }
        catch (ConnectionException)
        {
            MensajeBusqueda = "Error de conexion. Intenta mas tarde.";
            PacienteBuscado = null;
        }
        catch (Exception ex)
        {
            MensajeBusqueda = $"Error: {ex.Message}";
            PacienteBuscado = null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LimpiarBusqueda()
    {
        BusquedaCedula = string.Empty;
        PacienteBuscado = null;
        MensajeBusqueda = string.Empty;
    }

    private async Task ConectarAlHub()
    {
        try
        {
            await _citasHubClient.ConnectAsync();
        }
        catch (UnauthorizedException)
        {
            await MostrarNoAutenticado();
        }
        catch (ConnectionException)
        {
            // ignore, se reintentara luego
        }
    }

    private void OnNuevaEnCita(object? sender, CitaHubEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (MedicoActual != null)
            {
                _ = SeleccionarDoctor(MedicoActual);
            }
        });
    }

    private void OnCitaConfirmada(object? sender, CitaHubEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (MedicoActual != null)
            {
                _ = SeleccionarDoctor(MedicoActual);
            }
        });
    }

    private void CargarCitasDelDia()
    {
        if (MedicoActual != null && MedicoActual.Id > 0)
        {
            _ = SeleccionarDoctor(MedicoActual);
        }
    }

    private async Task CargarDashboard()
    {
        IsBusy = true;
        try
        {
            if (MedicoActual != null && MedicoActual.Id > 0)
            {
                await SeleccionarDoctor(MedicoActual);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ActualizarEstadisticas()
    {
        TotalCitasHoy = CitasHoy.Count;
        CitasConfirmadas = CitasHoy.Count(c => c.Estado is EstadoCita.Confirmada or EstadoCita.EnCurso or EstadoCita.Completada);
        CitasPendientes = CitasHoy.Count(c => c.Estado is EstadoCita.Pendiente or EstadoCita.Reprogramada);
        CitasCanceladas = CitasHoy.Count(c => c.Estado is EstadoCita.Cancelada or EstadoCita.NoAsistio);
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

    private static Paciente MapearPacienteDto(PacienteResponseDto dto)
    {
        var (nombre, apellido) = SepararNombre(dto.Nombre);
        return new Paciente
        {
            Id = dto.Id,
            Nombre = nombre,
            Apellido = apellido,
            Cedula = dto.Cedula ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            Telefono = dto.Telefono ?? string.Empty
        };
    }

    private Cita MapearCitaDtoACita(CitaResponseDto citaDto)
    {
        var estado = MapearEstado(citaDto.Estado);
        return new Cita
        {
            Id = citaDto.Id,
            FechaHora = citaDto.FechaHora,
            Motivo = citaDto.Motivo,
            Confirmada = estado is EstadoCita.Confirmada or EstadoCita.EnCurso or EstadoCita.Completada,
            Paciente = new Paciente
            {
                Id = citaDto.PacienteId,
                Nombre = citaDto.PacienteNombre,
                Cedula = string.Empty
            },
            Estado = estado,
            DuracionMinutos = citaDto.DuracionMinutos
        };
    }

    private static EstadoCita MapearEstado(string? estado)
    {
        return (estado ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "confirmada" => EstadoCita.Confirmada,
            "solicitada" => EstadoCita.Pendiente,
            "pendiente" => EstadoCita.Pendiente,
            "enprogreso" => EstadoCita.EnCurso,
            "en_progreso" => EstadoCita.EnCurso,
            "encurso" => EstadoCita.EnCurso,
            "en_curso" => EstadoCita.EnCurso,
            "completada" => EstadoCita.Completada,
            "cancelada" => EstadoCita.Cancelada,
            "rechazada" => EstadoCita.Cancelada,
            "reprogramada" => EstadoCita.Reprogramada,
            "noasistio" => EstadoCita.NoAsistio,
            "no_asistio" => EstadoCita.NoAsistio,
            _ => EstadoCita.Pendiente
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

    private static Task MostrarNoAutenticado()
    {
        if (Application.Current?.Dispatcher == null || Application.Current.MainPage == null)
            return Task.CompletedTask;

        return Application.Current.Dispatcher.DispatchAsync(async () =>
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "No autenticado. Inicie sesion nuevamente.",
                "OK");

            Application.Current.MainPage = new DoctorApp.Views.LoginPage();
        });
    }

    public async Task OnDisappearing()
    {
        try
        {
            if (_citasHubClient.IsConnected)
                await _citasHubClient.DisconnectAsync();
        }
        catch
        {
            // ignore
        }
    }
}
