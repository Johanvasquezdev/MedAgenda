using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Catalog;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Entities.Security;
using SGC.Domain.Enums;
using SGC.Persistence.Context;

namespace SGC.API.Seeding;

public static class DemoDataSeeder
{
    public const string AdminEmail = "admin.demo@medagenda.test";
    public const string DemoPassword = "MedAgendaDemo2026!";

    private const string DemoMarker = "[DEMO-SEED]";

    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SGCDbContext>();
        var connectionString = db.Database.GetDbConnection().ConnectionString;

        if (!IsLocalConnection(connectionString) &&
            !configuration.GetValue("DemoSeed:AllowRemote", false))
        {
            logger.LogWarning(
                "Skipping demo seed: Development seeding is limited to local databases. Set DemoSeed:AllowRemote only for a dedicated disposable demo database.");
            return;
        }

        if (!await db.Database.CanConnectAsync(cancellationToken))
        {
            logger.LogWarning("Skipping demo seed: the configured database is unavailable.");
            return;
        }

        var admin = await db.Administradores
            .SingleOrDefaultAsync(user => user.Email == AdminEmail, cancellationToken);

        if (admin is null)
        {
            var emailAlreadyUsed = await db.Usuarios
                .AnyAsync(user => user.Email == AdminEmail, cancellationToken);

            if (emailAlreadyUsed)
            {
                throw new InvalidOperationException(
                    $"The demo email {AdminEmail} already belongs to a non-admin account.");
            }

            db.Administradores.Add(new Administrador
            {
                Nombre = "Administración Demo",
                Email = AdminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
                Activo = true,
            });
        }

        var specialties = await EnsureSpecialtiesAsync(db, cancellationToken);
        var providers = await EnsureProvidersAsync(db, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var patients = await EnsurePatientsAsync(db, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var doctors = await EnsureDoctorsAsync(
            db,
            specialties,
            providers,
            cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await EnsureAppointmentsAsync(db, patients, doctors, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Demo data is ready. Sign in with {DemoEmail}; the demo password is documented in docs/DEMO-SEED.md.",
            AdminEmail);
    }

    private static async Task<Dictionary<string, Especialidad>> EnsureSpecialtiesAsync(
        SGCDbContext db,
        CancellationToken cancellationToken)
    {
        var descriptions = new Dictionary<string, string>
        {
            ["Cardiología"] = "Atención del corazón y sistema circulatorio.",
            ["Dermatología"] = "Salud de la piel, cabello y uñas.",
            ["Pediatría"] = "Atención integral de niñas, niños y adolescentes.",
        };

        var existing = await db.Especialidades
            .Where(specialty => descriptions.Keys.Contains(specialty.Nombre))
            .ToDictionaryAsync(specialty => specialty.Nombre, cancellationToken);

        foreach (var (name, description) in descriptions)
        {
            if (existing.ContainsKey(name)) continue;

            var specialty = new Especialidad
            {
                Nombre = name,
                Descripcion = description,
                Activo = true,
            };
            db.Especialidades.Add(specialty);
            existing.Add(name, specialty);
        }

        return existing;
    }

    private static async Task<Dictionary<string, ProveedorSalud>> EnsureProvidersAsync(
        SGCDbContext db,
        CancellationToken cancellationToken)
    {
        var names = new Dictionary<string, string>
        {
            ["Centro Médico Demo Central"] = "Centro médico",
            ["Clínica Demo Norte"] = "Clínica",
            ["Hospital Demo Salud"] = "Hospital",
        };

        var existing = await db.ProveedoresSalud
            .Where(provider => names.Keys.Contains(provider.Nombre))
            .ToDictionaryAsync(provider => provider.Nombre, cancellationToken);

        foreach (var (name, type) in names)
        {
            if (existing.ContainsKey(name)) continue;

            var provider = new ProveedorSalud
            {
                Nombre = name,
                Tipo = type,
                Telefono = "809-555-0100",
                Email = "contacto@medagenda.test",
                Activo = true,
            };
            db.ProveedoresSalud.Add(provider);
            existing.Add(name, provider);
        }

        return existing;
    }

    private static async Task<Dictionary<string, Paciente>> EnsurePatientsAsync(
        SGCDbContext db,
        CancellationToken cancellationToken)
    {
        var patientDetails = new (string Name, string Email, string Insurance)[]
        {
            ("Carla Demo", "carla.paciente@medagenda.test", "ARS Humano"),
            ("Diego Demo", "diego.paciente@medagenda.test", "SeNaSa"),
            ("Elena Demo", "elena.paciente@medagenda.test", "ARS Universal"),
        };
        var emails = patientDetails.Select(patient => patient.Email).ToArray();
        var existing = await db.Pacientes
            .Where(patient => emails.Contains(patient.Email))
            .ToDictionaryAsync(patient => patient.Email, cancellationToken);

        foreach (var (name, email, insurance) in patientDetails)
        {
            if (existing.ContainsKey(email)) continue;

            var patient = new Paciente
            {
                Nombre = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
                Rol = RolUsuario.Paciente,
                Activo = true,
                Telefono = "809-555-0101",
                FechaNacimiento = new DateOnly(1990, 5, 12),
                TipoSeguro = insurance,
                NumeroSeguro = "DEMO-" + email[..email.IndexOf('@')].ToUpperInvariant(),
            };
            db.Pacientes.Add(patient);
            existing.Add(email, patient);
        }

        return existing;
    }

    private static async Task<Dictionary<string, Medico>> EnsureDoctorsAsync(
        SGCDbContext db,
        IReadOnlyDictionary<string, Especialidad> specialties,
        IReadOnlyDictionary<string, ProveedorSalud> providers,
        CancellationToken cancellationToken)
    {
        var doctorDetails = new (string Name, string Email, string Specialty, string Provider, string License)[]
        {
            ("Dra. Ana Reyes", "ana.reyes@medagenda.test", "Cardiología", "Centro Médico Demo Central", "DEMO-EX-001"),
            ("Dr. Luis Santos", "luis.santos@medagenda.test", "Dermatología", "Clínica Demo Norte", "DEMO-EX-002"),
            ("Dra. Paula Méndez", "paula.mendez@medagenda.test", "Pediatría", "Hospital Demo Salud", "DEMO-EX-003"),
        };
        var emails = doctorDetails.Select(doctor => doctor.Email).ToArray();
        var existing = await db.Medicos
            .Where(doctor => emails.Contains(doctor.Email))
            .ToDictionaryAsync(doctor => doctor.Email, cancellationToken);

        foreach (var (name, email, specialtyName, providerName, license) in doctorDetails)
        {
            if (existing.ContainsKey(email)) continue;

            var doctor = new Medico
            {
                Nombre = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
                Rol = RolUsuario.Medico,
                Activo = true,
                MedicoActivo = true,
                Exequatur = license,
                TelefonoConsultorio = "809-555-0102",
                EspecialidadId = specialties[specialtyName].Id,
                ProveedorSaludId = providers[providerName].Id,
            };
            db.Medicos.Add(doctor);
            existing.Add(email, doctor);
        }

        return existing;
    }

    private static async Task EnsureAppointmentsAsync(
        SGCDbContext db,
        IReadOnlyDictionary<string, Paciente> patients,
        IReadOnlyDictionary<string, Medico> doctors,
        CancellationToken cancellationToken)
    {
        var examples = new (string Key, string PatientEmail, string DoctorEmail, int DayOffset, int Hour, EstadoCita State, string Reason)[]
        {
            ("confirmada", "carla.paciente@medagenda.test", "ana.reyes@medagenda.test", 1, 9, EstadoCita.Confirmada, "Control preventivo"),
            ("solicitada", "diego.paciente@medagenda.test", "luis.santos@medagenda.test", 2, 11, EstadoCita.Solicitada, "Consulta inicial"),
            ("confirmada-2", "elena.paciente@medagenda.test", "paula.mendez@medagenda.test", 3, 14, EstadoCita.Confirmada, "Consulta pediátrica"),
            ("completada", "carla.paciente@medagenda.test", "ana.reyes@medagenda.test", -4, 10, EstadoCita.Completada, "Seguimiento de rutina"),
        };

        foreach (var (key, patientEmail, doctorEmail, dayOffset, hour, state, reason) in examples)
        {
            var marker = $"{DemoMarker} {key}";
            if (await db.Citas.AnyAsync(appointment => appointment.Notas == marker, cancellationToken))
            {
                continue;
            }

            db.Citas.Add(new Cita
            {
                PacienteId = patients[patientEmail].Id,
                MedicoId = doctors[doctorEmail].Id,
                FechaHora = DateTime.UtcNow.Date.AddDays(dayOffset).AddHours(hour),
                Estado = state,
                Motivo = reason,
                Notas = marker,
            });
        }
    }

    private static bool IsLocalConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return false;

        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
        var host = builder.TryGetValue("Host", out var hostValue)
            ? Convert.ToString(hostValue)
            : builder.TryGetValue("Server", out var serverValue)
                ? Convert.ToString(serverValue)
                : null;

        if (string.IsNullOrWhiteSpace(host)) return false;

        return host.Split(',').Select(value => value.Trim()).All(value =>
            value.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("::1", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("host.docker.internal", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("postgres", StringComparison.OrdinalIgnoreCase));
    }
}
