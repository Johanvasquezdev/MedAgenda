# Documento de Diseno: Capa de Presentacion (MedAgenda)

## 0. Portada
- Proyecto: MedAgenda (SGCM - Sistema de Gestion de Citas Medicas)
- Curso: Programacion II
- Profesor: Francis Ramirez
- Institucion: ITLA
- Estudiantes:
  - Johan Vasquez (2025-1235)
  - Gregori Evangelista (2025-1232)
- Fecha: 2026-04-08

## 1. Introduccion y Stack Tecnologico
Este documento describe la arquitectura, integracion y consumo de la capa de presentacion del proyecto MedAgenda. La capa web esta desarrollada con Next.js 14 (App Router) y React, con una experiencia de usuario moderna mediante Tailwind CSS. La capa de escritorio utiliza .NET MAUI bajo el patron MVVM. Ambas capas consumen de forma segura la API en .NET 8 Web API usando JWT.

### 1.1 Tecnologias Web
- Next.js 14 (App Router)
- React + TypeScript
- Tailwind CSS
- Axios
- Lucide Icons
- Sonner (notificaciones UI)

### 1.2 Tecnologias Desktop
- .NET MAUI
- MVVM (ViewModel + Commands)
- HttpClient + IHttpClientFactory
- SignalR (tiempo real)

## 2. Alcance
La presente documentacion cubre unicamente la capa de presentacion (Web y Desktop). El backend, la base de datos y la infraestructura se describen solo desde la perspectiva de consumo.

## 3. Arquitectura Logica y Separacion de Responsabilidades (Web)
Para cumplir con los lineamientos del proyecto, el frontend web esta organizado en capas logicas estrictas. Queda prohibido consumir la API directamente desde componentes UI.

### 3.1 Capas internas
- Capa de Presentacion (src/app, src/components): renderizado y eventos del usuario.
- Capa de Servicios (src/services): encapsula consumo HTTP (ej. CitaService, AuthService).
- Capa de Transferencia (src/types): DTOs TypeScript equivalentes a modelos del backend.
- Capa de Integracion (src/lib/api.ts): instancia Axios con JWT.

### 3.2 Diagrama de Arquitectura Logica (Web)
[Insertar diagrama web]

## 4. Diseno de Integracion y Estrategia de Consumo (Web)
La comunicacion se realiza via HTTP/HTTPS con JSON.

- Instancia Axios centralizada: `src/lib/api.ts`
- Interceptor JWT: agrega `Authorization: Bearer <token>`
- Servicios desacoplados: la UI invoca `CitaService.crearCita(dto)` y similares
- DTOs tipados: payloads consistentes con el backend

## 5. Manejo de Errores y Validaciones (Web)
Se aplica validacion en doble capa:

- Frontend: validaciones en formularios (required, formato, longitud).
- Backend: validadores de dominio.
- Manejo HTTP: 400/401 se muestran como feedback sin romper UI.
- Loading: uso de spinners o skeletons para UX consistente.

## 6. Arquitectura Logica: Aplicacion de Escritorio (.NET MAUI)
El modulo para Medicos se desarrollo con .NET MAUI para experiencia nativa. Se aplica MVVM:

- Views (XAML): UI sin logica de negocio.
- ViewModels: estado y comandos, notificacion de cambios.
- Servicios HttpClient: consumo de API con JWT via DI e IHttpClientFactory.

### 6.1 Diagrama de Arquitectura Logica (MAUI - MVVM)
[Insertar diagrama MAUI]

### 6.2 Estrategia de Consumo y Tiempo Real
La app desktop consume la misma API .NET 8 con DTOs compartidos. Integra SignalR para actualizaciones en tiempo real (CitaHub).

## 7. Arquitectura Fisica (Despliegue)
Componentes principales:
- Cliente Web (navegador) -> Next.js
- Cliente Desktop -> .NET MAUI
- Backend -> .NET 8 Web API
- Base de Datos -> PostgreSQL (hosting Supabase)
- Servicios externos -> Stripe, Claude, SMTP, Twilio, Redis, SignalR

### 7.1 Diagrama de Despliegue
[Insertar diagrama fisico]

## 8. Estructura de Carpetas (Web)
- `src/app`: rutas y layouts
- `src/components`: UI y componentes reutilizables
- `src/services`: consumo de API
- `src/types`: DTOs TypeScript
- `src/lib`: utilidades (Axios, JWT, helpers)

## 9. Seguridad y Autenticacion
- JWT en header `Authorization`
- Token guardado en localStorage
- Rutas protegidas por rol (Administrador/Paciente)

## 10. UX y Estilos
- Glassmorphism sutil
- Modo oscuro y claro
- Feedback visual (toasts, badges)
- Carga con spinners o skeletons

## 11. Flujos funcionales
### 11.1 Paciente (Web)
- Registro / Login
- Buscar Medicos
- Agendar / Cancelar / Reprogramar cita
- Ver Mis Citas
- Pagos
- Notificaciones
- Chatbot

### 11.2 Administrador (Web)
- CRUD Usuarios
- CRUD Medicos
- CRUD Especialidades
- CRUD Proveedores
- Auditoria
- Pagos

### 11.3 Medico (Desktop)
- Agenda diaria
- Confirmar / iniciar / completar citas
- Disponibilidad
- Historial pacientes

## 12. Estado actual y pendientes
Estado al 2026-04-08:
- Web: conectado a API, JWT, CRUD admin, flujo paciente funcional.
- Desktop: consumo de API y tiempo real via SignalR.
- Pendiente: validaciones avanzadas con Zod/React Hook Form (si se decide).

## 13. Evidencias
- Capturas de pantalla de dashboard paciente y admin
- Capturas de formularios
- Diagramas actualizados
