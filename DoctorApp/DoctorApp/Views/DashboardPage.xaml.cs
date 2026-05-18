using DoctorApp.ViewModels;
using DoctorApp.Services.Interfaces;
using DoctorApp.Services.Hubs;

namespace DoctorApp.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();

        // Obtener servicios del contenedor DI
        var citasService = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<ICitasService>();
        var doctorService = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IDoctorService>();
        var pacienteService = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IPacienteService>();
        var citasHubClient = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<ICitasHubClient>();
        var tokenManager = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<DoctorApp.Security.ITokenManager>();

        // Inyectar servicios en el ViewModel
        BindingContext = new DashboardViewModel(citasService, doctorService, pacienteService, citasHubClient, tokenManager);
    }
}

