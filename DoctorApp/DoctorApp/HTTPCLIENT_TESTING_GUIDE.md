# 🧪 GUÍA DE TESTING: HttpClient y ApiClient en MAUI

## Cómo verificar que tu configuración HttpClient está funcionando correctamente

---

## 🎯 TEST 1: Verificar que se Use ApiClient (no Mock)

### Verificar en el Output Window
```
1. Abre: View → Output (o Ctrl+Alt+O)
2. En el Dropdown, selecciona: "Debug"
3. Ejecuta la app
4. Busca estos mensajes:

✅ CORRECTO:
[MauiProgramExtensions] HttpClient configurado con URL base: http://localhost:5189/api
[MauiProgramExtensions] ✅ Servicios reales registrados (ApiClient, no Mock)

❌ INCORRECTO:
(No aparecen estos mensajes)
(Aparecen mensajes de Mock)
```

### Código para Verificar
```csharp
// En DashboardViewModel.cs, agrega esto temporalmente:
public DashboardViewModel(...)
{
    var apiClientType = _citasService.GetType().Name;
    System.Diagnostics.Debug.WriteLine($"[DEBUG] CitasService es: {apiClientType}");
    // Debe ser: CitasService (no MockCitasService)
}
```

---

## 🔗 TEST 2: Verificar URL Base Correcta

### Usando Fiddler
```
1. Descarga Fiddler (https://www.telerik.com/fiddler)
2. Abre Fiddler
3. Ejecuta tu app MAUI
4. En Fiddler, busca requests HTTP
5. Verifica la URL:

✅ CORRECTO:
GET http://localhost:5189/api/citas/... HTTP/1.1
GET http://localhost:5189/api/disponibilidad/... HTTP/1.1

❌ INCORRECTO:
GET https://localhost:5001/... (URL vieja)
GET http://mockserver/... (Mock)
```

### Usando VS Network Debugging
```
1. En Visual Studio: Debug → Windows → Network Debugging
2. Ejecuta app
3. Verifica URLs en la ventana de Network
4. Deben ser: http://localhost:5189/api/...
```

---

## 🔐 TEST 3: Verificar Autenticación JWT

### Verificar Token en Request
```
En Fiddler:
1. Encuentra un request HTTP
2. Haz click para verlo
3. Espera a la pestaña "Headers"
4. Busca:

✅ CORRECTO:
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

❌ INCORRECTO:
(No aparece Authorization header)
(Aparece: Authorization: Bearer null)
```

### Debug de TokenManager
```csharp
// En TokenManager.cs, agrega logging:
public bool GuardarToken(string token)
{
    System.Diagnostics.Debug.WriteLine($"[TokenManager] Guardando token: {token.Substring(0, 30)}...");
    // ... código ...
    System.Diagnostics.Debug.WriteLine($"[TokenManager] Token guardado exitosamente");
}

public string? ObtenerToken()
{
    var token = SecureStorage.GetAsync("authToken").Result;
    System.Diagnostics.Debug.WriteLine($"[TokenManager] Token obtenido: {token?.Substring(0, 30)}...");
    return token;
}
```

---

## 📊 TEST 4: Verificar Datos Reales vs Mock

### Comparar Output
```csharp
// Mock data (hardcodeado):
[
    { Id: 1, Paciente: "Carlos López", Motivo: "Revisión general" },
    { Id: 2, Paciente: "María González", Motivo: "Seguimiento" }
]

// Datos Reales (de BD):
[
    { Id: 1001, Paciente: "Juan Pérez", Motivo: "Consulta inicial" },
    { Id: 1002, Paciente: "Ana García", Motivo: "Control presión" }
]

Si ves datos que cambian según la BD → ✅ Usando API real
Si siempre ves los mismos datos → ❌ Usando Mock
```

### Test Code
```csharp
public async void TestearDatosReales()
{
    var citasService = ServiceProvider.GetRequiredService<ICitasService>();
    var citas = await citasService.ObtenerCitasDelDiaAsync();

    System.Diagnostics.Debug.WriteLine($"[TEST] Citas obtenidas: {citas.Count}");
    foreach (var cita in citas)
    {
        System.Diagnostics.Debug.WriteLine($"  - ID: {cita.Id}, Paciente: {cita.PacienteNombre}");
    }

    // Si muestra datos diferentes cada vez → ✅ REAL
    // Si siempre muestra lo mismo → ❌ MOCK
}
```

---

## 🔄 TEST 5: Verificar Flujo Completo

### Scenario: Cargar citas de hoy
```
1. Ejecuta app
2. Abre DashboardPage
3. Espera a que cargue
4. En Output window debes ver:

[ApiClient] GET /citas
[ApiClient] Response: 200 OK
[DashboardViewModel] Citas cargadas: 3

Si ves "404 Not Found":
  → Endpoint no existe en backend
  → Verifica URL en backend

Si ves "500 Server Error":
  → Error en backend
  → Revisa logs del servidor

Si ves "Connection refused":
  → Backend no está corriendo
  → Inicia: dotnet run en backend
```

---

## 📋 TEST CHECKLIST

### Configuración HttpClient
- [ ] URL base es `http://localhost:5189/api` (no `https://localhost:5001`)
- [ ] IApiClient = `ApiClient` (no `MockApiClient`)
- [ ] Servicios = Reales (no Mock)
- [ ] Compilación exitosa sin errores

### Verificación de Ejecución
- [ ] Aparecen mensajes en Output window
- [ ] No aparecen datos hardcodeados de Mock
- [ ] Datos cambian según BD
- [ ] Requests aparecen en Fiddler con URL correcta

### Autenticación
- [ ] Headers incluyen "Authorization: Bearer ..."
- [ ] Token es válido (no vacío o "null")
- [ ] Login funcionó correctamente

### Manejo de Errores
- [ ] Error 404 → Mensaje claro "Recurso no encontrado"
- [ ] Error 401 → Mensaje "No autenticado"
- [ ] Error 500 → Mensaje "Error en servidor"
- [ ] No hay crashes, solo AlertDialogs

---

## 🔧 TROUBLESHOOTING AVANZADO

### Problema: "TypeError: Cannot read property 'BaseAddress' of null"
```
Causa: HttpClient no se registró correctamente
Solución:
  1. Verifica RegisterApiServices()
  2. Asegúrate de: builder.Services.AddSingleton(httpClient);
  3. Recompila: Ctrl+Shift+B
```

### Problema: "Connection refused"
```
Causa: Backend no está corriendo en puerto 5189
Solución:
  1. Abre terminal: cd tu-backend
  2. Ejecuta: dotnet run
  3. Verifica que diga: "Now listening on: http://localhost:5189"
```

### Problema: "401 Unauthorized"
```
Causa: Token JWT no es válido o expiró
Solución:
  1. Primero debes hacer login
  2. En LoginViewModel, ejecuta: await _authService.LoginAsync(...)
  3. Esto guarda el token
  4. Luego otros requests funcionarán
```

### Problema: "Aún veo datos Mock"
```
Causa: Estás usando MockCitasService en lugar de CitasService
Solución:
  1. Abre MauiProgramExtensions.cs
  2. Verifica que NO haya:
     #if DEBUG
         AddScoped<ICitasService, MockCitasService>()
  3. Debe ser:
     AddScoped<ICitasService, CitasService>()
  4. Recompila: Ctrl+Shift+B
```

---

## 📊 VERIFICACIÓN CON HERRAMIENTAS

### Option 1: Fiddler (Recomendado)
```
Instalación:
1. https://www.telerik.com/fiddler
2. Descarga e instala
3. Abre Fiddler
4. Ejecuta tu app
5. Todos los HTTP requests aparecerán en Fiddler

Qué buscar:
- URL: http://localhost:5189/api/...
- Status: 200 (éxito) o 4xx/5xx (error)
- Headers: Authorization: Bearer ...
- Response: JSON con datos reales
```

### Option 2: Postman
```
Configuración:
1. Descarga Postman
2. Create new Request
3. Method: GET
4. URL: http://localhost:5189/api/citas
5. Tab: Headers
   Key: Authorization
   Value: Bearer <tu_token_aqui>
6. Send

Si obtienes JSON con datos reales:
  → ✅ Backend y API funcionan
  → ✅ HttpClient también debe funcionar
```

### Option 3: Visual Studio Network Debugging
```
Pasos:
1. Debug → Windows → Network Debugging
2. Ejecuta app
3. Haz acciones que generen requests
4. Verifica en ventana de Network
5. Busca:
   - URL: http://localhost:5189/api/...
   - Status: 200
   - Headers con Authorization
```

---

## 📈 TEST REPORT

Crea este reporte después de verificar:

```
┌─────────────────────────────────────┐
│ HTTP CLIENT TEST REPORT             │
├─────────────────────────────────────┤
│ URL Base Correcto: ✅               │
│ ApiClient (no Mock): ✅             │
│ JWT Token Inyectado: ✅             │
│ Datos desde BD Real: ✅             │
│ Sin Datos Hardcodeados: ✅          │
│ Manejo de Errores: ✅              │
│ Compilación: ✅ (sin errores)       │
├─────────────────────────────────────┤
│ RESULTADO: ✅ LISTO PARA PRODUCCIÓN │
└─────────────────────────────────────┘
```

---

## 🎯 CASOS DE TEST

### Test Case 1: Login
```
1. Ejecuta LoginPage
2. Ingresa credenciales válidas
3. Presiona "Login"
4. En Output debes ver:
   [AuthService] Login exitoso
   [TokenManager] Token guardado
5. Resultado: ✅ Token en SecureStorage
```

### Test Case 2: Obtener Citas
```
1. Después de login, abre DashboardPage
2. Verifica que cargue citas
3. En Output debes ver:
   [ApiClient] GET /citas
   [ApiClient] Response: 200
4. En Fiddler:
   URL: http://localhost:5189/api/citas
   Headers: Authorization: Bearer ...
5. Resultado: ✅ Citas cargadas desde BD real
```

### Test Case 3: Error Handling
```
1. Apaga el backend
2. Intenta cargar citas
3. Debes ver:
   - No crash
   - AlertDialog: "Error de conexión"
   - En Output: "[ApiClient] Error: Connection refused"
4. Resultado: ✅ Manejo de errores correcto
```

### Test Case 4: Cambiar Doctor
```
1. En DashboardPage, cambia de doctor
2. Citas deben actualizarse
3. En Output:
   [DashboardViewModel] Doctor seleccionado: Juan García
   [ApiClient] GET /doctores/1/citas
   [ApiClient] Response: 200
4. En Fiddler:
   URL: http://localhost:5189/api/doctores/1/citas
5. Resultado: ✅ Citas del nuevo doctor cargadas
```

---

## ✅ CONCLUSIÓN

Para verificar que tu HttpClient está configurado correctamente:

1. ✅ Verifica Output Window para mensajes de configuración
2. ✅ Usa Fiddler para ver requests HTTP reales
3. ✅ Compara datos con BD (deben ser iguales)
4. ✅ Prueba cambio de doctor, búsqueda, etc.
5. ✅ Verifica que no haya errores de conexión

Si todo paso estos tests → ✅ **LISTO PARA PRODUCCIÓN**

---

## 📞 DEBUGGING RÁPIDO

```csharp
// Agrega esto temporalmente para debugging:

System.Diagnostics.Debug.WriteLine("=== INFORMACIÓN DE CONFIGURACIÓN ===");
var apiClient = ServiceProvider.GetRequiredService<IApiClient>();
System.Diagnostics.Debug.WriteLine($"IApiClient: {apiClient.GetType().Name}");

var citasService = ServiceProvider.GetRequiredService<ICitasService>();
System.Diagnostics.Debug.WriteLine($"ICitasService: {citasService.GetType().Name}");

// Debe mostrar:
// IApiClient: ApiClient (no MockApiClient)
// ICitasService: CitasService (no MockCitasService)
```

¡**Listo para verificar!** 🚀
