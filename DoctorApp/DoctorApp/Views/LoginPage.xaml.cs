using DoctorApp.ViewModels;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        // Obtener servicio de autenticación del contenedor DI
        var authService = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IAuthService>();

        // Inyectar servicio en el ViewModel
        BindingContext = new LoginViewModel(authService);
    }
}
