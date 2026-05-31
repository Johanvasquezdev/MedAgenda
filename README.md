# MedAgenda — Sistema de Gestion de Citas Medicas (SGCM)

Proyecto academico de Programacion II (ITLA 2026).


## Descripcion del proyecto

MedAgenda centraliza la gestion de pacientes, medicos y citas para reducir conflictos de horario, errores operativos y carga administrativa.

### Actores

- Paciente: busca medicos, agenda y gestiona sus citas.
- Medico: administra agenda, disponibilidad e historial de pacientes.
- Administrador: gestiona usuarios, especialidades, proveedores y auditoria.

### Alcance

Incluye:

- Gestion del ciclo de vida de citas.
- Gestion de disponibilidad medica.
- Gestion de pacientes, medicos y especialidades.
- Notificaciones (Email/SMS/Push).
- Auditoria.
- Pagos en linea.
- Asistente virtual con IA.

Fuera de alcance:

- Expedientes clinicos completos.

## Stack tecnologico

- Backend: .NET 8 Web API
- Web: Next.js (React)
- Desktop: .NET MAUI
- DB: PostgreSQL (Supabase como hosting)
- ORM: Entity Framework Core
- Auth: JWT
- Real-time: SignalR
- Pagos: Stripe
- Chatbot: Groq (API compatible OpenAI)
- Cache: Redis
- Notificaciones: SMTP (MailKit) y Twilio
- Logging: Serilog via `ISGCLogger`

## Estructura de la solucion

- `SGC.Domain`
- `SGC.Application`
- `SGC.Persistence`
- `SGC.Infraestructure`
- `SGC.IOC`
- `SGC.API`
- `SGC.Web`
- `DoctorApp`
- `SGC.ApplicationTest`

## Division de trabajo (evaluacion)

### Johan Vasquez (2025-1235)

- Portal Paciente (registro/login, busqueda medicos, agendamiento, mis citas, pagos, notificaciones, chatbot).
- Panel Administrador (usuarios, medicos, especialidades, proveedores, auditoria).

### Gregori Evangelista (2025-1232)

- App Medico (agenda, gestion de citas, disponibilidad, historial de pacientes, seguridad JWT).


## Documentacion

- SRS MedAgenda
- SAD MedAgenda

ITLA - 2026
