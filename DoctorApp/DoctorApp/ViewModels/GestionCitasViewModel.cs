using System.Collections.ObjectModel;
using System.Windows.Input;
using DoctorApp.Models;
using DoctorApp.Services.Interfaces;
using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using System.Collections.Concurrent;

namespace DoctorApp.ViewModels;

public class GestionCitasViewModel : BaseViewModel
{
    private ObservableCollection<Cita> _citas = new();
    private ObservableCollection<Cita> _citasFiltradas = new();
    private Cita? _citaSeleccionada;
    private EstadoCita _estadoFiltro = EstadoCita.Todos;
    private DateTime _fechaFiltro = DateTime.Today;
    private bool _usarFechaFiltro;
    private string _busquedaPaciente = string.Empty;
    private bool _mostrarCancelar;

    public List<EstadoCita> EstadosDisponibles => Enum.GetValues(typeof(EstadoCita))
        .Cast<EstadoCita>()
        .ToList();

    public ObservableCollection<Cita> Citas
    {
        get => _citas;
        set
        {
            if (_citas != value)
            {
                _citas = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Cita> CitasFiltradas
    {
        get => _citasFiltradas;
        set
        {
            if (_citasFiltradas != value)
            {
                _citasFiltradas = value;
                OnPropertyChanged();
            }
        }
    }

    public Cita? CitaSeleccionada
    {
        get => _citaSeleccionada;
        set
        {
            if (_citaSeleccionada != value)
            {
                _citaSeleccionada = value;
                OnPropertyChanged();
            }
        }
    }

    public EstadoCita EstadoFiltro
    {
        get => _estadoFiltro;
        set
        {
            if (_estadoFiltro != value)
            {
                _estadoFiltro = value;
                OnPropertyChanged();
                AplicarFiltros();
            }
        }
    }

    public DateTime FechaFiltro
    {
        get => _fechaFiltro;
        set
        {
            if (_fechaFiltro != value)
            {
                _fechaFiltro = value;
                OnPropertyChanged();
                if (UsarFechaFiltro)
                    AplicarFiltros();
            }
        }
    }

    public bool UsarFechaFiltro
    {
        get => _usarFechaFiltro;
        set
        {
            if (_usarFechaFiltro != value)
            {
                _usarFechaFiltro = value;
                OnPropertyChanged();
                AplicarFiltros();
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
                AplicarFiltros();
            }
        }
    }

    public bool MostrarCancelar
    {
        get => _mostrarCancelar;
        set
        {
            if (_mostrarCancelar != value)
            {
                _mostrarCancelar = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand CargarCitasCommand { get; }
    public ICommand ConfirmarCitaCommand { get; }
    public ICommand CompletarCitaCommand { get; }
    public ICommand CancelarCitaCommand { get; }
    public ICommand ActualizarEstadoCommand { get; }

    private readonly ICitasService _citasService;
    private readonly IPacienteService _pacienteService;
    private readonly ConcurrentDictionary<int, PacienteResponseDto> _pacientesCache = new();

    public GestionCitasViewModel(ICitasService citasService, IPacienteService pacienteService)
    {
        _citasService = citasService ?? throw new ArgumentNullException(nameof(citasService));
        _pacienteService = pacienteService ?? throw new ArgumentNullException(nameof(pacienteService));
        Title = "Gestión de Citas";

        CargarCitasCommand = new Command(async () => await CargarCitas());
        ConfirmarCitaCommand = new Command<Cita>(async (cita) => await ConfirmarCita(cita));
        CompletarCitaCommand = new Command<Cita>(async (cita) => await CompletarCita(cita));
        CancelarCitaCommand = new Command<Cita>(async (cita) => await CancelarCitaSeleccionada(cita));
        ActualizarEstadoCommand = new Command<EstadoCita>(async (estado) => await ActualizarEstadoCita(estado));

        _ = CargarCitas();
    }

    private async Task CargarCitas()
    {
        IsBusy = true;
        try
        {
            var citasDto = await _citasService.ObtenerCitasMedicoAsync();
            System.Diagnostics.Debug.WriteLine($"[GestionCitas] API citas recibidas: {citasDto?.Count ?? 0}");

            Citas.Clear();
            foreach (var dto in citasDto)
            {
            Citas.Add(MapearCita(dto));
        }

            await EnriquecerPacientesAsync();

            AplicarFiltros();
        }
        catch (UnauthorizedException)
        {
            await ShowAlertAsync("Error", "No autenticado. Inicie sesión nuevamente.");
        }
        catch (ConnectionException ex)
        {
            await ShowAlertAsync("Error de Conexión", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ConfirmarCita(Cita? cita)
    {
        var citaObjetivo = cita ?? CitaSeleccionada;
        if (citaObjetivo == null) return;

        try
        {
            await _citasService.ConfirmarCitaAsync(citaObjetivo.Id, true);
            await CargarCitas();
        }
        catch (ValidationException ex)
        {
            await ShowAlertAsync("Validación", ex.Message);
        }
        catch (ConnectionException ex)
        {
            await ShowAlertAsync("Error de Conexión", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", ex.Message);
        }
    }

    private async Task CompletarCita(Cita? cita)
    {
        var citaObjetivo = cita ?? CitaSeleccionada;
        if (citaObjetivo == null) return;

        try
        {
            if (citaObjetivo.Estado == EstadoCita.Completada)
            {
                await ShowAlertAsync("Aviso", "La cita ya está completada.");
                return;
            }

            if (citaObjetivo.Estado is EstadoCita.Cancelada or EstadoCita.NoAsistio)
            {
                await ShowAlertAsync("Aviso", "No se puede completar una cita cancelada o no asistida.");
                return;
            }

            if (citaObjetivo.Estado == EstadoCita.Confirmada)
            {
                await _citasService.IniciarConsultaAsync(citaObjetivo.Id);
            }

            await _citasService.MarcarAsistenciaAsync(citaObjetivo.Id, true);
            await CargarCitas();
            await ShowAlertAsync("Éxito", "Cita completada.");
        }
        catch (ValidationException ex)
        {
            await ShowAlertAsync("Validación", ex.Message);
        }
        catch (ConnectionException ex)
        {
            await ShowAlertAsync("Error de Conexión", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", ex.Message);
        }
    }

    private async Task CancelarCitaSeleccionada(Cita? cita)
    {
        var citaObjetivo = cita ?? CitaSeleccionada;
        if (citaObjetivo == null) return;

        try
        {
            if (citaObjetivo.Estado == EstadoCita.Completada)
            {
                await ShowAlertAsync("Aviso", "No se puede cancelar una cita completada.");
                return;
            }

            if (citaObjetivo.Estado == EstadoCita.Cancelada)
            {
                await ShowAlertAsync("Aviso", "La cita ya está cancelada.");
                return;
            }

            await _citasService.CancelarCitaAsync(citaObjetivo.Id);
            await CargarCitas();
        }
        catch (ValidationException ex)
        {
            await ShowAlertAsync("Validación", ex.Message);
        }
        catch (ConnectionException ex)
        {
            await ShowAlertAsync("Error de Conexión", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", ex.Message);
        }
    }

    private async Task ActualizarEstadoCita(EstadoCita nuevoEstado)
    {
        if (CitaSeleccionada == null) return;

        try
        {
            if (nuevoEstado == EstadoCita.Confirmada)
            {
                await _citasService.ConfirmarCitaAsync(CitaSeleccionada.Id, true);
            }
            else if (nuevoEstado == EstadoCita.Completada)
            {
                await _citasService.MarcarAsistenciaAsync(CitaSeleccionada.Id, true);
            }
            else if (nuevoEstado == EstadoCita.Cancelada)
            {
                await _citasService.MarcarAsistenciaAsync(CitaSeleccionada.Id, false);
            }

            await CargarCitas();
        }
        catch (ValidationException ex)
        {
            await ShowAlertAsync("Validación", ex.Message);
        }
        catch (ConnectionException ex)
        {
            await ShowAlertAsync("Error de Conexión", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Error", ex.Message);
        }
    }

    private void AplicarFiltros()
    {
        var filtradas = Citas.Where(c =>
            (EstadoFiltro == EstadoCita.Todos || c.Estado == EstadoFiltro) &&
            (!UsarFechaFiltro || c.FechaHora.Date == FechaFiltro.Date) &&
            (string.IsNullOrEmpty(BusquedaPaciente) ||
             c.Paciente?.NombreCompleto?.Contains(BusquedaPaciente, StringComparison.OrdinalIgnoreCase) == true ||
             c.Paciente?.Cedula?.Contains(BusquedaPaciente) == true)
        ).OrderBy(c => c.FechaHora).ToList();

        CitasFiltradas.Clear();
        foreach (var cita in filtradas)
        {
            CitasFiltradas.Add(cita);
        }
    }

    private static Cita MapearCita(CitaResponseDto dto)
    {
        System.Diagnostics.Debug.WriteLine($"[GestionCitas] Mapeando cita {dto.Id} paciente {dto.PacienteNombre} fecha {dto.FechaHora}");
        var estadoNormalizado = (dto.Estado ?? string.Empty).Trim().ToLowerInvariant();
        var estado = estadoNormalizado switch
        {
            "confirmada" => EstadoCita.Confirmada,
            "encurso" => EstadoCita.EnCurso,
            "en_curso" => EstadoCita.EnCurso,
            "enprogreso" => EstadoCita.EnCurso,
            "en_progreso" => EstadoCita.EnCurso,
            "completada" => EstadoCita.Completada,
            "cancelada" => EstadoCita.Cancelada,
            "rechazada" => EstadoCita.Cancelada,
            "noasistio" => EstadoCita.NoAsistio,
            "no_asistio" => EstadoCita.NoAsistio,
            _ => EstadoCita.Pendiente
        };

        return new Cita
        {
            Id = dto.Id,
            FechaHora = dto.FechaHora,
            Motivo = dto.Motivo ?? string.Empty,
            Confirmada = dto.Confirmada,
            Estado = estado,
            DuracionMinutos = dto.DuracionMinutos,
            Paciente = new Paciente
            {
                Id = dto.PacienteId,
                Nombre = string.IsNullOrWhiteSpace(dto.PacienteNombre) ? "Paciente" : dto.PacienteNombre,
                Cedula = "N/D",
                Telefono = "N/D",
                Email = "N/D"
            }
        };
    }

    private async Task EnriquecerPacientesAsync()
    {
        var pacientesIds = Citas
            .Select(c => c.Paciente?.Id ?? 0)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        foreach (var pacienteId in pacientesIds)
        {
            try
            {
                if (!_pacientesCache.ContainsKey(pacienteId))
                {
                    var dto = await _pacienteService.ObtenerPorIdAsync(pacienteId);
                    if (dto != null)
                    {
                        _pacientesCache[pacienteId] = dto;
                    }
                }
            }
            catch
            {
                // Se mantiene la UI con valores por defecto si falla un paciente.
            }
        }

        foreach (var cita in Citas)
        {
            var paciente = cita.Paciente;
            if (paciente == null || paciente.Id <= 0) continue;

            if (_pacientesCache.TryGetValue(paciente.Id, out var dto))
            {
                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    paciente.Nombre = dto.Nombre;
                if (!string.IsNullOrWhiteSpace(dto.Cedula))
                    paciente.Cedula = dto.Cedula;
                if (!string.IsNullOrWhiteSpace(dto.Telefono))
                    paciente.Telefono = dto.Telefono;
                if (!string.IsNullOrWhiteSpace(dto.Email))
                    paciente.Email = dto.Email;
            }
        }
    }

    private static Task ShowAlertAsync(string title, string message)
    {
        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var page = Application.Current?.MainPage;
            if (page != null)
            {
                await page.DisplayAlert(title, message, "OK");
            }
        });
    }
}
