using System.Collections.ObjectModel;
using System.Windows.Input;
using DoctorApp.Models;
using DoctorApp.Services.Interfaces;
using DoctorApp.Services.Hubs;
using DoctorApp.Exceptions;
using DoctorApp.DTOs.Requests;

namespace DoctorApp.ViewModels;

/// <summary>
/// ViewModel para la Gestión de Disponibilidad Médica
/// Implementa patrones Senior: MVVM, Idempotencia, Repository Pattern
/// </summary>
public class GestionDisponibilidadViewModel : BaseViewModel
{
    // Fields privados
    private Medico _medicoActual = new();
    private ObservableCollection<DisponibilidadHoraria> _horariosDisponibles = new();
    private DisponibilidadHoraria? _horarioSeleccionado;
    private DiaSemana _diaSeleccionado = DiaSemana.Lunes;
    private TimeOnly _horaInicio = new(08, 0);
    private TimeOnly _horaFin = new(17, 0);
    private int _duracionConsulta = 30;
    private bool _disponible = true;
    private bool _esEditando;
    private string _mensajeConfirmacion = string.Empty;

    // Propiedades Públicas

    public List<DiaSemana> DiasDisponibles => Enum.GetValues(typeof(DiaSemana))
        .Cast<DiaSemana>()
        .ToList();

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

    public ObservableCollection<DisponibilidadHoraria> HorariosDisponibles
    {
        get => _horariosDisponibles;
        set
        {
            if (_horariosDisponibles != value)
            {
                _horariosDisponibles = value;
                OnPropertyChanged();
            }
        }
    }

    public DisponibilidadHoraria? HorarioSeleccionado
    {
        get => _horarioSeleccionado;
        set
        {
            if (_horarioSeleccionado != value)
            {
                _horarioSeleccionado = value;
                OnPropertyChanged();
                ActualizarBotones();
            }
        }
    }

    public DiaSemana DiaSeleccionado
    {
        get => _diaSeleccionado;
        set
        {
            if (_diaSeleccionado != value)
            {
                _diaSeleccionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HorariosDisponibles));
            }
        }
    }

    public TimeOnly HoraInicio
    {
        get => _horaInicio;
        set
        {
            if (_horaInicio != value)
            {
                _horaInicio = value;
                OnPropertyChanged();
            }
        }
    }

    public TimeOnly HoraFin
    {
        get => _horaFin;
        set
        {
            if (_horaFin != value)
            {
                _horaFin = value;
                OnPropertyChanged();
            }
        }
    }

    public int DuracionConsulta
    {
        get => _duracionConsulta;
        set
        {
            if (_duracionConsulta != value)
            {
                _duracionConsulta = value;
                OnPropertyChanged();
            }
        }
    }

    public bool Disponible
    {
        get => _disponible;
        set
        {
            if (_disponible != value)
            {
                _disponible = value;
                OnPropertyChanged();
            }
        }
    }

    public bool EsEditando
    {
        get => _esEditando;
        set
        {
            if (_esEditando != value)
            {
                _esEditando = value;
                OnPropertyChanged();
            }
        }
    }

    public string MensajeConfirmacion
    {
        get => _mensajeConfirmacion;
        set
        {
            if (_mensajeConfirmacion != value)
            {
                _mensajeConfirmacion = value;
                OnPropertyChanged();
            }
        }
    }

    // Commands

    public ICommand AgregarHorarioCommand { get; }
    public ICommand EliminarHorarioCommand { get; }
    public ICommand ActualizarHorarioCommand { get; }
    public ICommand GenerarHorariosCommand { get; }
    public ICommand CargarDisponibilidadesCommand { get; }

    // Servicios inyectados
    private readonly IDisponibilidadService _disponibilidadService;
    private readonly IDisponibilidadHubClient _disponibilidadHubClient;
    private readonly DoctorApp.Security.ITokenManager _tokenManager;

    // Constructor

    public GestionDisponibilidadViewModel(
        IDisponibilidadService disponibilidadService, 
        IDisponibilidadHubClient disponibilidadHubClient,
        DoctorApp.Security.ITokenManager tokenManager)
    {
        Title = "Gestión de Disponibilidad";

        _disponibilidadService = disponibilidadService ?? throw new ArgumentNullException(nameof(disponibilidadService));
        _disponibilidadHubClient = disponibilidadHubClient ?? throw new ArgumentNullException(nameof(disponibilidadHubClient));
        _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));

        // Inicializar Commands
        AgregarHorarioCommand = new Command(async () => await AgregarHorario());
        EliminarHorarioCommand = new Command(async () => await EliminarHorario());
        ActualizarHorarioCommand = new Command(async () => await ActualizarHorario());
        GenerarHorariosCommand = new Command(async () => await GenerarHorariosDiarios());
        CargarDisponibilidadesCommand = new Command(async () => await CargarDisponibilidades());

        // Suscribirse a eventos del hub
        _disponibilidadHubClient.OnDisponibilidadCreada += OnDisponibilidadCreada;
        _disponibilidadHubClient.OnDisponibilidadActualizada += OnDisponibilidadActualizada;
        _disponibilidadHubClient.OnDisponibilidadEliminada += OnDisponibilidadEliminada;

        // Inicializacion secuencial para evitar condiciones de carrera
        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        try
        {
            await InicializarMedicoAsync();
            await CargarDisponibilidades();
            await ConectarAlHub();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GestionDisponibilidadViewModel] Error inicializando: {ex}");
        }
    }

    private async Task ConectarAlHub()
    {
        try
        {
            await _disponibilidadHubClient.ConnectAsync();
        }
        catch (UnauthorizedException)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", "No autenticado. Inicie sesión nuevamente.", "OK");
        }
        catch (ConnectionException ex)
        {
            System.Diagnostics.Debug.WriteLine($"SignalR connection failed: {ex.Message}");
        }
    }

    private void OnDisponibilidadCreada(object? sender, Services.Hubs.DisponibilidadHubEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _ = CargarDisponibilidades();
        });
    }

    private void OnDisponibilidadActualizada(object? sender, Services.Hubs.DisponibilidadHubEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _ = CargarDisponibilidades();
        });
    }

    private void OnDisponibilidadEliminada(object? sender, Services.Hubs.DisponibilidadHubEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var item = HorariosDisponibles.FirstOrDefault(h => h.Id == e.DisponibilidadId);
            if (item != null)
                HorariosDisponibles.Remove(item);
        });
    }

    // Métodos Privados

    private void ActualizarBotones()
    {
        OnPropertyChanged(nameof(HorarioSeleccionado));
    }

    // Métodos Públicos

    private async Task CargarDisponibilidades()
    {
        IsBusy = true;
        try
        {
            if (MedicoActual.Id <= 0)
            {
                await InicializarMedicoAsync();
            }

            if (MedicoActual.Id <= 0)
            {
                HorariosDisponibles.Clear();
                MensajeConfirmacion = "No se encontro el medico autenticado";
                return;
            }

            var disponibilidades = await _disponibilidadService.ObtenerDisponibilidadesAsync(MedicoActual.Id);

            if (disponibilidades == null || disponibilidades.Count == 0)
            {
                HorariosDisponibles.Clear();
                MensajeConfirmacion = "No hay disponibilidades registradas";
                return;
            }

            HorariosDisponibles.Clear();

            foreach (var disp in disponibilidades)
            {
                HorariosDisponibles.Add(new DisponibilidadHoraria
                {
                    Id = disp.Id,
                    Dia = ObtenerDiaSemanaDeString(disp.DiaSemana),
                    Hora = disp.HoraInicio.ToString("HH:mm"),
                    HoraInicio = TimeOnly.FromTimeSpan(disp.HoraInicio),
                    HoraFin = TimeOnly.FromTimeSpan(disp.HoraFin),
                    EstaDisponible = disp.EsRecurrente,
                    DuracionMinutos = disp.DuracionCitaMin,
                    FechaRegistro = DateTime.Now,
                    Activo = disp.EsRecurrente
                });
            }

            MensajeConfirmacion = $"{disponibilidades.Count} disponibilidades cargadas correctamente";
        }
        catch (UnauthorizedException)
        {
            MensajeConfirmacion = "Sesión expirada. Inicia sesión nuevamente.";
            await Application.Current!.MainPage!.DisplayAlert("Error de Autenticación", MensajeConfirmacion, "OK");
        }
        catch (AppException ex) when (ex.Code == "NOT_FOUND")
        {
            // 404: No se encontraron disponibilidades
            HorariosDisponibles.Clear();
            MensajeConfirmacion = "No se encontraron disponibilidades";
        }
        catch (ConnectionException ex)
        {
            HorariosDisponibles.Clear();
            MensajeConfirmacion = "Error de conexión: No se pudo conectar al servidor";
            await Application.Current!.MainPage!.DisplayAlert("Error de Conexión", $"No se pudo conectar: {ex.Message}", "Reintentar");
        }
        catch (Exception ex)
        {
            MensajeConfirmacion = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[GestionDisponibilidadViewModel] Error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private DiaSemana ObtenerDiaSemanaDeString(string nombre)
    {
        return nombre.ToLower() switch
        {
            "lunes" => DiaSemana.Lunes,
            "martes" => DiaSemana.Martes,
            "miercoles" => DiaSemana.Miercoles,
            "jueves" => DiaSemana.Jueves,
            "viernes" => DiaSemana.Viernes,
            "sabado" => DiaSemana.Sabado,
            "domingo" => DiaSemana.Domingo,
            _ => DiaSemana.Lunes
        };
    }

    private async Task InicializarMedicoAsync()
    {
        var medicoId = await _tokenManager.GetUserIdAsync();
        if (medicoId.HasValue)
        {
            MedicoActual.Id = medicoId.Value;
        }

        var nombre = await _tokenManager.GetUserNameAsync();
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            MedicoActual.Nombre = nombre;
        }
    }

    private async Task AgregarHorario()
    {
        if (HoraInicio >= HoraFin)
        {
            MensajeConfirmacion = "La hora de inicio debe ser menor a la hora de fin";
            return;
        }

        if (MedicoActual.Id <= 0)
        {
            await InicializarMedicoAsync();
            if (MedicoActual.Id <= 0)
            {
                MensajeConfirmacion = "No se encontro el medico autenticado";
                return;
            }
        }

        IsBusy = true;
        try
        {
            var request = new CrearDisponibilidadRequest
            {
                MedicoId = MedicoActual.Id,
                DiaSemana = (int)DiaSeleccionado,
                HoraInicio = HoraInicio.ToTimeSpan(),
                HoraFin = HoraFin.ToTimeSpan(),
                DuracionCitaMin = DuracionConsulta,
                EsRecurrente = Disponible
            };

            var resultado = await _disponibilidadService.CrearDisponibilidadAsync(request);

            if (resultado != null)
            {
                MensajeConfirmacion = "Horario agregado exitosamente";
                HoraInicio = HoraInicio.AddMinutes(DuracionConsulta);

                // Recargar lista
                await CargarDisponibilidades();
            }
        }
        catch (ConflictException)
        {
            MensajeConfirmacion = "Este horario ya existe";
        }
        catch (ValidationException ex)
        {
            MensajeConfirmacion = $"Validación: {ex.Message}";
        }
        catch (ConnectionException ex)
        {
            MensajeConfirmacion = $"Error de conexión: {ex.Message}";
        }
        catch (Exception ex)
        {
            MensajeConfirmacion = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ActualizarHorario()
    {
        if (HorarioSeleccionado == null)
        {
            MensajeConfirmacion = "Selecciona un horario para actualizar";
            return;
        }

        IsBusy = true;
        try
        {
            var request = new ActualizarDisponibilidadRequest
            {
                DisponibilidadId = HorarioSeleccionado.Id,
                MedicoId = MedicoActual.Id,
                DiaSemana = (int)HorarioSeleccionado.Dia,
                HoraInicio = HorarioSeleccionado.HoraInicio.ToTimeSpan(),
                HoraFin = HorarioSeleccionado.HoraFin.ToTimeSpan(),
                DuracionCitaMin = DuracionConsulta,
                EsRecurrente = Disponible
            };

            var resultado = await _disponibilidadService.ActualizarDisponibilidadAsync(request);

            if (resultado != null)
            {
                MensajeConfirmacion = "Horario actualizado exitosamente";
                EsEditando = false;

                // Recargar lista
                await CargarDisponibilidades();
            }
        }
        catch (ValidationException ex)
        {
            MensajeConfirmacion = $"Validación: {ex.Message}";
        }
        catch (ConnectionException ex)
        {
            MensajeConfirmacion = $"Error de conexión: {ex.Message}";
        }
        catch (Exception ex)
        {
            MensajeConfirmacion = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task EliminarHorario()
    {
        if (HorarioSeleccionado == null)
        {
            MensajeConfirmacion = "Selecciona un horario para eliminar";
            return;
        }

        bool confirmado = await Application.Current!.MainPage!.DisplayAlert(
            "Confirmar",
            $"¿Eliminar horario {HorarioSeleccionado.Hora}?",
            "Sí",
            "No"
        );

        if (!confirmado)
            return;

        IsBusy = true;
        try
        {
            await _disponibilidadService.EliminarDisponibilidadAsync(HorarioSeleccionado.Id);

            HorariosDisponibles.Remove(HorarioSeleccionado);
            HorarioSeleccionado = null;
            MensajeConfirmacion = "Horario eliminado exitosamente";
        }
        catch (ConnectionException ex)
        {
            MensajeConfirmacion = $"Error de conexión: {ex.Message}";
        }
        catch (Exception ex)
        {
            MensajeConfirmacion = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GenerarHorariosDiarios()
    {
        IsBusy = true;
        try
        {
            await CargarDisponibilidades();
            MensajeConfirmacion = "Horarios cargados para la semana";
        }
        catch (Exception ex)
        {
            MensajeConfirmacion = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Desconecta del hub SignalR cuando el ViewModel se destruye
    /// </summary>
    public async Task OnDisappearing()
    {
        try
        {
            if (_disponibilidadHubClient.IsConnected)
                await _disponibilidadHubClient.DisconnectAsync();
        }
        catch { /* Ignore */ }
    }
}
