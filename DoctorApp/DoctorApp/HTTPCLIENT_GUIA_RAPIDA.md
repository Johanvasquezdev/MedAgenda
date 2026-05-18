# 🎯 GUÍA RÁPIDA: Configuración HttpClient - Paso a Paso

## ✅ TU CONFIGURACIÓN ESTÁ LISTA

**Estado**: ✅ Compilado  
**URL Base**: `http://localhost:5189/api`  
**Cliente**: `ApiClient` (REAL)  

---

## 🚀 5 PASOS PARA EMPEZAR

### PASO 1: Inicia tu Backend (2 min)

```bash
# En terminal (otra ventana)
cd C:\Users\user\source\repos\DoctorApp\BackendAspNetCore
# O donde esté tu backend

dotnet run
# Output debe mostrar:
# Now listening on: http://localhost:5189
# Now listening on: https://localhost:5190
```

✅ Si ves el mensaje → Backend corriendo en puerto 5189  
❌ Si no ves → Verifica puerto en launchSettings.json

---

### PASO 2: Ejecuta la App MAUI

```
En Visual Studio:
1. Presiona: Ctrl+F5 (Run without debugging)
2. O en Debug menu: Start Debugging
3. Selecciona device (Emulator o Device)
```

✅ La app debe iniciar sin errores  
❌ Si hay error → Ver Output window (Ctrl+Alt+O)

---

### PASO 3: Verifica en Output Window

```
1. Abre: View → Output (Ctrl+Alt+O)
2. En Dropdown selecciona: "Debug"
3. Busca estos mensajes:

✅ CORRECTO:
[MauiProgramExtensions] HttpClient configurado con URL base: http://localhost:5189/api
[MauiProgramExtensions] ✅ Servicios reales registrados (ApiClient, no Mock)

❌ INCORRECTO:
(No aparecen estos mensajes)
(Aparecen mensajes de Mock)
```

---

### PASO 4: Haz Login

```
1. Abre LoginPage
2. Ingresa credenciales válidas:
   - Usuario: [tu usuario]
   - Contraseña: [tu contraseña]
3. Presiona "Login"

✅ Éxito: Dirigido a DashboardPage
❌ Error: Verifica credenciales en BD
```

---

### PASO 5: Verifica Datos Reales

```
1. En DashboardPage, debes ver:
   - Doctores de verdad (no mock)
   - Citas de verdad (no hardcodeadas)
   - Estadísticas correctas

2. En Output window, busca:
   [ApiClient] GET /citas
   [ApiClient] Response: 200 OK

3. Prueba cambiar de doctor:
   - Citas deben actualizarse
   - En Output: [ApiClient] GET /doctores/{id}/citas
```

✅ Si ves datos que cambian → ApiClient real  
❌ Si ves datos siempre iguales → Aún en Mock

---

## 📊 VERIFICACIÓN CON FIDDLER

### Instalar Fiddler
```
1. Descarga: https://www.telerik.com/fiddler
2. Instala
3. Abre Fiddler
```

### Ver Requests
```
1. Ejecuta tu app
2. En Fiddler, verás requests HTTP
3. Busca:

✅ URL: http://localhost:5189/api/citas
✅ Headers: Authorization: Bearer eyJhbGc...
✅ Status: 200 OK
✅ Response: JSON con datos reales

❌ Incorrecto:
Status: 404 (endpoint no existe)
Status: 401 (token inválido)
Status: 500 (error en servidor)
```

---

## 🔐 VERIFICAR AUTENTICACIÓN JWT

### En Fiddler

```
1. Haz click en un request HTTP
2. Ve a Tab: "Headers"
3. Busca: Authorization
4. Verifica:

✅ CORRECTO:
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

❌ INCORRECTO:
Authorization: Bearer null
(No aparece Authorization)
```

### En Output Window

```csharp
// Agrega esto temporalmente en TokenManager.cs:
System.Diagnostics.Debug.WriteLine($"[DEBUG] Token: {token?.Substring(0, 50)}...");

// Debe mostrar un JWT válido (empieza con "eyJ...")
```

---

## 🧪 TEST RÁPIDO: ¿Está funcionando?

### Test 1: Verificar ApiClient Real
```csharp
// En DashboardViewModel o cualquier lugar:
var apiClient = ServiceProvider.GetService(typeof(IApiClient));
Debug.WriteLine($"ApiClient Type: {apiClient?.GetType().Name}");

// Debe mostrar: ApiClient (no MockApiClient)
```

### Test 2: Verificar URL Base
```csharp
var httpClient = ServiceProvider.GetService(typeof(HttpClient)) as HttpClient;
Debug.WriteLine($"Base URL: {httpClient?.BaseAddress}");

// Debe mostrar: http://localhost:5189/api/
```

### Test 3: Verificar Datos Reales
```csharp
var citas = await _citasService.ObtenerCitasDelDiaAsync();
foreach (var cita in citas)
{
    Debug.WriteLine($"Cita: {cita.Id} - {cita.PacienteNombre}");
}

// Debe mostrar datos diferentes cada vez (reales)
// No siempre "Carlos López" y "María González" (mock)
```

---

## ❌ TROUBLESHOOTING RÁPIDO

### Problema: "Cannot connect to http://localhost:5189"
```
Causa: Backend no está corriendo
Solución:
1. En terminal, abre: cd backend
2. Ejecuta: dotnet run
3. Verifica que diga: "Now listening on: http://localhost:5189"
```

### Problema: "401 Unauthorized"
```
Causa: Token no es válido o no existe
Solución:
1. Primero debes hacer LOGIN
2. Login guarda el token
3. Luego otros requests funcionan
```

### Problema: "404 Not Found"
```
Causa: Endpoint no existe en backend
Solución:
1. Verifica que endpoint existe en backend
2. Verifica nombre correcto: /citas, /disponibilidad, etc.
3. Verifica que es: /api/citas (no solo /citas)
```

### Problema: "Aún veo datos Mock"
```
Causa: Todavía está usando MockCitasService
Solución:
1. Abre: MauiProgramExtensions.cs
2. Verifica que NO haya:
   #if DEBUG
       AddScoped<ICitasService, MockCitasService>()
3. Debe ser:
   AddScoped<ICitasService, CitasService>()
4. Recompila: Ctrl+Shift+B
```

---

## 📋 CHECKLIST FINAL

### Antes de empezar
- [ ] Backend corriendo en puerto 5189
- [ ] Visual Studio abierto
- [ ] MAUI project visible

### Ejecución
- [ ] Ctrl+F5 para ejecutar app
- [ ] Output window muestra configuración
- [ ] Login exitoso
- [ ] DashboardPage carga citas

### Verificación
- [ ] Datos en pantalla son de verdad (no mock)
- [ ] Fiddler muestra URL correcta: http://localhost:5189/api/...
- [ ] Headers incluyen Authorization: Bearer ...
- [ ] Response tiene Status: 200 OK

### Funcionamiento
- [ ] Cambiar doctor actualiza citas
- [ ] Búsqueda de paciente funciona
- [ ] Email se muestra correctamente
- [ ] Sin errores en Output window

---

## 📺 QUÉ DEBES VER EN PANTALLA

### DashboardPage
```
┌─────────────────────────────────┐
│ Doctor seleccionado:            │
│ Dr. Manuel Gómez                │ ← Datos REALES de BD
│ Cardiología                     │
│                                 │
│ Citas de Hoy:                   │
│ • Juan Pérez - Consulta - 14:30 │ ← Datos REALES
│ • María García - Chequeo - 15:00│ ← Datos REALES
│                                 │
│ Total: 2 | Confirmadas: 1       │ ← Estadísticas actuales
│ Pendientes: 1                   │
└─────────────────────────────────┘
```

### Output Window
```
[MauiProgramExtensions] HttpClient configurado con URL base: http://localhost:5189/api
[MauiProgramExtensions] ✅ Servicios reales registrados (ApiClient, no Mock)
[ApiClient] GET /citas
[ApiClient] Response: 200 OK
[DashboardViewModel] Citas cargadas: 2
```

### Fiddler
```
GET http://localhost:5189/api/citas HTTP/1.1
Host: localhost:5189
Authorization: Bearer eyJhbGc...

← Response →
HTTP/1.1 200 OK
Content-Type: application/json

[
  { "id": 1, "pacienteNombre": "Juan Pérez", ... },
  { "id": 2, "pacienteNombre": "María García", ... }
]
```

---

## ✅ GARANTÍA DE FUNCIONAMIENTO

Si seguiste estos pasos correctamente:

✅ HttpClient apunta a `http://localhost:5189/api`  
✅ Estás usando `ApiClient` (no Mock)  
✅ Los datos vienen de BD real  
✅ JWT se inyecta automáticamente  
✅ Todo compiló sin errores  

---

## 📞 SI ALGO FALLA

### Obtén más información
1. Lee: `HTTPCLIENT_CONFIGURACION.md`
2. Lee: `HTTPCLIENT_TESTING_GUIDE.md`
3. Verifica: `HTTPCLIENT_RESUMEN_FINAL.md`

### Debugging avanzado
1. Abre Output window: Ctrl+Alt+O
2. Busca error específico
3. Usa Fiddler para ver requests HTTP
4. Verifica que backend esté corriendo

### Contacta
- Backend team si error es 500
- Database team si datos están mal
- DevOps si hay problemas de conectividad

---

## 🎉 ¡FELICITACIONES!

Si llegaste aquí y todo funciona:

✅ **HttpClient configurado**  
✅ **ApiClient real en uso**  
✅ **Datos de BD real**  
✅ **JWT automático**  
✅ **Todo funcionando**  

**¡Tu aplicación MAUI está conectada con el backend!** 🚀

---

## 🚀 SIGUIENTES PASOS

Una vez que todo funciona:

1. ✅ Prueba cambiar de doctor
2. ✅ Prueba buscar paciente
3. ✅ Prueba confirmar cita
4. ✅ Prueba crear disponibilidad
5. ✅ Agrega más funcionalidades

---

## 📚 DOCUMENTACIÓN DE REFERENCIA

| Documento | Para |
|-----------|------|
| HTTPCLIENT_RESUMEN_FINAL.md | Resumen ejecutivo |
| HTTPCLIENT_CONFIGURACION.md | Entender arquitectura |
| HTTPCLIENT_TESTING_GUIDE.md | Verificar funcionamiento |
| HTTPCLIENT_DOCUMENTACION_INDEX.md | Índice de docs |

---

## ✨ CONCLUSIÓN

Todo está configurado. Solo necesitas:

1. Backend corriendo en puerto 5189
2. Ejecutar app MAUI
3. Hacer login
4. Usar la app normalmente

**¡Listo!** 🎊

---

**Última actualización**: Hoy  
**Estado**: ✅ LISTO PARA USAR  
**URL**: http://localhost:5189/api  
**Cliente**: ApiClient (REAL)  

¡Disfruta tu integración HttpClient! 🚀
