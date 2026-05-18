# 🏥 Sistema de Gestión de Citas Médicas (SGCM) - Frontend del Doctor

## 📋 Descripción General

Este es el frontend de escritorio de la aplicación **SGCM** (Sistema de Gestión de Citas Médicas) destinado a los médicos. Permite la gestión integral de citas, disponibilidad y pacientes de forma centralizada y auditada.

**Tecnología**: .NET MAUI (.NET 10) para desktop (WinUI)

---

## 🎯 Funcionalidades Principales

### 1. **Dashboard (Panel de Control)**
- Visualización rápida de estadísticas del día
- Contador de citas totales, confirmadas y pendientes
- Selector de fecha para ver citas en diferentes días
- Listado en tiempo real de citas del día actual
- Botón de actualización manual

**Ruta**: `/DashboardPage`

### 2. **Gestión de Disponibilidad** ⭐ DESTACADO
La sección más importante para la gestión de citas. Permite al doctor definir:

#### Crear/Editar Disponibilidad Regular:
- **Día de la Semana**: Selector de día (Lunes a Domingo)
- **Hora de Inicio**: TimePicker para definir cuándo inicia
- **Hora de Fin**: TimePicker para definir cuándo termina
- **Duración Consulta**: Stepper con rango 15-120 minutos (incrementos de 5)
- **Estado**: Toggle para marcar como disponible/no disponible
- **Botones**: Guardar y Limpiar

#### Gestionar Disponibilidades Registradas:
- Vista lista de todas las disponibilidades configuradas
- Seleccionar una para editar
- Eliminar disponibilidades
- Feedback visual con colores según estado

#### Disponibilidades Especiales:
- Configurar fechas especiales (vacaciones, congresos, etc.)
- Diferentes horarios para días específicos

**Ruta**: `/GestionDisponibilidadPage`

### 3. **Gestión de Citas**
- Filtrar citas por estado (Pendiente, Confirmada, Completada, etc.)
- Filtrar por fecha
- Buscar paciente por nombre o cédula
- Actualizar estado de cita:
  - ✅ Confirmar
  - ✔️ Completar
  - ✕ Cancelar
- Visualizar información del paciente
- Historial y trazabilidad

**Ruta**: `/GestionCitasPage`

### 4. **Login** (Futuro)
- Autenticación segura del doctor
- Recordar sesión
- Recuperación de contraseña

**Ruta**: `/LoginPage`

---

## 🏗️ Estructura del Proyecto

```
DoctorApp/
├── Models/                          # Modelos de dominio
│   ├── Paciente.cs
│   ├── Medico.cs
│   ├── Cita.cs
│   ├── EstadoCita.cs
│   └── DiasemanaDisponible.cs
│
├── ViewModels/                      # Lógica de presentación
│   ├── BaseViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── GestionDisponibilidadViewModel.cs
│   ├── GestionCitasViewModel.cs
│   └── LoginViewModel.cs
│
├── Views/                           # Interfaz de usuario (XAML)
│   ├── DashboardPage.xaml
│   ├── DashboardPage.xaml.cs
│   ├── GestionDisponibilidadPage.xaml
│   ├── GestionDisponibilidadPage.xaml.cs
│   ├── GestionCitasPage.xaml
│   ├── GestionCitasPage.xaml.cs
│   ├── LoginPage.xaml
│   └── LoginPage.xaml.cs
│
├── Converters/                      # Conversores de datos
│   └── ValueConverters.cs
│
├── AppShell.xaml                    # Shell principal con navegación
├── App.xaml                         # Recursos globales
└── MauiProgramExtensions.cs         # Configuración de DI
```

---

## 🎨 Diseño Visual

### Paleta de Colores
- **Primario**: `#3B82F6` (Azul) - Acciones principales
- **Éxito**: `#10B981` (Verde) - Estados positivos
- **Advertencia**: `#F59E0B` (Amarillo/Naranja) - Pendiente
- **Error**: `#EF4444` (Rojo) - Cancelado
- **Neutro**: `#6B7280` (Gris) - Texto secundario
- **Fondo**: `#F5F7FA` (Gris claro) - Fondo principal

### Componentes UI
- **Frames**: Tarjetas redondeadas con sombra
- **Badges**: Para estados y etiquetas
- **CollectionView**: Para listados optimizados
- **Picker**: Para seleccionar opciones
- **TimePicker**: Para hora
- **DatePicker**: Para fechas
- **Stepper**: Para incrementar/decrementar valores

---

## 📊 Modelos de Datos

### Paciente
```csharp
public class Paciente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Cedula { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Genero { get; set; }
    public string Direccion { get; set; }
}
```

### DisponibilidadDiaria
```csharp
public class DisponibilidadDiaria
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DiaSemana Dia { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public int DuracionConsultaMinutos { get; set; }
    public bool Disponible { get; set; }
}
```

### Cita
```csharp
public class Cita
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public DateTime FechaHora { get; set; }
    public int DuracionMinutos { get; set; }
    public EstadoCita Estado { get; set; }
    public string Motivo { get; set; }
    public bool Confirmada { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
}
```

---

## 🔄 Estados de Cita

| Estado | Color | Descripción |
|--------|-------|-------------|
| Pendiente | 🟡 Amarillo | Cita sin confirmar |
| Confirmada | 🟢 Verde | Cita confirmada por doctor |
| En Curso | 🔵 Azul | En atención actual |
| Completada | ⚫ Gris | Atención finalizada |
| Cancelada | 🔴 Rojo | Cancelada |
| Reprogramada | 🟣 Púrpura | Reagendada |
| No Asistió | 🔴 Rojo oscuro | Paciente no asistió |

---

## 🔗 Rutas de Navegación

```
AppShell (TabBar)
├── Dashboard (/DashboardPage)
├── Disponibilidad (/GestionDisponibilidadPage)
└── Mis Citas (/GestionCitasPage)
```

---

## 💾 Datos Simulados

Actualmente, la aplicación incluye datos simulados para demostración. Cuando se integre con el backend:

1. **Cargar datos desde API**
2. **Guardar cambios en BD**
3. **Sincronización en tiempo real**
4. **Auditoría de cambios**

---

## 🎯 Características de UX

✅ **Responsive**: Diseño adaptable a diferentes tamaños
✅ **Accesible**: Etiquetas claras en todos los campos
✅ **Intuitivo**: Navegación clara y flujos lógicos
✅ **Feedback Visual**: Cambios de color y mensajes de confirmación
✅ **Eficiente**: Carga rápida, sin bloqueos
✅ **Seguro**: Confirmación antes de acciones críticas

---

## 🔐 Consideraciones de Seguridad

- ✅ Autenticación requerida (futuro)
- ✅ Autorización por rol (doctor solo ve sus datos)
- ✅ Auditoría de cambios
- ✅ Validación de entrada
- ✅ Encriptación de datos sensibles (futuro)

---

## 📱 Compatibilidad

- **Plataforma**: Windows Desktop (WinUI)
- **.NET**: 10.0
- **MAUI**: Última versión
- **Navegadores**: N/A (Aplicación de escritorio)

---

## 🚀 Próximos Pasos

1. ✅ Crear interfaz UI básica
2. ⏳ Integrar con API Backend
3. ⏳ Implementar autenticación
4. ⏳ Sincronización en tiempo real
5. ⏳ Notificaciones push
6. ⏳ Reportes y análisis
7. ⏳ Multi-idioma (ES/EN)

---

## 📝 Notas para Desarrolladores

### Para Agregar Nueva Funcionalidad:

1. Crear modelo en `/Models`
2. Crear ViewModel en `/ViewModels`
3. Crear vista XAML en `/Views`
4. Registrar en `MauiProgramExtensions.cs`
5. Agregar ruta en `AppShell.xaml`

### Cumplimiento de Reglas de Negocio:

✅ Toda cita asociada a paciente y médico
✅ No se permite agendar sin disponibilidad
✅ Los cambios quedan registrados
✅ Validación de horarios
✅ Estados bien definidos
✅ Historial íntegro

---

## 🛠️ Desarrollo Local

```bash
# Compilar
dotnet build

# Ejecutar (Windows)
dotnet maui run -f net10.0-windows

# Limpiar
dotnet clean
```

---

## 📞 Soporte

Para preguntas sobre el frontend, consulta:
- Documentación MAUI: https://learn.microsoft.com/en-us/dotnet/maui/
- Reglas de Negocio: Ver documento SGCM
- Arquitectura: Consulta con el equipo de arquitectura

---

**Última Actualización**: 2025
**Versión**: 1.0 (Beta)
**Estado**: ✅ Compilado y Funcional
