# 🚀 INICIO RÁPIDO: DoctorService

## Versión Ultra-Simplificada para Empezar

Si no sabes por dónde empezar, aquí está la forma más simple de usar `DoctorService`.

---

## Paso 1: Inyectar el Servicio

En tu ViewModel:

```csharp
public class MiViewModel : BaseViewModel
{
    private readonly IDoctorService _doctorService;

    public MiViewModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }
}
```

---

## Paso 2: Registrar un Doctor

```csharp
public async Task RegistrarDoctor()
{
    try
    {
        var response = await _doctorService.RegistrarDoctorAsync(
            new DoctorRequestDto
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Especialidad = "Cardiología"
            }
        );

        // ✅ Doctor registrado, ID: response.Id
        await App.DisplayAlert("Éxito", $"Doctor ID: {response.Id}", "OK");
    }
    catch (Exception ex)
    {
        // ❌ Error
        await App.DisplayAlert("Error", ex.Message, "OK");
    }
}
```

---

## Paso 3: Obtener Citas del Doctor

```csharp
public async Task MostrarCitasDelDoctor()
{
    try
    {
        int doctorId = 1; // El ID de tu doctor

        var citas = await _doctorService.GetCitasByDoctorIdAsync(doctorId);

        // ✅ Ahora tienes la lista de citas
        foreach (var cita in citas)
        {
            System.Diagnostics.Debug.WriteLine($"Cita: {cita.PacienteNombre}");
        }
    }
    catch (AppException ex) when (ex.Code == "DOCTOR_NOT_FOUND")
    {
        // ❌ El doctor no existe
        await App.DisplayAlert("Error", "Doctor no encontrado", "OK");
    }
    catch (ConnectionException ex)
    {
        // ❌ Problema de conexión
        await App.DisplayAlert("Error", ex.Message, "OK");
    }
}
```

---

## Paso 4: Obtener Datos del Doctor

```csharp
public async Task MostrarDatosDoctor()
{
    try
    {
        var doctor = await _doctorService.ObtenerDoctorActualAsync();

        // ✅ Tienes los datos
        NombreDoctor = doctor.Nombre;
        Consultorio = doctor.Consultorio;
    }
    catch (Exception ex)
    {
        await App.DisplayAlert("Error", ex.Message, "OK");
    }
}
```

---

## Paso 5: Actualizar Datos

```csharp
public async Task ActualizarDatos()
{
    try
    {
        int doctorId = 1;

        await _doctorService.ActualizarDoctorAsync(doctorId,
            new DoctorRequestDto
            {
                Nombre = "Juan Carlos",
                Apellido = "Pérez García",
                Especialidad = "Cardiología Pediátrica"
            }
        );

        // ✅ Actualizado
        await App.DisplayAlert("Éxito", "Datos actualizados", "OK");
    }
    catch (Exception ex)
    {
        await App.DisplayAlert("Error", ex.Message, "OK");
    }
}
```

---

## Flujo Completo en 1 Archivo

```csharp
using DoctorApp.DTOs.Requests;
using DoctorApp.Exceptions;
using DoctorApp.Services.Interfaces;

namespace DoctorApp.ViewModels
{
    public class DoctorQuickViewModel : BaseViewModel
    {
        private readonly IDoctorService _doctorService;

        public DoctorQuickViewModel(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // 1️⃣ REGISTRAR
        public async Task Registrar()
        {
            try
            {
                var doctor = await _doctorService.RegistrarDoctorAsync(
                    new DoctorRequestDto
                    {
                        Nombre = "Carlos",
                        Apellido = "López",
                        Especialidad = "Neurología"
                    }
                );

                System.Diagnostics.Debug.WriteLine($"✅ Registrado: {doctor.Id}");
                _doctorService.EstablecerDoctorId(doctor.Id); // Guardar en caché
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        // 2️⃣ OBTENER CITAS (ESTE ES EL NUEVO)
        public async Task ObtenerCitas()
        {
            try
            {
                var doctorId = _doctorService.ObtenerDoctorIdCacheado() ?? 1;

                var citas = await _doctorService.GetCitasByDoctorIdAsync(doctorId);

                System.Diagnostics.Debug.WriteLine($"✅ Citas encontradas: {citas.Count}");

                foreach (var cita in citas)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"- {cita.PacienteNombre} @ {cita.FechaHora:HH:mm}"
                    );
                }
            }
            catch (AppException ex) when (ex.Code == "DOCTOR_NOT_FOUND")
            {
                System.Diagnostics.Debug.WriteLine("❌ Doctor no existe");
            }
            catch (ConnectionException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Conexión: {ex.Message}");
            }
        }

        // 3️⃣ OBTENER DATOS
        public async Task ObtenerDatos()
        {
            try
            {
                var doctor = await _doctorService.ObtenerDoctorActualAsync();
                System.Diagnostics.Debug.WriteLine(
                    $"✅ {doctor.Nombre} | {doctor.Especialidad}"
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        // 4️⃣ ACTUALIZAR
        public async Task Actualizar()
        {
            try
            {
                var doctorId = _doctorService.ObtenerDoctorIdCacheado() ?? 1;

                await _doctorService.ActualizarDoctorAsync(doctorId,
                    new DoctorRequestDto
                    {
                        Nombre = "Carlos Antonio",
                        Apellido = "López García",
                        Especialidad = "Neurología Pediátrica"
                    }
                );

                System.Diagnostics.Debug.WriteLine("✅ Actualizado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}
```

---

## XAML Ultra-Simple

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             Title="Doctor">

    <VerticalStackLayout Padding="20" Spacing="10">

        <!-- Botones para prueba -->
        <Button Text="1️⃣ Registrar Doctor" Clicked="OnRegistrarClicked" />
        <Button Text="2️⃣ Ver Citas" Clicked="OnVerCitasClicked" />
        <Button Text="3️⃣ Ver Datos" Clicked="OnVerDatosClicked" />
        <Button Text="4️⃣ Actualizar" Clicked="OnActualizarClicked" />

        <!-- Mostrar resultados -->
        <Editor x:Name="ResultadoEditor" 
                IsReadOnly="True"
                HeightRequest="200" />

    </VerticalStackLayout>

</ContentPage>
```

---

## Code-Behind XAML

```csharp
namespace DoctorApp.Views
{
    public partial class DoctorQuickPage : ContentPage
    {
        private readonly DoctorQuickViewModel _viewModel;

        public DoctorQuickPage()
        {
            InitializeComponent();
            _viewModel = new DoctorQuickViewModel(
                Application.Current.Handler.MauiContext.Services
                    .GetRequiredService<IDoctorService>()
            );
            BindingContext = _viewModel;
        }

        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            await _viewModel.Registrar();
            ResultadoEditor.Text += "Registrar llamado\n";
        }

        private async void OnVerCitasClicked(object sender, EventArgs e)
        {
            await _viewModel.ObtenerCitas();
            ResultadoEditor.Text += "Ver citas llamado\n";
        }

        private async void OnVerDatosClicked(object sender, EventArgs e)
        {
            await _viewModel.ObtenerDatos();
            ResultadoEditor.Text += "Ver datos llamado\n";
        }

        private async void OnActualizarClicked(object sender, EventArgs e)
        {
            await _viewModel.Actualizar();
            ResultadoEditor.Text += "Actualizar llamado\n";
        }
    }
}
```

---

## Salida en Debug (Output → Debug)

Cuando ejecutes los botones verás:

```
✅ Registrado: 100
✅ Citas encontradas: 2
- Juan Pérez @ 14:30
- María García @ 15:00
✅ Carlos López | Neurología
✅ Actualizado
```

---

## Notas Importantes

### GetCitasByDoctorIdAsync (el nuevo método)

```csharp
// ✅ Lo que hace:
var citas = await _doctorService.GetCitasByDoctorIdAsync(doctorId);
// 1. Valida que doctorId > 0
// 2. Envía GET a /api/doctores/{doctorId}/citas
// 3. Recibe list de CitaResponseDto desde la BD REAL
// 4. Si error 404 → "Doctor no encontrado"
// 5. Si error 500 → "Error en servidor"
// 6. Si error 503 → "Servidor no disponible"
// 7. Si no hay citas → retorna list vacía

// ❌ Lo que NO hace:
// - No usa datos mock en RELEASE
// - No hardcodea URLs
// - No ignora errores de conexión
```

### Errores Comunes

```csharp
// ❌ MALO: No validar
var citas = await _doctorService.GetCitasByDoctorIdAsync(0); // Falla

// ✅ BUENO: Validar
var doctorId = _doctorService.ObtenerDoctorIdCacheado();
if (doctorId.HasValue)
{
    var citas = await _doctorService.GetCitasByDoctorIdAsync(doctorId.Value);
}

// ❌ MALO: Ignorar errores
var citas = await _doctorService.GetCitasByDoctorIdAsync(1); // ¿Qué pasa si error?

// ✅ BUENO: Manejar errores
try
{
    var citas = await _doctorService.GetCitasByDoctorIdAsync(1);
}
catch (AppException ex) when (ex.Code == "DOCTOR_NOT_FOUND")
{
    // Doctor no existe
}
catch (ConnectionException ex)
{
    // Problema de conexión
}
```

---

## Orden Recomendado de Uso

```
1. Registrar Doctor
   ↓
2. Guardar ID (EstablecerDoctorId)
   ↓
3. Ver Citas (GetCitasByDoctorIdAsync) ← NUEVO
   ↓
4. Ver Datos (ObtenerDoctorActualAsync)
   ↓
5. Actualizar (ActualizarDoctorAsync)
```

---

## Debugging

### Ver logs en Visual Studio

1. Ejecutar app
2. Abierto → View → Output
3. Cambiar dropdown a "Debug"
4. Ejecutar botones
5. Ver output:
   ```
   [DoctorService] Registrando doctor: Juan Pérez
   [DoctorService] Doctor registrado exitosamente. ID: 100
   [DoctorService] Obteniendo citas para doctor 100 desde /api/doctores/100/citas
   ```

### Revisar qué se envía a la API

En Fiddler o equivalente:

```
GET http://localhost:5001/api/doctores/100/citas HTTP/1.1
Authorization: Bearer eyJhbGc...
Content-Type: application/json

Response 200:
[
  {
    "id": 1,
    "pacienteNombre": "Juan",
    "fechaHora": "2024-01-20T14:30:00",
    ...
  }
]
```

---

## ¿Qué Si No Funciona?

| Problema | Solución |
|----------|----------|
| NullReferenceException | ¿IDoctorService está inyectado? |
| ConnectionException | ¿API está corriendo en localhost:5001? |
| 404: Doctor no encontrado | ¿Existe ese doctorId en BD? |
| 500: Error servidor | ¿Revisar logs del backend? |
| List vacía de citas | ¿Hay citas para ese doctor en BD? |

---

## Conclusión

Con solo estos 5 pasos tienes acceso a:

✅ Registrar doctores  
✅ Obtener citas REALES de la BD  
✅ Obtener datos del doctor  
✅ Actualizar datos  
✅ Manejo automático de errores  

**¡Sin hardcoding de URLs!**  
**¡Sin datos mock en producción!**  
**¡Sin complicaciones!**

🚀 **Listos para empezar!**
