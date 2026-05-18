using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Repositorios - Interfaces (Domain)
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Interfaces;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Services;
using SGC.Domain.Validators;

// Repositorios - Implementaciones (Persistence)
using SGC.Persistence.Context;
using SGC.Persistence.Repositories.Appointments;
using SGC.Persistence.Repositories.Audit;
using SGC.Persistence.Repositories.Catalog;
using SGC.Persistence.Repositories.Medical;
using SGC.Persistence.Repositories.Notifications;
using SGC.Persistence.Repositories.Payments;
using SGC.Persistence.Repositories.Security;

// Servicios - Interfaces (Application Contracts)
using SGC.Application.Contracts;

// Servicios - Implementaciones (Application Services)
using SGC.Application.Services;

// Infraestructura (registro centralizado)
using SGC.Infraestructure;

namespace SGC.IOC
{
    // Contenedor de inyeccion de dependencias
    public static class DependencyContainer
    {
        // Registra todos los servicios, repositorios y el DbContext del sistema
        public static IServiceCollection AddSGCDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Fix para timestamps sin timezone con Npgsql y PostgreSQL
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // ============================================================
            // 1. DbContext con PostgreSQL via Supabase
            // ============================================================
            services.AddDbContext<SGCDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("SGCConnection")));

            // ============================================================
            // 2. Repositorios (Persistence -> Domain interfaces)
            // ============================================================

            // Appointments
            services.AddScoped<ICitaRepository, CitaRepository>();
            services.AddScoped<IDisponibilidadRepository, DisponibilidadRepository>();

            // Audit
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

            // Catalog
            services.AddScoped<IEspecialidadRepository, EspecialidadRepository>();
            services.AddScoped<IProveedorSaludRepository, ProveedorSaludRepository>();

            // Medical
            services.AddScoped<IMedicoRepository, MedicoRepository>();
            services.AddScoped<IPacienteRepository, PacienteRepository>();

            // Notifications
            services.AddScoped<INotificacionRepository, NotificacionRepository>();
            services.AddScoped<IPrefNotificacionRepository, PrefNotificacionRepository>();

            // Security
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Payments
            services.AddScoped<IPagoRepository, PagoRepository>();

            // ============================================================
            // 3. Domain Services y Validators
            // ============================================================
            services.AddScoped<CitaDomainService>();
            services.AddScoped<CitaValidator>();
            services.AddScoped<DisponibilidadValidator>();
            services.AddScoped<EspecialidadValidator>();
            services.AddScoped<MedicoValidator>();
            services.AddScoped<PacienteValidator>();
            services.AddScoped<PrefNotificacionValidator>();
            services.AddScoped<ProveedorSaludValidator>();
            services.AddScoped<UsuarioValidator>();

            // ============================================================
            // 4. Infrastructure (Cache, Email, SMS, SignalR, Webhook, etc.)
            // ============================================================
            services.AddInfrastructure(configuration);

            // ============================================================
            // 5. Application Services
            // ============================================================
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICitaService, CitaService>();
            services.AddScoped<IDisponibilidadService, DisponibilidadService>();
            services.AddScoped<IEspecialidadService, EspecialidadService>();
            services.AddScoped<IMedicoService, MedicoService>();
            services.AddScoped<INotificacionService, NotificacionService>();
            services.AddScoped<IPacienteService, PacienteService>();
            services.AddScoped<IPrefNotificacionService, PrefNotificacionService>();
            services.AddScoped<IProveedorSaludService, ProveedorSaludService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuditoriaService, AuditoriaService>();
            services.AddScoped<IChatbotAppService, ChatbotAppService>();
            services.AddScoped<IPagoService, PagoService>();

            return services;
        }
    }
}
