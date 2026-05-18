# 📅 Guía: Módulo de Gestión de Disponibilidad

## 🎯 Objetivo

El módulo de **Gestión de Disponibilidad** permite al doctor definir y mantener actualizado su horario de atención médica. Es **fundamental** para que el sistema pueda agendar citas correctamente respetando la disponibilidad.

---

## ⚠️ Importancia Crítica

**La disponibilidad es la base del sistema de citas.** Sin disponibilidad definida:
- ❌ No se puede agendar citas
- ❌ El sistema no sabe cuándo está disponible
- ❌ Se generan conflictos de horarios
- ❌ Baja calidad en la gestión administrativa

---

## 🔧 Cómo Usar

### 1. Acceder al Módulo

1. Abre la aplicación SGCM Doctor
2. Haz clic en la pestaña **"Disponibilidad"** (segundo ícono)
3. Verás el formulario de crear disponibilidad y el listado

### 2. Crear Disponibilidad Ordinaria

#### Paso 1: Seleccionar Día
```
Día de la Semana: [Picker ▼]
```
- Selecciona el día de la semana (Lunes, Martes, etc.)
- Si trabajas múltiples horarios el mismo día, crea dos registros

#### Paso 2: Definir Hora de Inicio
```
Hora de Inicio: [TimePicker]
```
- Hora a la que INICIAS a atender pacientes
- Ejemplo: 08:00 (8 de la mañana)

#### Paso 3: Definir Hora de Fin
```
Hora de Fin: [TimePicker]
```
- Hora a la que TERMINAS de atender pacientes
- Ejemplo: 12:00 (12 del mediodía)
- **Debe ser mayor a la hora de inicio**

#### Paso 4: Duración de Consulta
```
Duración Consulta (minutos): [Stepper ↑↓]
```
- Tiempo que dedicas a cada paciente
- Opciones comunes: 15, 20, 30, 45, 60 minutos
- El sistema usará esto para calcular slots disponibles
- Rango: 15-120 minutos

#### Paso 5: Estado de Disponibilidad
```
Estado: [Toggle]
    ✓ Disponible  ✕ No Disponible
```
- Marca como **Disponible** si el horario está activo
- Marca como **No Disponible** para desactivar sin eliminar

#### Paso 6: Guardar
```
💾 Guardar  |  ✕ Limpiar
```
- Haz clic en **Guardar**
- Recibirás confirmación: ✅ "Disponibilidad creada correctamente"

### 3. Ejemplo Práctico

**Escenario**: Trabajas lunes y miércoles por la mañana (08:00-12:00), 
lunes y miércoles por la tarde (14:00-18:00), y viernes todo el día (08:00-17:00)

**Registros a crear:**

| Día | Inicio | Fin | Duración | Estado |
|-----|--------|-----|----------|--------|
| Lunes | 08:00 | 12:00 | 30 min | ✓ |
| Lunes | 14:00 | 18:00 | 30 min | ✓ |
| Miércoles | 08:00 | 12:00 | 30 min | ✓ |
| Miércoles | 14:00 | 18:00 | 30 min | ✓ |
| Viernes | 08:00 | 17:00 | 30 min | ✓ |

**Total: 5 registros de disponibilidad**

---

## ✏️ Editar Disponibilidad

1. **Haz clic** en cualquier disponibilidad del listado
2. Los campos se llenarán automáticamente
3. Modifica los valores necesarios
4. Haz clic en **"💾 Guardar"**
5. Recibirás confirmación: ✅ "Disponibilidad actualizada correctamente"

---

## 🗑️ Eliminar Disponibilidad

1. **Haz clic** en la disponibilidad que deseas eliminar
2. Se habilitará el botón **"🗑️ Eliminar Disponibilidad Seleccionada"**
3. Haz clic en eliminar
4. **Confirma** en el diálogo que aparecerá
5. Recibirás confirmación: ✅ "Disponibilidad eliminada correctamente"

---

## 📅 Gestionar Disponibilidades Especiales

Para fechas especiales (vacaciones, congresos, días festivos):

1. **Sección**: "📅 Disponibilidades Especiales" (parte inferior)
2. Haz clic en **"+ Agregar Fecha Especial"**
3. Configura:
   - **Fecha**: Día específico
   - **Rango horario**: (Opcional) Si quieres horario diferente
   - **Disponible**: Sí (diferente horario) o No (no trabajas)
   - **Razón**: "Congreso médico", "Vacaciones", etc.

**Ejemplos:**
- 25/12: No disponible (Navidad)
- 01/01: No disponible (Año Nuevo)
- 15/03: 10:00-13:00 disponible (Día de reunión, solo mañana)

---

## ⚙️ Cálculo Automático de Slots

Una vez definida la disponibilidad, el sistema **calcula automáticamente** los slots:

**Ejemplo:**
- Hora inicio: 08:00
- Hora fin: 12:00
- Duración: 30 minutos
- **Slots disponibles**: 08:00, 08:30, 09:00, 09:30, 10:00, 10:30, 11:00, 11:30
- **Total: 8 citas posibles en ese horario**

---

## 🔔 Consideraciones Importantes

### ✅ HACER

- ✅ Actualizar disponibilidad **regularmente**
- ✅ Usar la misma duración si es consistente (ej: siempre 30 min)
- ✅ Marcar como "No disponible" si necesitas pausar sin eliminar
- ✅ Agregar disponibilidades especiales **con anticipación**
- ✅ Revisar cambios antes de guardar

### ❌ NO HACER

- ❌ Dejar disponibilidad vacía (sin registros)
- ❌ Crear solapamientos de horario (ej: 08-12 y 10-14)
- ❌ Usar duraciones muy cortas (<15 min)
- ❌ Olvidar actualizar cuando cambias de horario
- ❌ Eliminar sin confirmar

---

## 🐛 Solución de Problemas

### Problema: "La hora de inicio debe ser menor a la hora de fin"

**Causa**: Hora inicio ≥ Hora fin
**Solución**: Verifica que `Hora Inicio < Hora Fin`

Ejemplo ❌: Inicio 14:00, Fin 12:00
Correcto ✅: Inicio 12:00, Fin 18:00

---

### Problema: No aparecen citas disponibles

**Causa**: No hay disponibilidad configurada para ese día/hora
**Solución**:
1. Ve a Gestión de Disponibilidad
2. Crea la disponibilidad para el día requerido
3. Verifica fecha y horario

---

### Problema: Aparecen menos slots de lo esperado

**Causa**: La duración no se divide exactamente en el rango
**Solución**: Ajusta duración o rango

Ejemplo ❌: 08:00-12:30 con duración 30 min
- Slots: 08:00, 08:30, 09:00, 09:30, 10:00, 10:30, 11:00, 11:30
- Slot 12:00 no cabe (solo hay 30 min)

Correcto ✅: 08:00-12:00 con duración 30 min
- Slots: 08:00, 08:30, 09:00, 09:30, 10:00, 10:30, 11:00, 11:30 ✓

---

## 📊 Casos de Uso Reales

### Caso 1: Doctor Generalista
```
Lunes-Viernes: 08:00-17:00 (duración 30 min)
Sábados: 09:00-13:00 (duración 30 min)
Domingos: No disponible
```

### Caso 2: Cardiólogo Especialista
```
Lunes: 10:00-12:00 y 15:00-18:00 (duración 45 min)
Miércoles: 10:00-12:00 (duración 45 min)
Viernes: 14:00-17:00 (duración 45 min)
Otros días: No disponible
```

### Caso 3: Doctor en Turnos
```
Turno 1 (Semana 1): Lun-Vie 08:00-16:00 (duración 30 min)
Turno 2 (Semana 2): Lun-Vie 16:00-23:00 (duración 30 min)
Especiales: Fiestas patrias no disponible
```

---

## 📱 Visualización de Disponibilidades

El listado muestra:

```
┌─────────────────────────────────────┐
│ Lunes                               │
│ ⏰ 08:00 - 12:00                     │
│ ⏱️ 30 min | ✓ Disponible            │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ Lunes                               │
│ ⏰ 14:00 - 18:00                     │
│ ⏱️ 30 min | ✓ Disponible            │
└─────────────────────────────────────┘
```

---

## 🔐 Auditoría y Cambios

Todos los cambios de disponibilidad se registran:
- ✅ Fecha/Hora de creación
- ✅ Fecha/Hora de última modificación
- ✅ Quién hizo el cambio (usuario)
- ✅ Qué cambió (valores anteriores/nuevos)

---

## 📞 Soporte

Si tienes dudas:
1. Revisa esta guía
2. Consulta el README del frontend
3. Contacta al equipo de soporte

---

**Versión**: 1.0
**Última actualización**: 2025
**Estado**: ✅ Funcional
