# MedAgenda — Sistema de Gestion de Citas Medicas (SGCM)

Proyecto academico de Programacion II (ITLA 2026).

## Nota sobre ramas y autoria

Las ramas sin matricula (`feature/api`, `feature/ioc`, `feature/application`, `feature/dominio`, `feature/persistencia`, `feature/unittest`) fueron trabajadas en conjunto por Johan Vasquez y Gregori Evangelista.

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

## Ramas

- `feature/web-20251235` (Web - Johan)
- `feature/Desktop-20251232` (Desktop - Gregori)
- Ramas compartidas: `feature/api`, `feature/ioc`, `feature/infraestructure`, `feature/dominio`, `feature/persistencia`, `feature/application`, `feature/unittest`

## API

Swagger UI local: `http://localhost:5189/swagger`

## Docker quickstart

1. Copy `.env.example` to `.env` and set real values.
2. Start API container:

```bash
docker compose up --build -d
```

3. Verify API is up at `http://localhost:5189/swagger`.

Notes:

- `docker-compose.yml` maps container port `8080` to host `5189`.
- Database/Redis in `.env.example` use `host.docker.internal` so local services can be used from container.
- Configure CORS origins using `SGC_CORS_ORIGIN_0`/`SGC_CORS_ORIGIN_1` (or `Cors:AllowedOrigins`).
- Outside Development, API startup fails if `Cors:AllowedOrigins` is not configured.

## Documentacion

- SRS MedAgenda
- SAD MedAgenda

ITLA - 2026
