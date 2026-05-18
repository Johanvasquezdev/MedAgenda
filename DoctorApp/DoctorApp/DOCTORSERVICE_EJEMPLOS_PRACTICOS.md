# Ejemplo Práctico: Integración de DoctorService en ViewModels

Este archivo muestra cómo integrar `DoctorService` en tus ViewModels con ejemplos reales.

---

## Ejemplo 1: RegistroDoctorViewModel (Registro Completo)

```csharp
using System.Windows.Input;
using DoctorApp.DTOs.Requests;
using DoctorApp.Exceptions;
using DoctorApp.Services.Interfaces;
using DoctorApp.ViewModels;

namespace DoctorApp.ViewModels
{
    public class RegistroDoctorViewModel : BaseViewModel
    {
        private readonly IDoctorService _doctorService;

        private string _nombre = string.Empty;
        private string _apellido = string.Empty;
        private string _especialidad = string.Empty;
        private string _email = string.Empty;
        private string _telefono = string.Empty;
        private string _mensajeEstado = string.Empty;
        private bool _mostrarMensaje;
        private int _doctorIdRegistrado;

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public string Apellido
        {
            get => _apellido;
            set => SetProperty(ref _apellido, value);
        }

        public string Especialidad
        {
            get => _especialidad;
            set => SetProperty(ref _especialidad, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Telefono
        {
            get => _telefono;
            set => SetProperty(ref _telefono, value);
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        public bool MostrarMensaje
        {
            get => _mostrarMensaje;
            set => SetProperty(ref _mostrarMensaje, value);
        }

        public int DoctorIdRegistrado
        {
            get => _doctorIdRegistrado;
            set => SetProperty(ref _doctorIdRegistrado, value);
        }

        public ICommand RegistrarCommand { get; }

        public RegistroDoctorViewModel(IDoctorService doctorService)
        {
            _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
            RegistrarCommand = new Command(async () => await RegistrarDoctor());
        }

        private async Task RegistrarDoctor()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            MostrarMensaje = false;

            try
            {
                // Crear DTO con los datos
                var doctorData = new DoctorRequestDto
                {
                    Nombre = Nombre.Trim(),
                    Apellido = Apellido.Trim(),
                    Especialidad = Especialidad.Trim(),
                    Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim()
                };

                // Llamar al servicio (valida automáticamente)
                var response = await _doctorService.RegistrarDoctorAsync(doctorData);

                // Guardar el ID registrado
                DoctorIdRegistrado = response.Id;
                _doctorService.EstablecerDoctorId(response.Id);

                // Mostrar éxito y navegar
                MensajeEstado = $"¡Bienvenido Dr. {response.Nombre}! Tu ID es {response.Id}";
                MostrarMensaje = true;

                // Navegar al dashboard después de 2 segundos
                await Task.Delay(2000);
                await Shell.Current.GoToAsync("dashboard");
            }
            catch (ValidationException ex)
            {
                // Error de validación
                var errores = string.Join("\n", ex.Errors.Values.SelectMany(e => e));
                MensajeEstado = $"Error de validación:\n{errores}";
                MostrarMensaje = true;
            }
            catch (AppException ex) when (ex.Code == "DOCTOR_EXISTS")
            {
                // Doctor ya existe
                MensajeEstado = "Este doctor ya está registrado en el sistema (email duplicado)";
                MostrarMensaje = true;
            }
            catch (ConnectionException ex)
            {
                // Errores de conexión, 500, 503
                MensajeEstado = ex.Message;
                MostrarMensaje = true;
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error inesperado: {ex.Message}";
                MostrarMensaje = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
```

---

## Ejemplo 2: DashboardViewModel (Mostrar Citas del Doctor)

```csharp
using System.Collections.ObjectModel;
using DoctorApp.DTOs.Responses;
using DoctorApp.Exceptions;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IDoctorService _doctorService;
        private string _nombreDoctor = string.Empty;
        private string _especialidad = string.Empty;

        public ObservableCollection<CitaResponseDto> CitasDelDia { get; } = new();
        public ObservableCollection<CitaResponseDto> CitasPendientes { get; } = new();

        public string NombreDoctor
        {
            get => _nombreDoctor;
            set => SetProperty(ref _nombreDoctor, value);
        }

        public string Especialidad
        {
            get => _especialidad;
            set => SetProperty(ref _especialidad, value);
        }

        public DashboardViewModel(IDoctorService doctorService)
        {
            _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
        }

        public async Task CargarDatosDoctor()
        {
            IsBusy = true;

            try
            {
                // Obtener datos del doctor actual
                var doctor = await _doctorService.ObtenerDoctorActualAsync();

                NombreDoctor = $"Dr. {doctor.Nombre} {doctor.Apellido}";
                Especialidad = doctor.Especialidad;

                // Obtener sus citas de la BD real
                await CargarCitasDelDoctor(doctor.Id);
            }
            catch (AppException ex) when (ex.Code == "NO_DOCTOR_ID")
            {
                await Application.Current.MainPage.DisplayAlert(
                    "No autenticado",
                    "Por favor registra como doctor primero",
                    "OK"
                );
                await Shell.Current.GoToAsync("login");
            }
            catch (AppException ex) when (ex.Code == "DOCTOR_NOT_FOUND")
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Tu perfil de doctor no fue encontrado",
                    "OK"
                );
            }
            catch (ConnectionException ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Conexión",
                    ex.Message,
                    "OK"
                );
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CargarCitasDelDoctor(int doctorId)
        {
            try
            {
                // Obtener todas las citas del doctor desde la BD real
                var citas = await _doctorService.GetCitasByDoctorIdAsync(doctorId);

                CitasDelDia.Clear();
                CitasPendientes.Clear();

                var hoy = DateTime.Now.Date;

                foreach (var cita in citas)
                {
                    // Citas de hoy
                    if (cita.FechaHora.Date == hoy)
                    {
                        CitasDelDia.Add(cita);
                    }

                    // Citas pendientes
                    if (!cita.Confirmada && cita.FechaHora > DateTime.Now)
                    {
                        CitasPendientes.Add(cita);
                    }
                }

                System.Diagnostics.Debug.WriteLine(
                    $"[Dashboard] Cargadas {citas.Count} citas totales. " +
                    $"Hoy: {CitasDelDia.Count}, Pendientes: {CitasPendientes.Count}"
                );
            }
            catch (AppException ex) when (ex.Code == "DOCTOR_NOT_FOUND")
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Doctor no encontrado en la base de datos",
                    "OK"
                );
            }
            catch (ConnectionException ex)
            {
                var mensaje = ex.Message.Contains("500")
                    ? "Error en el servidor. Intenta más tarde."
                    : "Problema de conexión. Verifica tu internet.";

                await Application.Current.MainPage.DisplayAlert(
                    "Error de Conexión",
                    mensaje,
                    "OK"
                );
            }
        }
    }
}
```

---

## Ejemplo 3: ActualizarPerfilViewModel (Editar Datos del Doctor)

```csharp
using System.Windows.Input;
using DoctorApp.DTOs.Requests;
using DoctorApp.Exceptions;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.ViewModels
{
    public class ActualizarPerfilViewModel : BaseViewModel
    {
        private readonly IDoctorService _doctorService;
        private int _doctorId;
        private string _nombre = string.Empty;
        private string _apellido = string.Empty;
        private string _especialidad = string.Empty;
        private string _email = string.Empty;
        private string _mensajeEstado = string.Empty;
        private bool _mostrarMensaje;

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public string Apellido
        {
            get => _apellido;
            set => SetProperty(ref _apellido, value);
        }

        public string Especialidad
        {
            get => _especialidad;
            set => SetProperty(ref _especialidad, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        public bool MostrarMensaje
        {
            get => _mostrarMensaje;
            set => SetProperty(ref _mostrarMensaje, value);
        }

        public ICommand ActualizarCommand { get; }
        public ICommand CargarDatosCommand { get; }

        public ActualizarPerfilViewModel(IDoctorService doctorService)
        {
            _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));
            ActualizarCommand = new Command(async () => await ActualizarPerfil());
            CargarDatosCommand = new Command(async () => await CargarDatosActuales());
        }

        private async Task CargarDatosActuales()
        {
            IsBusy = true;

            try
            {
                var doctor = await _doctorService.ObtenerDoctorActualAsync();

                _doctorId = doctor.Id;
                Nombre = doctor.Nombre;
                Apellido = doctor.Apellido;
                Especialidad = doctor.Especialidad;
                Email = doctor.Email;
            }
            catch (AppException ex)
            {
                MensajeEstado = $"Error: {ex.Message}";
                MostrarMensaje = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ActualizarPerfil()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            MostrarMensaje = false;

            try
            {
                var datosActualizados = new DoctorRequestDto
                {
                    Nombre = Nombre.Trim(),
                    Apellido = Apellido.Trim(),
                    Especialidad = Especialidad.Trim(),
                    Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                    Telefono = null // No se actualiza en este ejemplo
                };

                await _doctorService.ActualizarDoctorAsync(_doctorId, datosActualizados);

                MensajeEstado = "¡Perfil actualizado exitosamente!";
                MostrarMensaje = true;

                await Task.Delay(2000);
                await Shell.Current.GoToAsync("..");
            }
            catch (ValidationException ex)
            {
                var errores = string.Join("\n", ex.Errors.Values.SelectMany(e => e));
                MensajeEstado = $"Errores:\n{errores}";
                MostrarMensaje = true;
            }
            catch (AppException ex) when (ex.Code == "DOCTOR_NOT_FOUND")
            {
                MensajeEstado = "El doctor no fue encontrado en la base de datos";
                MostrarMensaje = true;
            }
            catch (ConnectionException ex)
            {
                MensajeEstado = ex.Message;
                MostrarMensaje = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
```

---

## Ejemplo 4: XAML para Mostrar Citas (DashboardPage)

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="DoctorApp.Views.DashboardPage"
             Title="Dashboard">

    <VerticalStackLayout Padding="20" Spacing="20">

        <!-- Header con datos del doctor -->
        <VerticalStackLayout Spacing="5">
            <Label Text="{Binding NombreDoctor}" FontSize="24" FontAttributes="Bold" />
            <Label Text="{Binding Especialidad}" FontSize="14" TextColor="Gray" />
        </VerticalStackLayout>

        <!-- Sección: Citas de Hoy -->
        <Label Text="Citas de Hoy" FontSize="18" FontAttributes="Bold" />
        <CollectionView ItemsSource="{Binding CitasDelDia}" SelectionMode="Single">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Frame BorderColor="Blue" CornerRadius="10" Padding="15" Margin="0,5">
                        <VerticalStackLayout Spacing="5">
                            <Label Text="{Binding PacienteNombre}" FontAttributes="Bold" />
                            <Label Text="{Binding FechaHora, StringFormat='{0:HH:mm}'}" 
                                   FontSize="12" TextColor="Gray" />
                            <Label Text="{Binding Motivo}" FontSize="12" />
                            <Label Text="{Binding Estado}" 
                                   TextColor="Green" 
                                   FontAttributes="Bold" />
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <!-- Sección: Citas Pendientes -->
        <Label Text="Citas Pendientes de Confirmar" FontSize="18" FontAttributes="Bold" />
        <CollectionView ItemsSource="{Binding CitasPendientes}" SelectionMode="Single">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Frame BorderColor="Orange" CornerRadius="10" Padding="15" Margin="0,5">
                        <VerticalStackLayout Spacing="5">
                            <Label Text="{Binding PacienteNombre}" FontAttributes="Bold" />
                            <Label Text="{Binding FechaHora, StringFormat='{0:dd/MM/yyyy HH:mm}'}" 
                                   FontSize="12" />
                            <Button Text="Confirmar" 
                                    Command="{Binding Source={x:Reference DashboardPageRef}, Path=BindingContext.ConfirmarCommand}"
                                    CommandParameter="{Binding Id}" />
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

    </VerticalStackLayout>

</ContentPage>
```

---

## Flujo de Datos Completo

```
1. Usuario abre LoginPage
   ↓
2. Se autentica con AuthService
   ↓
3. Si es nuevo doctor → RegistroDoctorPage
   - RegistroDoctorViewModel.RegistrarDoctor()
   - Llama: _doctorService.RegistrarDoctorAsync(datos)
   - Cachea el ID: _doctorService.EstablecerDoctorId(id)
   ↓
4. Navega a DashboardPage
   - DashboardViewModel.CargarDatosDoctor()
   - Llama: _doctorService.ObtenerDoctorActualAsync()
   - Llama: _doctorService.GetCitasByDoctorIdAsync(doctorId)
   ↓
5. Muestra:
   - Datos del doctor
   - Citas de hoy
   - Citas pendientes
   ↓
6. Usuario puede actualizar su perfil (ActualizarPerfilPage)
   - ActualizarPerfilViewModel.ActualizarPerfil()
   - Llama: _doctorService.ActualizarDoctorAsync(id, datosNuevos)
```

---

## Resumen

✅ **RegistroDoctorViewModel**: Registra nuevo doctor  
✅ **DashboardViewModel**: Carga y muestra datos e información  
✅ **ActualizarPerfilViewModel**: Edita perfil del doctor  
✅ Todo maneja errores HTTP (400, 404, 409, 500, 503)  
✅ Validación automática con FluentValidation  
✅ UI responsiva con IsBusy y mensajes de estado  

¡Listo para copiar-pegar en tu proyecto! 🚀
