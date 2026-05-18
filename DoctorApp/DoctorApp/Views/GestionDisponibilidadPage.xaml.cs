using DoctorApp.ViewModels;
using DoctorApp.Services.Interfaces;
using DoctorApp.Services.Hubs;

namespace DoctorApp.Views;

public partial class GestionDisponibilidadPage : ContentPage
{
    public GestionDisponibilidadPage()
    {
        InitializeComponent();

        // Obtener servicios del contenedor DI
        var disponibilidadService = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IDisponibilidadService>();
        var disponibilidadHubClient = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IDisponibilidadHubClient>();
        var tokenManager = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<DoctorApp.Security.ITokenManager>();

        // Inyectar servicios en el ViewModel
        BindingContext = new GestionDisponibilidadViewModel(disponibilidadService, disponibilidadHubClient, tokenManager);
    }
}
