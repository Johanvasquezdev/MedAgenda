using Moq;
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
    // Pruebas unitarias para el servicio de medicos con repositorio mockeado
    public class MedicoServiceTests
    {
        private readonly Mock<IMedicoRepository> _medicoRepoMock;
        private readonly Mock<ISGCLogger> _loggerMock;
        private readonly MedicoService _medicoService;

        public MedicoServiceTests()
        {
            _medicoRepoMock = new Mock<IMedicoRepository>();
            _loggerMock = new Mock<ISGCLogger>();
            _medicoService = new MedicoService(
                _medicoRepoMock.Object,
                new MedicoValidator(),
                _loggerMock.Object);
        }

        [Fact]
        public async Task CrearAsync_CuandoDatosValidos_RetornaMedicoResponse()
        {
            // Arrange
            var request = new CrearMedicoRequest
            {
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                Password = "password123",
                Exequatur = "EX-12345",
                EspecialidadId = 1,
                ProveedorSaludId = 1,
                TelefonoConsultorio = "809-555-0001"
            };

            _medicoRepoMock.Setup(r => r.AddAsync(It.IsAny<Medico>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _medicoService.CrearAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Dr. Garcia", resultado.Nombre);
            Assert.Equal("garcia@email.com", resultado.Email);
            Assert.Equal("EX-12345", resultado.Exequatur);
            _medicoRepoMock.Verify(r => r.AddAsync(It.IsAny<Medico>()), Times.Once);
        }

        [Fact]
        public async Task ActualizarAsync_CuandoMedicoExiste_ActualizaDatos()
        {
            // Arrange
            var medicoExistente = new Medico
            {
                Id = 1,
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Medico,
                Exequatur = "EX-12345",
                EspecialidadId = 1
            };

            var request = new ActualizarMedicoRequest
            {
                Id = 1,
                Nombre = "Dr. Garcia Lopez",
                Email = "garcia.lopez@email.com",
                Exequatur = "EX-12345",
                EspecialidadId = 2,
                ProveedorSaludId = 1,
                TelefonoConsultorio = "809-555-0002"
            };

            _medicoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medicoExistente);
            _medicoRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Medico>())).Returns(Task.CompletedTask);

            // Act
            await _medicoService.ActualizarAsync(request);

            // Assert - verificar que el nombre se actualizo
            Assert.Equal("Dr. Garcia Lopez", medicoExistente.Nombre);
            Assert.Equal(2, medicoExistente.EspecialidadId);
            _medicoRepoMock.Verify(r => r.UpdateAsync(medicoExistente), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CuandoMedicoExiste_RetornaMedicoResponse()
        {
            // Arrange
            var medico = new Medico
            {
                Id = 1,
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash",
                Rol = RolUsuario.Medico,
                Exequatur = "EX-12345",
                EspecialidadId = 1,
                MedicoActivo = true
            };

            _medicoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(medico);

            // Act
            var resultado = await _medicoService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Dr. Garcia", resultado.Nombre);
            Assert.True(resultado.MedicoActivo);
        }
    }
}
