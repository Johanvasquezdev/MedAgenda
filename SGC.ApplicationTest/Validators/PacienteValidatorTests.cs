using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using SGC.Domain.Validators;
using Xunit;

namespace SGC.ApplicationTest.Validators
{
    // Pruebas unitarias para el validador de pacientes
    public class PacienteValidatorTests
    {
        private readonly PacienteValidator _validator = new PacienteValidator();

        // Crea un paciente valido para usar como base en las pruebas
        private static Paciente CrearPacienteValido()
        {
            return new Paciente
            {
                Nombre = "Juan Perez",
                Email = "juan@email.com",
                PasswordHash = "hash123",
                Rol = RolUsuario.Paciente,
                Cedula = "001-1234567-8",
                FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30))
            };
        }

        [Fact]
        public void Validar_CuandoPacienteValido_NoLanzaExcepcion()
        {
            // Arrange
            var paciente = CrearPacienteValido();

            // Act & Assert - no debe lanzar excepcion
            _validator.Validar(paciente);
        }

        [Fact]
        public void Validar_CuandoNombreVacio_LanzaExcepcion()
        {
            // Arrange
            var paciente = CrearPacienteValido();
            paciente.Nombre = "";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _validator.Validar(paciente));
        }

        [Fact]
        public void Validar_CuandoFechaNacimientoFutura_LanzaExcepcion()
        {
            // Arrange - fecha de nacimiento en el futuro
            var paciente = CrearPacienteValido();
            paciente.FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _validator.Validar(paciente));
        }
    }
}
