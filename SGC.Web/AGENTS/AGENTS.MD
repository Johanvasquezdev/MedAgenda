# Instrucciones Generales para Agentes (AGENTS.MD)

Este repositorio (MedAgenda - Sistema de Gestión de Citas Médicas) sigue una arquitectura limpia (Clean Architecture) con las siguientes capas:
- SGC.Domain
- SGC.Application
- SGC.Persistence
- SGC.Infrastructure
- SGC.API
- SGC.IOC
- SGC.Web (Frontend Next.js)
- SGC.Desktop (Frontend .NET MAUI)

## Reglas de diseño
- No colocar lógica de negocio en controllers ni endpoints de `SGC.API`.
- Colocar reglas de negocio base en `SGC.Domain`.
- Colocar casos de uso, DTOs, interfaces y validaciones en `SGC.Application`.
- `SGC.Infrastructure` implementa comunicación externa (Email, SMS Twilio, pagos Stripe, IA Claude, SignalR) y otros detalles técnicos.
- `SGC.Persistence` implementa el acceso a datos.
- Mantener dependencias apuntando hacia adentro (Dependency Inversion Principle).
- Usar DTOs y Mappers para contratos externos.
- Toda funcionalidad nueva debe seguir el principio de separación de responsabilidades.

## Convenciones
- `*Service` para servicios de aplicación o infraestructura cuando aplique.
- `*Repository` para implementación abstracta del acceso a la DB.
- `*Dto` para contratos y transferencia.
- Patrón MVC aplicado en el portal web (Next.js) y MVVM en la aplicación de escritorio (.NET MAUI).
- Logging estructurado gestionado vía `ISGCLogger` (Serilog).
- Manejo centralizado de errores en el pipeline de la API.

## Documentación a revisar antes de generar código
- `README.md` (Raíz del proyecto - Contexto general, actores y alcance)
- `AGENTS/CLAUDE.MD` (Instrucciones específicas para Backend y Conexión)
- `AGENTS/GEMINI.MD` (Instrucciones específicas para Diseño y Next.js)
- `AGENTS/V0.MD` (Instrucciones para generación de Componentes UI en v0)
- Documentos adicionales: SRS MedAgenda y SAD MedAgenda.

## Preferencias técnicas
- .NET 8+ Web API
- Entity Framework Core
- PostgreSQL (Supabase Cloud — solo hosting)
- Next.js (React 19) y Tailwind CSS para entorno Web
- .NET MAUI para aplicación de Escritorio (Gestión Médica)
- Autenticación Custom con tokens JWT
- SignalR para comunicación de estado y agenda en tiempo real
- Stripe API para el módulo de facturación
- API de Anthropic (Claude) para módulo Chatbot IA
