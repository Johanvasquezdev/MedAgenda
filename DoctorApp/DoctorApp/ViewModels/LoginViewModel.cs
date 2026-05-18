using System.Windows.Input;
using DoctorApp.Services.Interfaces;
using DoctorApp.Exceptions;

namespace DoctorApp.ViewModels;

/// <summary>
/// ViewModel para la página de login.
/// Maneja la autenticación del usuario.
/// </summary>
public class LoginViewModel : BaseViewModel
{
    private string _usuario = string.Empty;
    private string _contrasena = string.Empty;
    private string _mensajeEstado = string.Empty;
    private bool _mostrarMensaje;

    public string Usuario
    {
        get => _usuario;
        set
        {
            if (_usuario != value)
            {
                _usuario = value;
                OnPropertyChanged();
            }
        }
    }

    public string Contrasena
    {
        get => _contrasena;
        set
        {
            if (_contrasena != value)
            {
                _contrasena = value;
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

    public ICommand LoginCommand { get; }

    private readonly IAuthService _authService;

    public LoginViewModel(IAuthService authService)
    {
        Title = "Iniciar Sesión";
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        LoginCommand = new Command(async () => await RealizarLogin());
    }

    /// <summary>
    /// Realiza el login del usuario.
    /// </summary>
    private async Task RealizarLogin()
    {
        // Validar campos vacíos.
        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Contrasena))
        {
            await MostrarErrorAsync("Usuario y contraseña son requeridos");
            return;
        }

        IsBusy = true;
        MostrarMensaje = false;

        try
        {
            // Intentar login.
            var respuesta = await _authService.LoginAsync(Usuario, Contrasena);

            if (respuesta != null && !string.IsNullOrEmpty(respuesta.Token))
            {
                if (!string.Equals(respuesta.Rol, "Medico", StringComparison.OrdinalIgnoreCase))
                {
                    await MostrarErrorAsync("Este usuario no es medico. Usa una cuenta de medico.");
                    return;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current!.MainPage = new AppShell();
                });

                Usuario = string.Empty;
                Contrasena = string.Empty;
            }
            else
            {
                await MostrarErrorAsync("Credenciales inválidas");
            }
        }
        catch (UnauthorizedException)
        {
            await MostrarErrorAsync("Usuario o contraseña incorrectos");
        }
        catch (ConnectionException ex)
        {
            await MostrarErrorAsync($"Error de conexión: {ex.Message}");
        }
        catch (Exception ex)
        {
            await MostrarErrorAsync($"Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task MostrarErrorAsync(string mensaje)
    {
        MensajeEstado = mensaje;
        MostrarMensaje = true;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
            }
        });
    }
}
