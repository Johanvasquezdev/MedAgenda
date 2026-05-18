# 🎨 Prototipo Visual de Pantallas

## PANTALLA 1: Dashboard

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃ 🏥 SGCM - Sistema de Gestión Médica    ┃
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
┃ [📊] [📅] [📋]                         ┃  ← Tabs
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
┃                                        ┃
┃ Bienvenido, Dr. Juan Pérez             ┃
┃ Cardiología - Consultorio 201          ┃
┃ Jueves, 16 Enero 2025                  ┃
┃                                        ┃
┃ ┌──────────────┬──────────────┬──────┐ ┃
┃ │     3        │      3       │  0   │ ┃
┃ │ Citas Hoy    │ Confirmadas  │Pend. │ ┃
┃ └──────────────┴──────────────┴──────┘ ┃
┃                                        ┃
┃ Seleccionar Fecha:                     ┃
┃ [16/01/2025 ▼]                         ┃
┃                                        ┃
┃ Citas de Hoy:                          ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ 09:00  │ Carlos López            ✓│ ┃
┃ │ 30min  │ Revisión general         │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ 10:00  │ María González          ⏳│ ┃
┃ │ 45min  │ Seguimiento             │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ 11:00  │ Juan Ramírez            ✓│ ┃
┃ │ 20min  │ Electrocardiograma      │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ [🔄 Actualizar]                        ┃
┃                                        ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
```

---

## PANTALLA 2: Gestión de Disponibilidad ⭐

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃ 🏥 SGCM - Sistema de Gestión Médica    ┃
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
┃ [📊] [📅] [📋]                         ┃
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
┃                                        ┃
┃ Gestión de Disponibilidad              ┃
┃ Configura tu horario de atención       ┃
┃                                        ┃
┃ ╔════════════════════════════════════╗ ┃
┃ ║ Crear Nueva Disponibilidad         ║ ┃
┃ ╠════════════════════════════════════╣ ┃
┃ ║                                    ║ ┃
┃ ║ Día de la Semana:                  ║ ┃
┃ ║ [Lunes ▼]                          ║ ┃
┃ ║                                    ║ ┃
┃ ║ Hora de Inicio:                    ║ ┃
┃ ║ [08:00 ⏰]                          ║ ┃
┃ ║                                    ║ ┃
┃ ║ Hora de Fin:                       ║ ┃
┃ ║ [12:00 ⏰]                          ║ ┃
┃ ║                                    ║ ┃
┃ ║ Duración Consulta (min):           ║ ┃
┃ ║ [◄] 30 min [►]                     ║ ┃
┃ ║                                    ║ ┃
┃ ║ Estado:                            ║ ┃
┃ ║ [ON] ✓ Disponible                  ║ ┃
┃ ║                                    ║ ┃
┃ ║ [💾 Guardar] [✕ Limpiar]           ║ ┃
┃ ║                                    ║ ┃
┃ ╚════════════════════════════════════╝ ┃
┃                                        ┃
┃ Disponibilidades Registradas:          ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ Lunes                              │ ┃
┃ │ ⏰ 08:00 - 12:00                    │ ┃
┃ │ ⏱️ 30 min │ ✓ Disponible           │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ Lunes                              │ ┃
┃ │ ⏰ 14:00 - 18:00                    │ ┃
┃ │ ⏱️ 30 min │ ✓ Disponible           │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ Martes                             │ ┃
┃ │ ⏰ 08:00 - 12:00                    │ ┃
┃ │ ⏱️ 30 min │ ✓ Disponible           │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ [🗑️ Eliminar Sel.]                    ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ 📅 Disponibilidades Especiales     │ ┃
┃ │ Configura fechas (vacaciones, etc.)│ ┃
┃ │ [+ Agregar Fecha Especial]         │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
```

---

## PANTALLA 3: Gestión de Citas

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃ 🏥 SGCM - Sistema de Gestión Médica    ┃
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
┃ [📊] [📅] [📋]                         ┃
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
┃                                        ┃
┃ Mis Citas Médicas                      ┃
┃ Administra y actualiza el estado       ┃
┃                                        ┃
┃ ╔════════════════════════════════════╗ ┃
┃ ║ Filtrar Citas                      ║ ┃
┃ ╠════════════════════════════════════╣ ┃
┃ ║                                    ║ ┃
┃ ║ Estado: [Confirmada ▼]             ║ ┃
┃ ║ Fecha:  [16/01/2025 ▼]             ║ ┃
┃ ║ Buscar: [Nombre o Cédula...]       ║ ┃
┃ ║                                    ║ ┃
┃ ╚════════════════════════════════════╝ ┃
┃                                        ┃
┃ 📋 3 cita(s) encontrada(s)             ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ 🕐 09:00   │ Carlos López      ✓  │ ┃
┃ │ 30min      │ Revisión general     │ ┃
┃ │────────────────────────────────────│ ┃
┃ │ 👤 Paciente: 12345678             │ ┃
┃ │ 📱 Teléfono: +34 600 123 456       │ ┃
┃ │ 📧 Email: carlos@email.com         │ ┃
┃ │────────────────────────────────────│ ┃
┃ │ [✅ Confirmar] [✔️ Completar]      │ ┃
┃ │ [✕ Cancelar]                       │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ 🕑 10:00   │ María González    ⏳  │ ┃
┃ │ 45min      │ Seguimiento cardiaco│ ┃
┃ │────────────────────────────────────│ ┃
┃ │ 👤 Paciente: 87654321             │ ┃
┃ │ 📱 Teléfono: +34 601 234 567       │ ┃
┃ │ 📧 Email: maria@email.com          │ ┃
┃ │────────────────────────────────────│ ┃
┃ │ [✅ Confirmar] [✔️ Completar]      │ ┃
┃ │ [✕ Cancelar]                       │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┃ ┌────────────────────────────────────┐ ┃
┃ │ 🕐 11:00   │ Juan Ramírez      ✓  │ ┃
┃ │ 20min      │ Electrocardiograma   │ ┃
┃ │────────────────────────────────────│ ┃
┃ │ 👤 Paciente: 11111111             │ ┃
┃ │ 📱 Teléfono: +34 602 345 678       │ ┃
┃ │ 📧 Email: juan@email.com           │ ┃
┃ │────────────────────────────────────│ ┃
┃ │ [✅ Confirmar] [✔️ Completar]      │ ┃
┃ │ [✕ Cancelar]                       │ ┃
┃ └────────────────────────────────────┘ ┃
┃                                        ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
```

---

## Paleta de Colores

```
PRIMARIO          ÉXITO           ADVERTENCIA      ERROR
┌────────────┐   ┌────────────┐   ┌────────────┐   ┌────────────┐
│ #3B82F6    │   │ #10B981    │   │ #F59E0B    │   │ #EF4444    │
│ Azul       │   │ Verde      │   │ Amarillo   │   │ Rojo       │
└────────────┘   └────────────┘   └────────────┘   └────────────┘
     Botones           ✓                ⏳              ✕
     Links         Confirmada       Pendiente      Cancelada

NEUTRO            FONDO           BORDER
┌────────────┐   ┌────────────┐   ┌────────────┐
│ #6B7280    │   │ #F5F7FA    │   │ #E5E7EB    │
│ Gris       │   │ Gris Claro │   │ Gris Claro │
└────────────┘   └────────────┘   └────────────┘
  Texto Sec.    Fondo Página      Separadores
```

---

## Estados de Cita - Visuales

```
✓ CONFIRMADA           ⏳ PENDIENTE           ✔️ COMPLETADA
┌────────────────┐   ┌────────────────┐   ┌────────────────┐
│ 🟢 Verde       │   │ 🟡 Amarillo    │   │ ⚫ Gris         │
│ Listo          │   │ Espera conf.   │   │ Terminado      │
└────────────────┘   └────────────────┘   └────────────────┘

✕ CANCELADA          🔴 NO ASISTIÓ        🔵 EN CURSO
┌────────────────┐   ┌────────────────┐   ┌────────────────┐
│ 🔴 Rojo        │   │ 🔴 Rojo Oscuro │   │ 🔵 Azul        │
│ Rechazada      │   │ No vino        │   │ En atención    │
└────────────────┘   └────────────────┘   └────────────────┘
```

---

## Flujo de Navegación

```
                    ┌─────────────────┐
                    │   AppShell      │
                    │  (TabBar)       │
                    └────┬────┬────┬──┘
                         │    │    │
         ┌───────────────┴─┐  │  ┌─┴───────────────┐
         │                 │  │  │                 │
    ┌────▼─────┐    ┌──────▼──▼──────┐    ┌────────▼────┐
    │Dashboard │    │Disponibilidad │    │   Citas    │
    │  Page    │    │     Page       │    │   Page     │
    └──────────┘    └────────────────┘    └────────────┘
         │                 │                       │
    [Tap]│            [Tap]│                  [Tap]│
         └─────────────────┴───────────────────────┘
              Vuelve al Dashboard
```

---

## Responsividad

```
LAPTOP / DESKTOP                    TABLET
┌──────────────────────────┐       ┌────────────────┐
│  Ancho completo          │       │ Ancho reducido │
│  Cards lado a lado       │       │ Cards apiladas │
│  Máximo aprovechamiento  │       │ Optimizado     │
└──────────────────────────┘       └────────────────┘
```

---

## Interactividad

```
USUARIO HACE CLIC EN...        RESULTADO

Fecha en DatePicker    →  Se abre selector de fechas
Picker (Día, Estado)   →  Se abre lista de opciones
TimePicker             →  Se abre selector de hora
Stepper (Duración)     →  Incrementa/Decrementa valor
Toggle (Disponible)    →  Cambia estado ON/OFF
Entrada de texto       →  Cursor, teclado visible
Button                 →  Ejecuta Command

VALIDACIÓN

Hora Fin < Hora Inicio  →  ❌ "La hora fin debe ser mayor"
Campo requerido vacío   →  ❌ No permite guardar
Datos válidos           →  ✅ "Guardado correctamente"
```

---

## Animaciones y Transiciones

```
✨ Binding en Tiempo Real
   Usuario escribe en TimePicker
   ↓
   ViewModel se actualiza instantáneamente
   ↓
   View refleja el cambio

✨ Cambio de Página
   Haz clic en Tab
   ↓
   Transición suave
   ↓
   Nueva página aparece

✨ CollectionView Scroll
   Listado de disponibilidades largo
   ↓
   Scroll suave sin lag
   ↓
   Performance optimizada
```

---

## Iconografía

```
Dashboard:     📊  Gráfico/Estadísticas
Disponibilidad: 📅  Calendario
Citas:         📋  Documento
Guardar:       💾  Disquete
Limpiar:       ✕   Equis
Eliminar:      🗑️  Basura
Confirmar:     ✅  Checkmark Verde
Completar:     ✔️  Checkmark Azul
Cancelar:      ✕   Equis Rojo
Buscar:        🔍  Lupa
Paciente:      👤  Persona
Teléfono:      📱  Celular
Email:         📧  Sobre
Hora:          🕐  Reloj
Duración:      ⏱️  Cronómetro
```

---

**Diseño Profesional** ✅  
**Colores Coherentes** ✅  
**Fácil de Navegar** ✅  
**Información Clara** ✅  
**Feedback Inmediato** ✅

