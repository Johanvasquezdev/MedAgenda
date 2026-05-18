# 📋 FLUJO DE REGISTRO Y AUTENTICACIÓN DE DOCTORES

## **Descripción General**

El sistema ahora permite que nuevos doctores se registren en la aplicación. Una vez registrados, obtienen un ID único de la base de datos y pueden acceder a los módulos de gestión.

---

## **🔄 FLUJO COMPLETO**

### **1. Registro Inicial**
```
RegistroDoctorPage (UI)
    ↓
RegistroDoctorViewModel
    ↓
IDoctorService.RegistrarDoctorAsync(DoctorRequestDto)
    ↓
POST /api/doctores/registrar
    ↓
Base de Datos → Genera ID único
    ↓
Respuesta: DoctorResponseDto (con ID)
    ↓
DoctorService cachea el ID
    ↓
Navega a DashboardPage (acceso a módulos)
```

### **2. Acceso a Módulos**
```
DashboardPage, GestionDisponibilidadPage, etc.
    ↓
Recuperan doctorId desde DoctorService
    ↓
CitasService.ObtenerCitasDelDiaAsync()
DisponibilidadService.ObtenerDisponibilidadesAsync()
    ↓
Endpoints filtraban por doctorId automáticamente
```

---

## **📦 ARCHIVOS CREADOS**

### **DTOs**
- ✅ `DoctorRequests.cs` - DoctorRequestDto, DoctorLoginRequestDto
- ✅ `DoctorResponses.cs` - DoctorResponseDto, DoctorLoginResponseDto

### **Servicios**
- ✅ `DoctorService.cs` - Implementación real
- ✅ `MockDoctorService.cs` - Mock para testing

### **Validadores**
- ✅ `DoctorValidators.cs` - RegistrarDoctorValidator, DoctorLoginValidator

### **ViewModels & Pages**
- ✅ `RegistroDoctorViewModel.cs` - Lógica de registro
- ✅ `RegistroDoctorPage.xaml/xaml.cs` - Página de registro

### **Interfaces**
- ✅ `IDoctorService` - Contrato en IServiceInterfaces.cs

---

## **🔐 VALIDACIONES IMPLEMENTADAS**

### **Nombre**
- ✅ No vacío
- ✅ Mínimo 2 caracteres
- ✅ Máximo 100 caracteres

### **Apellido**
- ✅ No vacío
- ✅ Mínimo 2 caracteres
- ✅ Máximo 100 caracteres

### **Especialidad**
- ✅ No vacío
- ✅ Mínimo 3 caracteres
- ✅ Máximo 100 caracteres

### **Email (Opcional)**
- ✅ Formato de email válido si se proporciona

### **Teléfono (Opcional)**
- ✅ Formato de teléfono válido si se proporciona (10+ dígitos)

---

## **📡 ENDPOINTS CONSUMIDOS**

### **POST /api/doctores/registrar**
**Request:**
```json
{
  "nombre": "Juan",
  "apellido": "Pérez García",
  "especialidad": "Cardiología",
  "email": "juan@hospital.com",
  "telefono": "+34 600 123 456"
}
```

**Response (201 Created):**
```json
{
  "id": 42,
  "nombre": "Juan",
  "apellido": "Pérez García",
  "especialidad": "Cardiología",
  "email": "juan@hospital.com",
  "telefono": "+34 600 123 456",
  "consultorio": "201",
  "fechaRegistro": "2024-01-15T10:30:00Z"
}
```

### **GET /api/doctores/{id}**
Obtiene datos del doctor actual usando el ID cacheado.

### **PUT /api/doctores/{id}**
Actualiza datos del doctor (Nombre, Apellido, Especialidad, etc).

---

## **💾 PERSISTENCIA DEL ID**

El ID del doctor se cachea en `DoctorService`:

```csharp
// Después de registrar
var response = await _doctorService.RegistrarDoctorAsync(datos);
// El ID se cachea automáticamente: _cachedDoctorId = response.Id

// Para recuperarlo después
int? doctorId = _doctorService.ObtenerDoctorIdCacheado();
_doctorService.EstablecerDoctorId(doctorId); // Después de login
```

---

## **🔌 MODO DEBUG vs RELEASE**

### **DEBUG (Actual)**
- Usa `MockDoctorService`
- Genera IDs secuenciales (100, 101, 102, ...)
- No requiere backend real
- Ideal para testing

### **RELEASE**
- Usa `DoctorService` real
- Consume `POST /api/doctores/registrar`
- Requiere backend ASP.NET Core en `https://localhost:5001`

---

## **🎯 FLUJO DE USUARIO**

1. **Pantalla de Login/Registro**
   - Usuario elige: Registrar nuevo doctor

2. **Página RegistroDoctorPage**
   - Ingresa: Nombre, Apellido, Especialidad
   - Ingresa opcionales: Email, Teléfono
   - Click "Registrar Doctor"

3. **Validación (Client-side)**
   - FluentValidation valida campos
   - Muestra errores si hay (Ej: "Nombre muy corto")

4. **Envío a API**
   - `POST /api/doctores/registrar`
   - Envía datos en JSON

5. **Respuesta del Servidor**
   - Si es exitosa (201): Doctor ID generado
   - Si es conflicto (409): "Doctor ya existe"
   - Si hay validación (400): Muestra error específico

6. **Acceso a Módulos**
   - DoctorService cachea el ID
   - Navega a DashboardPage
   - Módulos usan el ID para filtrar datos

---

## **🚨 MANEJO DE ERRORES**

| Código HTTP | Mensaje | Acción |
|---|---|---|
| **400** | Validación | Muestra error específico del campo |
| **409** | Duplicado | "Este doctor ya está registrado" |
| **500** | Servidor | "Error interno del servidor" |
| **503** | No disponible | "Servidor no disponible. Intenta más tarde" |
| **Timeout** | Red | "Error de conexión al servidor" |

---

## **📝 EJEMPLO DE USO EN CÓDIGO**

```csharp
// Inyectar en ViewModel
private readonly IDoctorService _doctorService;

public RegistroDoctorViewModel(IDoctorService doctorService)
{
    _doctorService = doctorService;
}

// Registrar doctor
var request = new DoctorRequestDto
{
    Nombre = "Juan",
    Apellido = "Pérez",
    Especialidad = "Cardiología"
};

var response = await _doctorService.RegistrarDoctorAsync(request);
// response.Id = 42 (ID generado por base de datos)

// Usar ID en otros servicios
var citas = await _citasService.ObtenerCitasDelDiaAsync();
// CitasService usa internamente: doctorId = 42
```

---

## **✅ CHECKLIST**

- ✅ DoctorService implementado
- ✅ DTOs creados (Request/Response)
- ✅ Validadores creados
- ✅ RegistroDoctorPage creada
- ✅ RegistroDoctorViewModel creado
- ✅ MockDoctorService implementado
- ✅ Registrado en DI (DEBUG y RELEASE)
- ✅ Build exitoso
- ✅ Manejo robusto de errores HTTP
- ✅ Caching de ID del doctor

---

## **🔗 PRÓXIMOS PASOS (Opcionales)**

1. **LoginPage mejorada**
   - Agregar opción: "¿Nuevo doctor? Regístrate aquí"
   - Link a RegistroDoctorPage

2. **Persistencia de ID**
   - Guardar doctorId en SecureStorage
   - Recuperar al iniciar app

3. **Recuperación de contraseña**
   - Endpoint: `POST /api/auth/forgot-password`

4. **Actualización de perfil**
   - Página para editar datos del doctor
   - Usa `ActualizarDoctorAsync()`

---

**¡Sistema de registro de doctores completamente implementado! 🎉**
