using DoctorApp.ViewModels;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.Views;

public partial class RegistroDoctorPage : ContentPage
{
    public RegistroDoctorPage()
    {
        InitializeComponent();

        // Obtener servicio de doctor del contenedor DI
        var doctorService = Application.Current!.Handler.MauiContext!.Services.GetRequiredService<IDoctorService>();

        // Inyectar servicio en el ViewModel
        BindingContext = new RegistroDoctorViewModel(doctorService);
    }
}
