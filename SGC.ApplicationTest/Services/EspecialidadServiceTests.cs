using Moq;
using SGC.Application.DTOs.Catalog;
using SGC.Application.Services;
using SGC.Domain.Entities.Catalog;
using SGC.Domain.Interfaces.ILogger;
using SGC.Domain.Interfaces.Repository;
using SGC.Domain.Validators;
using Xunit;

namespace SGC.ApplicationTest.Services
{
    // Pruebas unitarias para el servicio de especialidades medicas con repositorio mockeado
    public class EspecialidadServiceTests
    {
        private readonly Mock<IEspecialidadRepository> _especialidadRepoMock;
        private readonly Mock<ISGCLogger> _loggerMock;
        private readonly EspecialidadService _especialidadService;

        public EspecialidadServiceTests()
        {
            _especialidadRepoMock = new Mock<IEspecialidadRepository>();
            _loggerMock = new Mock<ISGCLogger>();
            _especialidadService = new EspecialidadService(
                _especialidadRepoMock.Object,
                new EspecialidadValidator(),
                _loggerMock.Object);
        }

        [Fact]
        public async Task CrearAsync_CuandoDatosValidos_RetornaEspecialidadResponse()
        {
            // Arrange
            var request = new EspecialidadRequest
            {
                Nombre = "Cardiologia",
                Descripcion = "Especialidad del corazon y sistema circulatorio"
            };

            _especialidadRepoMock.Setup(r => r.AddAsync(It.IsAny<Especialidad>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _especialidadService.CrearAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Cardiologia", resultado.Nombre);
            Assert.Equal("Especialidad del corazon y sistema circulatorio", resultado.Descripcion);
            Assert.True(resultado.Activo); // por defecto es activa
            _especialidadRepoMock.Verify(r => r.AddAsync(It.IsAny<Especialidad>()), Times.Once);
        }

        [Fact]
        public async Task ActualizarAsync_CuandoEspecialidadExiste_ActualizaDatos()
        {
            // Arrange
            var especialidadExistente = new Especialidad
            {
                Id = 1,
                Nombre = "Cardiologia",
                Descripcion = "Corazon",
                Activo = true
            };

            var request = new EspecialidadRequest
            {
                Nombre = "Cardiologia Intervencionista",
                Descripcion = "Procedimientos cardiacos invasivos y no invasivos"
            };

            _especialidadRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(especialidadExistente);
            _especialidadRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Especialidad>())).Returns(Task.CompletedTask);

            // Act
            await _especialidadService.ActualizarAsync(1, request);

            // Assert
            Assert.Equal("Cardiologia Intervencionista", especialidadExistente.Nombre);
            Assert.Equal("Procedimientos cardiacos invasivos y no invasivos", especialidadExistente.Descripcion);
            _especialidadRepoMock.Verify(r => r.UpdateAsync(especialidadExistente), Times.Once);
        }

        [Fact]
        public async Task GetActivasAsync_RetornaSoloEspecialidadesActivas()
        {
            // Arrange
            var especialidades = new List<Especialidad>
            {
                new Especialidad { Id = 1, Nombre = "Cardiologia", Activo = true },
                new Especialidad { Id = 2, Nombre = "Dermatologia", Activo = true }
            };

            _especialidadRepoMock.Setup(r => r.GetActivasAsync()).ReturnsAsync(especialidades);

            // Act
            var resultado = await _especialidadService.GetActivasAsync();

            // Assert
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, r => Assert.True(r.Activo));
            _especialidadRepoMock.Verify(r => r.GetActivasAsync(), Times.Once);
        }
    }
}
