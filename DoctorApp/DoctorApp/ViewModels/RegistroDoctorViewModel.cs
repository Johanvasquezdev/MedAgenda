using System.Windows.Input;
using DoctorApp.DTOs.Requests;
using DoctorApp.Services.Interfaces;
using DoctorApp.Exceptions;

namespace DoctorApp.ViewModels;

/// <summary>
/// ViewModel para el registro de nuevos doctores
/// </summary>
public class RegistroDoctorViewModel : BaseViewModel
{
    private string _nombre = string.Empty;
    private string _apellido = string.Empty;
    private string _especialidad = string.Empty;
    private string _email = string.Empty;
    private string _telefono = string.Empty;
    private string _mensajeEstado = string.Empty;
    private bool _mostrarMensaje;
    private int? _doctorIdRegistrado;

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (_nombre != value)
            {
                _nombre = value;
                OnPropertyChanged();
            }
        }
    }

    public string Apellido
    {
        get => _apellido;
        set
        {
            if (_apellido != value)
            {
                _apellido = value;
                OnPropertyChanged();
            }
        }
    }

    public string Especialidad
    {
        get => _especialidad;
        set
        {
            if (_especialidad != value)
            {
                _especialidad = value;
                OnPropertyChanged();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }

    public string Telefono
    {
        get => _telefono;
        set
        {
            if (_telefono != value)
            {
                _telefono = value;
                OnPropertyChanged();
            }
        }
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        set
        {
            if (_mensajeEstado != value)
            {
                _mensajeEstado = value;
                OnPropertyChanged();
            }
        }
    }

    public bool MostrarMensaje
    {
        get => _mostrarMensaje;
        set
        {
            if (_mostrarMensaje != value)
            {
                _mostrarMensaje = value;
                OnPropertyChanged();
            }
        }
    }

    public int? DoctorIdRegistrado
    {
        get => _doctorIdRegistrado;
        set
        {
            if (_doctorIdRegistrado != value)
            {
                _doctorIdRegistrado = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand RegistrarCommand { get; }

    private readonly IDoctorService _doctorService;

    public RegistroDoctorViewModel(IDoctorService doctorService)
    {
        Title = "Registro de Doctor";
        _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));

        RegistrarCommand = new Command(async () => await RegistrarDoctor());
    }

    /// <summary>
    /// Registra un nuevo doctor
    /// </summary>
    private async Task RegistrarDoctor()
    {
        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) || string.IsNullOrWhiteSpace(Especialidad))
        {
            MensajeEstado = "Nombre, Apellido y Especialidad son requeridos";
            MostrarMensaje = true;
            return;
        }

        IsBusy = true;
        MostrarMensaje = false;

        try
        {
            var request = new DoctorRequestDto
            {
                Nombre = Nombre.Trim(),
                Apellido = Apellido.Trim(),
                Especialidad = Especialidad.Trim(),
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim()
            };

            var response = await _doctorService.RegistrarDoctorAsync(request);

            if (response != null && response.Id > 0)
            {
                DoctorIdRegistrado = response.Id;
                MensajeEstado = $"¡Registro exitoso! Doctor ID: {response.Id}";
                MostrarMensaje = true;

                // Limpiar formulario
                await Task.Delay(2000);

                // Navegar a Dashboard
                await Shell.Current.GoToAsync("DashboardPage");
            }
            else
            {
                MensajeEstado = "Error: No se recibió respuesta válida del servidor";
                MostrarMensaje = true;
            }
        }
        catch (Exceptions.ValidationException ex)
        {
            // Errores de validación (400)
            var firstError = ex.Errors?.FirstOrDefault().Value?.FirstOrDefault();
            MensajeEstado = firstError ?? "Error de validación";
            MostrarMensaje = true;
        }
        catch (AppException ex) when (ex.Code == "DOCTOR_EXISTS")
        {
            // 409: Doctor ya existe
            MensajeEstado = "Este doctor ya está registrado";
            MostrarMensaje = true;
        }
        catch (ConnectionException ex)
        {
            // Error de conexión
            MensajeEstado = $"Error de conexión: {ex.Message}";
            MostrarMensaje = true;
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error: {ex.Message}";
            MostrarMensaje = true;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
