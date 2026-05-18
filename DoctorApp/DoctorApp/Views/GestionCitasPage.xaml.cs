using DoctorApp.ViewModels;

namespace DoctorApp.Views;

public partial class GestionCitasPage : ContentPage
{
    public GestionCitasPage()
    {
        InitializeComponent();
        var services = Application.Current!.Handler.MauiContext!.Services;
        var citasService = services.GetRequiredService<DoctorApp.Services.Interfaces.ICitasService>();
        var pacienteService = services.GetRequiredService<DoctorApp.Services.Interfaces.IPacienteService>();
        BindingContext = new GestionCitasViewModel(citasService, pacienteService);
    }
}
