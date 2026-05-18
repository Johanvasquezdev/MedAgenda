using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using SGC.Domain.Validators;
using Xunit;

namespace SGC.ApplicationTest.Validators
{
    // Pruebas unitarias para el validador de medicos
    public class MedicoValidatorTests
    {
        private readonly MedicoValidator _validator = new MedicoValidator();

        // Crea un medico valido para usar como base en las pruebas
        private static Medico CrearMedicoValido()
        {
            return new Medico
            {
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash123",
                Rol = RolUsuario.Medico,
                Exequatur = "EX-12345",
                EspecialidadId = 1
            };
        }

        [Fact]
        public void Validar_CuandoMedicoValido_NoLanzaExcepcion()
        {
            // Arrange
            var medico = CrearMedicoValido();

            // Act & Assert - no debe lanzar excepcion
            _validator.Validar(medico);
        }

        [Fact]
        public void Validar_CuandoNombreVacio_LanzaExcepcion()
        {
            // Arrange
            var medico = CrearMedicoValido();
            medico.Nombre = "";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _validator.Validar(medico));
        }

        [Fact]
        public void Validar_CuandoEmailVacio_LanzaExcepcion()
        {
            // Arrange
            var medico = CrearMedicoValido();
            medico.Email = "";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _validator.Validar(medico));
        }

        [Fact]
        public void Validar_CuandoExequaturVacio_LanzaExcepcion()
        {
            // Arrange
            var medico = CrearMedicoValido();
            medico.Exequatur = "";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _validator.Validar(medico));
        }
    }
}
