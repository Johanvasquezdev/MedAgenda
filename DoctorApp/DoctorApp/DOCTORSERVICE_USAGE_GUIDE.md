# Documentación: DoctorService - Consumo de API REST

## Descripción General

`DoctorService` es un servicio completo para manejar todas las operaciones relacionadas con doctores en la aplicación MAUI. Consume endpoints de tu API REST ASP.NET Core y proporciona:

- ✅ Registro de nuevos doctores
- ✅ Obtención de datos del doctor actual
- ✅ Actualización de perfil del doctor
- ✅ Consulta de citas del doctor desde la base de datos real
- ✅ Manejo robusto de errores HTTP (404, 500, 503, etc.)
- ✅ Validación client-side con FluentValidation
- ✅ Caching de ID del doctor

---

## Métodos Disponibles

### 1. `RegistrarDoctorAsync(DoctorRequestDto datos)`

**Descripción:** Registra un nuevo doctor en el sistema.

**Parámetros:**
- `datos` (DoctorRequestDto): Objeto con los datos del doctor

**Retorna:** `Task<DoctorResponseDto>` - Objeto con los datos del doctor registrado (incluyendo su ID asignado)

**Errores manejados:**
- `ValidationException` (400): Si los datos no cumplen validaciones
- `ConflictException` (409): Si el doctor ya existe (email duplicado)
- `ConnectionException` (500): Error interno del servidor
- `ConnectionException` (503): Servidor no disponible

**Ejemplo de uso:**

```csharp
// En tu ViewModel
public class RegistroDoctorViewModel
{
    private readonly IDoctorService _doctorService;

    public RegistroDoctorViewModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task RegistrarDoctor()
    {
        try
        {
            var datos = new DoctorRequestDto
            {
                Nombre = "Juan",
                Apellido = "Pérez García",
                Especialidad = "Cardiología",
                Email = "juan.perez@hospital.com",
                Telefono = "+34 600 123 456"
            };

            var response = await _doctorService.RegistrarDoctorAsync(datos);

            // Doctor registrado exitosamente
            MensajeEstado = $"Doctor registrado. ID: {response.Id}";
            DoctorIdRegistrado = response.Id;

            // Navegar a la siguiente pantalla
            await Shell.Current.GoToAsync("dashboard");
        }
        catch (Exceptions.ValidationException ex)
        {
            // Mostrar errores de validación
            MensajeEstado = $"Errores de validación: {string.Join(", ", ex.Errors.Values.SelectMany(e => e))}";
        }
        catch (AppException ex) when (ex.Code == "DOCTOR_EXISTS")
        {
            MensajeEstado = "Este doctor ya está registrado en el sistema";
        }
        catch (ConnectionException ex)
        {
            MensajeEstado = $"Error de conexión: {ex.Message}";
        }
    }
}
```

---

### 2. `GetCitasByDoctorIdAsync(int doctorId)`

**Descripción:** Obtiene TODAS las citas del doctor desde la base de datos real (no datos mockeados).

**Parámetros:**
- `doctorId` (int): ID del doctor

**Retorna:** `Task<List<CitaResponseDto>>` - Lista de citas del doctor (vacía si no hay citas)

**Errores manejados:**
- `AppException` (400): Si el doctorId es inválido
- `AppException` (404): Si el doctor no existe
- `ConnectionException` (500): Error interno del servidor
- `ConnectionException` (503): Servidor no disponible

**Ejemplo de uso:**

```csharp
// En tu DashboardViewModel
public class DashboardViewModel : BaseViewModel
{
    private readonly IDoctorService _doctorService;
    public ObservableCollection<CitaResponseDto> CitasDelDoctor { get; } = new();

    public DashboardViewModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task CargarCitasDelDoctor(int doctorId)
    {
        IsBusy = true;
        try
        {
            // Obtener citas reales de la BD
            var citas = await _doctorService.GetCitasByDoctorIdAsync(doctorId);

            CitasDelDoctor.Clear();
            foreach (var cita in citas)
            {
                CitasDelDoctor.Add(cita);
            }

            MensajeEstado = $"Cargadas {citas.Count} citas del doctor";
        }
        catch (AppException ex) when (ex.Code == "DOCTOR_NOT_FOUND")
        {
            MensajeEstado = "El doctor no existe en el sistema";
        }
        catch (ConnectionException ex)
        {
            MensajeEstado = ex.Message.Contains("500") 
                ? "Error en el servidor. Intenta más tarde."
                : "Problema de conexión. Verifica tu internet.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

---

### 3. `ObtenerDoctorActualAsync()`

**Descripción:** Obtiene los datos del doctor actualmente logueado (usando su ID cacheado).

**Parámetros:** Ninguno

**Retorna:** `Task<DoctorResponseDto>` - Objeto con los datos del doctor

**Errores manejados:**
- `AppException` (400): Si no hay un doctorId en caché
- `AppException` (404): Si el doctor no existe
- `ConnectionException` (500): Error interno del servidor

**Ejemplo de uso:**

```csharp
public async Task ActualizarDatosDoctor()
{
    try
    {
        var doctorActual = await _doctorService.ObtenerDoctorActualAsync();

        NombreDoctor = doctorActual.Nombre;
        ApellidoDoctor = doctorActual.Apellido;
        EspecialidadDoctor = doctorActual.Especialidad;
        ConsultorioDoctor = doctorActual.Consultorio;
    }
    catch (AppException ex) when (ex.Code == "NO_DOCTOR_ID")
    {
        MensajeEstado = "Por favor, registra como doctor primero";
    }
}
```

---

### 4. `ActualizarDoctorAsync(int doctorId, DoctorRequestDto datos)`

**Descripción:** Actualiza los datos de un doctor.

**Parámetros:**
- `doctorId` (int): ID del doctor a actualizar
- `datos` (DoctorRequestDto): Nuevos datos del doctor

**Retorna:** `Task<DoctorResponseDto>` - Datos actualizados

**Errores manejados:**
- `ValidationException` (400): Errores de validación
- `AppException` (404): Doctor no encontrado
- `ConnectionException` (500): Error servidor

**Ejemplo de uso:**

```csharp
public async Task ActualizarPerfil(int doctorId)
{
    try
    {
        var datosActualizados = new DoctorRequestDto
        {
            Nombre = Nombre,
            Apellido = Apellido,
            Especialidad = Especialidad,
            Email = Email,
            Telefono = Telefono
        };

        var response = await _doctorService.ActualizarDoctorAsync(doctorId, datosActualizados);

        MensajeEstado = "Perfil actualizado exitosamente";
    }
    catch (Exceptions.ValidationException ex)
    {
        MensajeEstado = "Revisa los datos ingresados";
    }
}
```

---

## Métodos Helper

### `EstablecerDoctorId(int doctorId)`

Establece el ID del doctor en caché (útil después de login o registro).

```csharp
// Después del login exitoso
_doctorService.EstablecerDoctorId(doctorResponseDto.Id);
```

### `ObtenerDoctorIdCacheado()`

Obtiene el ID del doctor actualmente en caché.

```csharp
int? doctorId = _doctorService.ObtenerDoctorIdCacheado();
if (doctorId.HasValue)
{
    // Hay un doctor logueado
    await CargarCitas(doctorId.Value);
}
```

---

## DTOs Utilizados

### `DoctorRequestDto` (para enviar datos)

```csharp
public class DoctorRequestDto
{
    public string Nombre { get; set; }           // Requerido
    public string Apellido { get; set; }         // Requerido
    public string Especialidad { get; set; }     // Requerido
    public string? Email { get; set; }           // Opcional
    public string? Telefono { get; set; }        // Opcional
}
```

### `DoctorResponseDto` (respuesta del servidor)

```csharp
public class DoctorResponseDto
{
    public int Id { get; set; }                  // ID asignado por el servidor
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Especialidad { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public string Consultorio { get; set; }
    public DateTime FechaRegistro { get; set; }
}
```

### `CitaResponseDto` (para las citas del doctor)

```csharp
public class CitaResponseDto
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public int PacienteId { get; set; }
    public string PacienteNombre { get; set; }
    public string PacienteCedula { get; set; }
    public string Motivo { get; set; }
    public bool Confirmada { get; set; }
    public bool Asistio { get; set; }
    public int DuracionMinutos { get; set; }
    public string Estado { get; set; }
}
```

---

## Manejo de Errores - Códigos y Significados

| Código | HTTP | Significado | Solución |
|--------|------|-------------|----------|
| `INVALID_DOCTOR_ID` | 400 | ID de doctor inválido | Verifica que el ID sea > 0 |
| `DOCTOR_NOT_FOUND` | 404 | Doctor no existe en BD | Registra al doctor primero |
| `DOCTOR_EXISTS` | 409 | Doctor ya registrado | El email ya está en uso |
| `NO_DOCTOR_ID` | 400 | No hay ID en caché | Debe registrarse/loguear primero |
| `NOT_FOUND` | 404 | Recurso no encontrado | Verifica que los datos existan |
| N/A | 500 | Error interno servidor | Intenta más tarde, contacta admin |
| N/A | 503 | Servidor no disponible | Intenta en unos momentos |

---

## Configuración en MauiProgramExtensions

El servicio se registra automáticamente en la inyección de dependencias:

```csharp
#if DEBUG
    // En DEBUG: Usa MockDoctorService para testing sin backend real
    serviceCollection.AddScoped<IDoctorService, MockDoctorService>();
#else
    // En RELEASE: Usa DoctorService real que consume la API
    serviceCollection.AddScoped<IDoctorService, DoctorService>();
#endif
```

---

## Endpoints de la API Consumida

El `DoctorService` realiza peticiones a estos endpoints de tu ASP.NET Core:

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/doctores/registrar` | Registrar nuevo doctor |
| GET | `/api/doctores/{id}` | Obtener datos de un doctor |
| PUT | `/api/doctores/{id}` | Actualizar datos del doctor |
| GET | `/api/doctores/{id}/citas` | Obtener citas del doctor |

**Nota:** La URL base se configura en `appsettings.json` o como variable de entorno.

---

## Flujo Completo: Desde Registro hasta Obtener Citas

```csharp
// PASO 1: Registrar nuevo doctor
var nuevoDoctor = await _doctorService.RegistrarDoctorAsync(new DoctorRequestDto
{
    Nombre = "Juan",
    Apellido = "García",
    Especialidad = "Pediatría"
});

// PASO 2: Cachear su ID (automático en RegistrarDoctorAsync)
// El ID se guarda internamente: _cachedDoctorId = nuevoDoctor.Id

// PASO 3: Cargar sus citas desde la BD real
var citas = await _doctorService.GetCitasByDoctorIdAsync(nuevoDoctor.Id);

// PASO 4: Mostrar en la UI
foreach (var cita in citas)
{
    // Bindear a UI
}

// PASO 5: Obtener datos actualizados del doctor
var doctorActual = await _doctorService.ObtenerDoctorActualAsync();
Console.WriteLine($"Consultorio: {doctorActual.Consultorio}");

// PASO 6: Actualizar perfil si es necesario
await _doctorService.ActualizarDoctorAsync(nuevoDoctor.Id, new DoctorRequestDto
{
    Nombre = "Juan Carlos",
    Apellido = "García López",
    Especialidad = "Pediatría y Neonatología"
});
```

---

## Testing con Mock Data

En modo DEBUG, `MockDoctorService` proporciona datos de prueba sin conectar a la API real:

```csharp
#if DEBUG
    var mockCitas = await _doctorService.GetCitasByDoctorIdAsync(100);
    // Retorna 2 citas mock para testing
#endif
```

---

## Validaciones Implementadas

El servicio valida automáticamente (FluentValidation):

- ✅ **Nombre**: Obligatorio, 2-100 caracteres
- ✅ **Apellido**: Obligatorio, 2-100 caracteres
- ✅ **Especialidad**: Obligatorio, 5-50 caracteres
- ✅ **Email**: Formato válido (si se proporciona)
- ✅ **Teléfono**: Formato válido (si se proporciona)

Si la validación falla, se lanza `ValidationException` con detalles de cada error.

---

## Conclusión

`DoctorService` proporciona una interfaz robusta y profesional para todas las operaciones relacionadas con doctores. Sus características enterprise-grade garantizan que tu aplicación MAUI pueda:

✅ Consumir correctamente tu API ASP.NET Core  
✅ Manejar todos los códigos de error HTTP importantes  
✅ Validar datos antes de enviarlos  
✅ Cachear IDs para acceso rápido  
✅ Testear sin backend real (modo DEBUG)  
✅ Proporcionar mensajes de error claros al usuario

¡Listo para usar en producción! 🚀
