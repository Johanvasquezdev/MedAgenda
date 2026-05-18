# 🏛️ Arquitectura del Frontend SGCM Doctor

## 📐 Visión General de la Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                    CAPA DE PRESENTACIÓN (UI)               │
│  (Vistas XAML + Code-Behind)                               │
│  - DashboardPage                                           │
│  - GestionDisponibilidadPage                              │
│  - GestionCitasPage                                       │
│  - LoginPage                                              │
└──────────────────┬──────────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────────┐
│              CAPA DE LÓGICA (ViewModels)                   │
│  - DashboardViewModel                                      │
│  - GestionDisponibilidadViewModel                         │
│  - GestionCitasViewModel                                  │
│  - LoginViewModel                                         │
│  - BaseViewModel (base)                                   │
└──────────────────┬──────────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────────┐
│           CAPA DE DATOS (Models + Converters)             │
│  - Paciente, Medico, Cita                                 │
│  - DisponibilidadDiaria, DisponibilidadEspecial          │
│  - EstadoCita (Enum)                                      │
│  - Converters (para transformaciones)                     │
└──────────────────┬──────────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────────┐
│              API REST / BASE DE DATOS                      │
│  (Futuro - Aún no integrado)                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎨 Patrón de Diseño: MVVM (Model-View-ViewModel)

### Model (M)
```csharp
// Modelos puros de dominio
public class Cita { ... }
public class Paciente { ... }
public class DisponibilidadDiaria { ... }
```

### View (V)
```xaml
<!-- Interfaz de usuario XAML -->
<ContentPage>
    <VerticalStackLayout>
        <Label Text="{Binding Titulo}" />
        <Button Command="{Binding GuardarCommand}" />
    </VerticalStackLayout>
</ContentPage>
```

### ViewModel (VM)
```csharp
// Lógica de presentación
public class GestionDisponibilidadViewModel : BaseViewModel
{
    public ObservableCollection<DisponibilidadDiaria> Disponibilidades { get; }
    public ICommand GuardarDisponibilidadCommand { get; }
}
```

---

## 🔄 Flujo de Datos

### 1. Binding Bidireccional (Two-Way Binding)

```
View (XAML)
    ↓ (Usuario ingresa datos)
ViewModel (Propiedades con INotifyPropertyChanged)
    ↓ (Datos del ViewModel)
View (Se actualiza automáticamente)
```

Ejemplo:
```xaml
<!-- View -->
<TimePicker Time="{Binding HoraInicio}" />
```

```csharp
// ViewModel
private TimeOnly _horaInicio = new(08, 0);
public TimeOnly HoraInicio
{
    get => _horaInicio;
    set
    {
        if (_horaInicio != value)
        {
            _horaInicio = value;
            OnPropertyChanged(); // ← Notifica cambio
        }
    }
}
```

### 2. Comandos (Commands)

```csharp
public class GestionDisponibilidadViewModel
{
    public ICommand GuardarDisponibilidadCommand { get; }

    public GestionDisponibilidadViewModel()
    {
        GuardarDisponibilidadCommand = new Command(
            async () => await GuardarDisponibilidad()
        );
    }

    private async Task GuardarDisponibilidad()
    {
        // Lógica...
    }
}
```

### 3. Collections Observables

```csharp
// Cuando la colección cambia, la View se actualiza automáticamente
public ObservableCollection<DisponibilidadDiaria> Disponibilidades
{
    get => _disponibilidades;
    set
    {
        if (_disponibilidades != value)
        {
            _disponibilidades = value;
            OnPropertyChanged();
        }
    }
}

// En la View:
<CollectionView ItemsSource="{Binding Disponibilidades}" />
```

---

## 📦 Estructura de Componentes

### BaseViewModel (Base para todos los ViewModels)

```csharp
public class BaseViewModel : INotifyPropertyChanged
{
    // Propiedad para indicar carga
    public bool IsBusy { get; set; }

    // Propiedad para título de página
    public string Title { get; set; }

    // Notifica cambios a la View
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

### Converters (Transforman datos para la View)

```csharp
public class BoolToColorConverter : IValueConverter
{
    // Convierte bool a Color
    public object Convert(object value, Type targetType, ...)
    {
        if (value is bool isAvailable)
            return isAvailable 
                ? Color.FromArgb("#10B981")  // Verde
                : Color.FromArgb("#EF4444");  // Rojo
        return Colors.Gray;
    }
}
```

---

## 🌳 Árbol de Composición de Vistas

### AppShell (Raíz)
```
AppShell
├── TabBar (Navegación principal)
│   ├── Dashboard Tab
│   │   └── DashboardPage
│   │       ├── VerticalStackLayout
│   │       ├── HorizontalStackLayout (Cards)
│   │       └── CollectionView (Citas)
│   ├── Disponibilidad Tab
│   │   └── GestionDisponibilidadPage
│   │       ├── Frame (Formulario)
│   │       │   ├── Picker (Día)
│   │       │   ├── TimePicker (Hora Inicio)
│   │       │   ├── TimePicker (Hora Fin)
│   │       │   ├── Stepper (Duración)
│   │       │   ├── Switch (Disponible)
│   │       │   └── Buttons
│   │       └── CollectionView (Disponibilidades)
│   └── Mis Citas Tab
│       └── GestionCitasPage
│           ├── Frame (Filtros)
│           │   ├── Picker (Estado)
│           │   ├── DatePicker (Fecha)
│           │   └── Entry (Búsqueda)
│           └── CollectionView (Citas Filtradas)
```

---

## 🔌 Inyección de Dependencias (DI)

Configuración en `MauiProgramExtensions.cs`:

```csharp
builder.Services
    // Vistas
    .AddSingleton<DashboardPage>()
    .AddSingleton<GestionDisponibilidadPage>()
    .AddSingleton<GestionCitasPage>()

    // ViewModels
    .AddSingleton<DashboardViewModel>()
    .AddSingleton<GestionDisponibilidadViewModel>()
    .AddSingleton<GestionCitasViewModel>();
```

---

## 📱 Ciclo de Vida de una Página

```
1. Página se crea (Constructor)
   ↓
2. InitializeComponent() - Carga XAML
   ↓
3. BindingContext asignado (ViewModel)
   ↓
4. OnAppearing() - Página visible
   ↓
5. Usuario interactúa (Clicks, texto)
   ↓
6. Commands ejecutados
   ↓
7. ViewModel actualiza datos
   ↓
8. View se actualiza (Binding)
   ↓
9. OnDisappearing() - Página oculta
   ↓
10. Limpieza de recursos
```

---

## 🎯 Estados de la Aplicación

```
┌─────────────┐
│   Login     │ ← Pantalla inicial (futuro)
└──────┬──────┘
       │ Autenticación exitosa
       ▼
┌─────────────┐
│  AppShell   │ ← Shell principal
├─────────────┤
│  Dashboard  │ ← Tab 1 (Bienvenida)
│  Disponib.  │ ← Tab 2 (Principal)
│  Citas      │ ← Tab 3
└─────────────┘
```

---

## 🔐 Manejo de Errores y Validación

### Validación en ViewModel

```csharp
private async Task GuardarDisponibilidad()
{
    if (HoraInicio >= HoraFin)
    {
        MensajeConfirmacion = "❌ La hora de inicio debe ser menor";
        return;
    }

    IsBusy = true;
    try
    {
        // Guardar...
        MensajeConfirmacion = "✅ Guardado correctamente";
    }
    catch (Exception ex)
    {
        MensajeConfirmacion = $"❌ Error: {ex.Message}";
    }
    finally
    {
        IsBusy = false;
    }
}
```

### Binding de Validación en View

```xaml
<Label Text="{Binding MensajeConfirmacion}" 
       IsVisible="{Binding MensajeConfirmacion, Converter={StaticResource StringToBoolConverter}}" />
```

---

## 📊 Flujo de Datos en Gestión de Disponibilidad

```
1. Usuario abre Gestión Disponibilidad
   ↓
2. GestionDisponibilidadPage.xaml.cs
   - Crea nueva instancia de GestionDisponibilidadViewModel
   - Establece BindingContext
   ↓
3. ViewModel carga datos simulados (futuro: API)
   - Llena ObservableCollection<DisponibilidadDiaria>
   ↓
4. View renderiza:
   - CollectionView con las disponibilidades
   - Formulario vacío para crear nuevas
   ↓
5. Usuario completa formulario (Día, Hora Inicio, Hora Fin, Duración)
   - Bindings actualizan ViewModel en tiempo real
   ↓
6. Usuario haz clic en "Guardar"
   - Ejecuta GuardarDisponibilidadCommand
   ↓
7. ViewModel:
   - Valida datos
   - Crea o actualiza DisponibilidadDiaria
   - Agrega a Disponibilidades collection
   - Muestra confirmación
   ↓
8. View se actualiza automáticamente
   - Nueva disponibilidad aparece en el listado
```

---

## 🎨 Recursos Globales (App.xaml)

```xaml
<Application.Resources>
    <ResourceDictionary>
        <!-- Converters -->
        <converters:BoolToColorConverter x:Key="BoolToColorConverter" />
        <converters:EstadoCitaColorConverter x:Key="EstadoCitaColorConverter" />

        <!-- Estilos (futuro) -->
        <Style x:Key="LabelPrimaryStyle" TargetType="Label">
            <Setter Property="FontSize" Value="14" />
            <Setter Property="TextColor" Value="#1F2937" />
        </Style>
    </ResourceDictionary>
</Application.Resources>
```

---

## 🔄 Patrón de Comandos

### Comando Simple

```csharp
RefrescarCommand = new Command(async () => await CargarDashboard());
```

### Comando con Parámetro

```csharp
ActualizarEstadoCommand = new Command<EstadoCita>(
    async (estado) => await ActualizarEstadoCita(estado)
);

// En XAML:
<Button Command="{Binding ActualizarEstadoCommand}" 
        CommandParameter="{x:Static local:EstadoCita.Completada}" />
```

---

## 📈 Performance y Optimización

### CollectionView vs ListView
- **CollectionView** ✅ (Usado)
  - Mejor rendimiento
  - Virtualizacion automática
  - Más flexible

### Datos Simulados
- Actualmente: Datos hardcodeados para demo
- Futuro: Cargar desde API

### Lazy Loading
```csharp
// Futuro:
private async Task CargarDisponibilidades()
{
    IsBusy = true;
    try
    {
        var datos = await apiClient.GetDisponibilidades();
        foreach (var item in datos)
            Disponibilidades.Add(item);
    }
    finally
    {
        IsBusy = false;
    }
}
```

---

## 🧪 Testabilidad

### ViewModel es Testeable

```csharp
[TestClass]
public class GestionDisponibilidadViewModelTests
{
    [TestMethod]
    public void GuardarDisponibilidad_ConDatosValidos_Debe Guardar()
    {
        // Arrange
        var vm = new GestionDisponibilidadViewModel();
        vm.DiaSeleccionado = DiaSemana.Lunes;
        vm.HoraInicio = new(08, 0);
        vm.HoraFin = new(12, 0);

        // Act
        vm.GuardarDisponibilidadCommand.Execute(null);

        // Assert
        Assert.AreEqual(1, vm.Disponibilidades.Count);
    }
}
```

---

## 📞 Puntos de Integración Futuro

### 1. API Backend
```csharp
private IApiClient _apiClient;

private async Task GuardarDisponibilidad()
{
    var response = await _apiClient.SaveDisponibilidad(nueva);
    // Manejar respuesta...
}
```

### 2. Base de Datos Local
```csharp
private ILocalDatabase _database;

private async Task CargarDisponibilidades()
{
    var datos = await _database.GetDisponibilidades();
    // Cargar...
}
```

### 3. Notificaciones
```csharp
private INotificationService _notifications;

private async Task GuardarDisponibilidad()
{
    // ...
    await _notifications.ShowAsync(
        "Disponibilidad", 
        "Guardada correctamente"
    );
}
```

---

## 🎓 Aprendizajes Clave

1. **MVVM es poderoso**: Separación clara de responsabilidades
2. **Binding es automático**: No necesitas actualizar manualmente la View
3. **Commands son preferidos**: Mejor que event handlers
4. **BaseViewModel reutilizable**: Reduce código duplicado
5. **Converters transforman datos**: View recibe datos formateados

---

## 📚 Referencias

- [Microsoft MAUI Docs](https://learn.microsoft.com/en-us/dotnet/maui/)
- [MVVM Pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel)
- [Data Binding](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/data-binding/)
- [Commands](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/gestures/tap-gesture#using-commands)

---

**Versión**: 1.0
**Actualizado**: 2025
**Estado**: ✅ Completo
