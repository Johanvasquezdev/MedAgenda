using DoctorApp.ViewModels;
using DoctorApp.Services.Interfaces;
using DoctorApp.Security;
using Microsoft.Extensions.DependencyInjection;

namespace DoctorApp.Views;

public partial class PanelConsultasDelDiaPage : ContentPage
{
    public PanelConsultasDelDiaPage()
    {
        InitializeComponent();
        var services = Application.Current?.Handler?.MauiContext?.Services;
        if (services != null)
        {
            var doctorService = services.GetRequiredService<IDoctorService>();
            var tokenManager = services.GetRequiredService<ITokenManager>();
            var citasService = services.GetRequiredService<ICitasService>();
            var pacienteService = services.GetRequiredService<IPacienteService>();
            BindingContext = new PanelConsultasDelDiaViewModel(doctorService, tokenManager, citasService, pacienteService);
        }
    }
}
