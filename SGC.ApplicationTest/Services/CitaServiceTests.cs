using SGC.Domain.Interfaces;
using Moq;
using SGC.Application.DTOs.Appointments;
using SGC.Application.Services;
using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using SGC.Domain.Exceptions;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Services;
using SGC.Domain.Entities.Notifications;
using Xunit;

namespace SGC.ApplicationTest.Services
{
    // Pruebas unitarias para el servicio de citas medicas con repositorios mockeados
    public class CitaServiceTests
    {
        private readonly Mock<ICitaRepository> _citaRepoMock;
        private readonly Mock<IMedicoRepository> _medicoRepoMock;
        private readonly Mock<INotificacionRepository> _notificacionRepoMock;
        private readonly Mock<IPacienteRepository> _pacienteRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly CitaDomainService _domainService;
        private readonly Mock<ISGCLogger> _loggerMock;
        private readonly CitaService _citaService;

                public CitaServiceTests()
        {
            _citaRepoMock = new Mock<ICitaRepository>();
            _medicoRepoMock = new Mock<IMedicoRepository>();
            _notificacionRepoMock = new Mock<INotificacionRepository>();
            _pacienteRepoMock = new Mock<IPacienteRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ISGCLogger>();
            _domainService = new CitaDomainService();
            _citaService = new CitaService(
                _citaRepoMock.Object,
                _medicoRepoMock.Object,
                _notificacionRepoMock.Object,
                _pacienteRepoMock.Object,
                _emailServiceMock.Object,
                _domainService,
                _loggerMock.Object);
        }

        [Fact]
        public async Task AgendarAsync_CuandoDatosValidos_RetornaCitaResponse()
        {
            // Arrange - crear medico con disponibilidad para la fecha de la cita
            var proximoLunes = ObtenerProximoDia(DayOfWeek.Monday);
            var fechaCita = proximoLunes.AddHours(9);

            var medico = new Medico
            {
                Id = 20,
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Medico,
                Exequatur = "EX-123",
                EspecialidadId = 1,
                MedicoActivo = true,
                Activo = true
            };
            medico.Horarios.Add(new Disponibilidad
            {
                Id = 1,
                MedicoId = 20,
                DiaSemana = DiaSemana.Lunes,
                HoraInicio = TimeSpan.FromHours(8),
                HoraFin = TimeSpan.FromHours(12),
                DuracionCitaMin = 30
            });

            var request = new CrearCitaRequest
            {
                PacienteId = 10,
                MedicoId = 20,
                DisponibilidadId = 1,
                FechaHora = fechaCita,
                Motivo = "Consulta general"
            };

            _medicoRepoMock.Setup(r => r.GetByIdWithHorariosAsync(20)).ReturnsAsync(medico);
            _citaRepoMock.Setup(r => r.ExisteConflictoAsync(20, fechaCita)).ReturnsAsync(false);
            _citaRepoMock.Setup(r => r.AddAsync(It.IsAny<Cita>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _citaService.AgendarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.PacienteId);
            Assert.Equal(20, resultado.MedicoId);
            Assert.Equal("Solicitada", resultado.Estado);
            _citaRepoMock.Verify(r => r.AddAsync(It.IsAny<Cita>()), Times.Once);
        }

        [Fact]
        public async Task AgendarAsync_CuandoConflictoHorario_LanzaExcepcion()
        {
            // Arrange - existe conflicto de horario
            var proximoLunes = ObtenerProximoDia(DayOfWeek.Monday);
            var fechaCita = proximoLunes.AddHours(9);

            var medico = new Medico
            {
                Id = 20,
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Medico,
                Exequatur = "EX-123",
                EspecialidadId = 1,
                MedicoActivo = true,
                Activo = true
            };
            medico.Horarios.Add(new Disponibilidad
            {
                Id = 1,
                MedicoId = 20,
                DiaSemana = DiaSemana.Lunes,
                HoraInicio = TimeSpan.FromHours(8),
                HoraFin = TimeSpan.FromHours(12),
                DuracionCitaMin = 30
            });

            var request = new CrearCitaRequest
            {
                PacienteId = 10,
                MedicoId = 20,
                DisponibilidadId = 1,
                FechaHora = fechaCita,
                Motivo = "Consulta"
            };

            _medicoRepoMock.Setup(r => r.GetByIdWithHorariosAsync(20)).ReturnsAsync(medico);
            _citaRepoMock.Setup(r => r.ExisteConflictoAsync(20, fechaCita)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<CitaConflictoException>(
                () => _citaService.AgendarAsync(request));
        }

        [Fact]
        public async Task CancelarAsync_CuandoCitaExiste_CambiaEstadoACancelada()
        {
            // Arrange
            var cita = new Cita
            {
                Id = 1,
                PacienteId = 10,
                MedicoId = 20,
                FechaHora = DateTime.UtcNow.AddDays(1),
                Estado = EstadoCita.Solicitada
            };

            _citaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cita);
            _citaRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cita>())).Returns(Task.CompletedTask);
            _medicoRepoMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new Medico
            {
                Id = 20,
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Medico,
                MedicoActivo = true,
                Activo = true
            });
            _notificacionRepoMock.Setup(r => r.AddAsync(It.IsAny<Notificacion>())).Returns(Task.CompletedTask);

            // Act
            await _citaService.CancelarAsync(1, "No puedo asistir");

            // Assert
            Assert.Equal(EstadoCita.Cancelada, cita.Estado);
            _citaRepoMock.Verify(r => r.UpdateAsync(cita), Times.Once);
        }

        [Fact]
        public async Task ConfirmarAsync_CuandoCitaExiste_CambiaEstadoAConfirmada()
        {
            // Arrange
            var cita = new Cita
            {
                Id = 1,
                PacienteId = 10,
                MedicoId = 20,
                FechaHora = DateTime.UtcNow.AddDays(1),
                Estado = EstadoCita.Solicitada
            };

            _citaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cita);
            _citaRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cita>())).Returns(Task.CompletedTask);
            _medicoRepoMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new Medico
            {
                Id = 20,
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Medico,
                MedicoActivo = true,
                Activo = true
            });
            _notificacionRepoMock.Setup(r => r.AddAsync(It.IsAny<Notificacion>())).Returns(Task.CompletedTask);

            // Act
            await _citaService.ConfirmarAsync(1);

            // Assert
            Assert.Equal(EstadoCita.Confirmada, cita.Estado);
            _citaRepoMock.Verify(r => r.UpdateAsync(cita), Times.Once);
            _notificacionRepoMock.Verify(r => r.AddAsync(It.Is<Notificacion>(n =>
                n.UsuarioId == cita.PacienteId &&
                n.CitaId == cita.Id &&
                n.Mensaje == "Dr. Garcia aceptó tu cita")), Times.Once);
        }

        [Fact]
        public async Task CancelarPorMedicoAsync_CuandoCitaExiste_CreaNotificacionPaciente()
        {
            // Arrange
            var cita = new Cita
            {
                Id = 1,
                PacienteId = 10,
                MedicoId = 20,
                FechaHora = DateTime.UtcNow.AddDays(1),
                Estado = EstadoCita.Solicitada
            };

            _citaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cita);
            _citaRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cita>())).Returns(Task.CompletedTask);
            _medicoRepoMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new Medico
            {
                Id = 20,
                Nombre = "Dra. Lopez",
                Email = "lopez@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Medico,
                MedicoActivo = true,
                Activo = true
            });
            _notificacionRepoMock.Setup(r => r.AddAsync(It.IsAny<Notificacion>())).Returns(Task.CompletedTask);

            // Act
            await _citaService.CancelarPorMedicoAsync(1, "No disponible");

            // Assert
            Assert.Equal(EstadoCita.Cancelada, cita.Estado);
            _notificacionRepoMock.Verify(r => r.AddAsync(It.Is<Notificacion>(n =>
                n.UsuarioId == cita.PacienteId &&
                n.CitaId == cita.Id &&
                n.Mensaje == "Dra. Lopez canceló tu cita")), Times.Once);
        }

        [Fact]
        public async Task GetByMedicoAsync_RetornaListaDeCitas()
        {
            // Arrange
            var citas = new List<Cita>
            {
                new Cita { Id = 1, PacienteId = 10, MedicoId = 20, FechaHora = DateTime.UtcNow.AddDays(1), Estado = EstadoCita.Solicitada },
                new Cita { Id = 2, PacienteId = 11, MedicoId = 20, FechaHora = DateTime.UtcNow.AddDays(2), Estado = EstadoCita.Confirmada }
            };

            _citaRepoMock.Setup(r => r.GetByMedicoIdAsync(20)).ReturnsAsync(citas);

            // Act
            var resultado = await _citaService.GetByMedicoAsync(20);

            // Assert
            Assert.Equal(2, resultado.Count());
            _citaRepoMock.Verify(r => r.GetByMedicoIdAsync(20), Times.Once);
        }

        // Obtiene la proxima fecha para un dia de la semana especifico
        private static DateTime ObtenerProximoDia(DayOfWeek dia)
        {
            var hoy = DateTime.UtcNow.Date;
            int diasHastaObjetivo = ((int)dia - (int)hoy.DayOfWeek + 7) % 7;
            if (diasHastaObjetivo == 0) diasHastaObjetivo = 7;
            return hoy.AddDays(diasHastaObjetivo);
        }
    }
}