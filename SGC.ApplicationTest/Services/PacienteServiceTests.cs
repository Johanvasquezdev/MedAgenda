using Moq;
using SGC.Domain.Interfaces;
using SGC.Application.DTOs.Medical;
using SGC.Application.Services;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Validators;
using Xunit;

namespace SGC.ApplicationTest.Services
{
    // Pruebas unitarias para el servicio de pacientes con repositorio mockeado
    public class PacienteServiceTests
    {
        private readonly Mock<IPacienteRepository> _pacienteRepoMock;
        private readonly Mock<ISGCLogger> _loggerMock;
        private readonly PacienteService _pacienteService;

        public PacienteServiceTests()
        {
            _pacienteRepoMock = new Mock<IPacienteRepository>();
            _loggerMock = new Mock<ISGCLogger>();
            _pacienteService = new PacienteService(
                _pacienteRepoMock.Object,
                new PacienteValidator(),
                new Mock<IEmailService>().Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CrearAsync_CuandoDatosValidos_RetornaPacienteResponse()
        {
            // Arrange
            var request = new CrearPacienteRequest
            {
                Nombre = "Juan Perez",
                Email = "juan@email.com",
                Password = "password123",
                Cedula = "001-1234567-8",
                Telefono = "809-555-0001",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
                TipoSeguro = "ARS Humano",
                NumeroSeguro = "POL-001"
            };

            _pacienteRepoMock.Setup(r => r.AddAsync(It.IsAny<Paciente>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _pacienteService.CrearAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Juan Perez", resultado.Nombre);
            Assert.Equal("001-1234567-8", resultado.Cedula);
            Assert.NotNull(resultado.Edad); // debe calcular la edad
            _pacienteRepoMock.Verify(r => r.AddAsync(It.IsAny<Paciente>()), Times.Once);
        }

        [Fact]
        public async Task ActualizarAsync_CuandoPacienteExiste_ActualizaDatos()
        {
            // Arrange
            var pacienteExistente = new Paciente
            {
                Id = 1,
                Nombre = "Juan Perez",
                Email = "juan@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Paciente,
                Cedula = "001-1234567-8"
            };

            var request = new ActualizarPacienteRequest
            {
                Id = 1,
                Nombre = "Juan Alberto Perez",
                Email = "juan.alberto@email.com",
                Cedula = "001-1234567-8",
                Telefono = "809-555-9999",
                TipoSeguro = "ARS Senasa",
                NumeroSeguro = "POL-002"
            };

            _pacienteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pacienteExistente);
            _pacienteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Paciente>())).Returns(Task.CompletedTask);

            // Act
            await _pacienteService.ActualizarAsync(request);

            // Assert - verificar que los datos se actualizaron
            Assert.Equal("Juan Alberto Perez", pacienteExistente.Nombre);
            Assert.Equal("809-555-9999", pacienteExistente.Telefono);
            Assert.Equal("ARS Senasa", pacienteExistente.TipoSeguro);
            _pacienteRepoMock.Verify(r => r.UpdateAsync(pacienteExistente), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CuandoPacienteExiste_RetornaPacienteResponse()
        {
            // Arrange
            var paciente = new Paciente
            {
                Id = 1,
                Nombre = "Juan Perez",
                Email = "juan@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Paciente,
                Cedula = "001-1234567-8",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25))
            };

            _pacienteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(paciente);

            // Act
            var resultado = await _pacienteService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Juan Perez", resultado.Nombre);
            Assert.Equal(25, resultado.Edad);
        }
    }
}
