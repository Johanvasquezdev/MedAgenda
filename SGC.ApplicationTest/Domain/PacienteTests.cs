using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using Xunit;

namespace SGC.ApplicationTest.Domain
{
    // Pruebas unitarias para la entidad Paciente
    public class PacienteTests
    {
        // Crea un paciente de prueba con datos basicos
        private static Paciente CrearPacientePrueba(DateOnly? fechaNacimiento = null)
        {
            return new Paciente
            {
                Id = 1,
                Nombre = "Juan Perez",
                Email = "juan@email.com",
                PasswordHash = "hash123",
                Rol = RolUsuario.Paciente,
                Cedula = "001-1234567-8",
                FechaNacimiento = fechaNacimiento
            };
        }

        [Fact]
        public void CalcularEdad_CuandoTieneFechaNacimiento_RetornaEdadCorrecta()
        {
            // Arrange - paciente nacido hace 25 anos
            var fechaNac = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25).AddDays(-1));
            var paciente = CrearPacientePrueba(fechaNac);

            // Act
            var edad = paciente.CalcularEdad();

            // Assert
            Assert.NotNull(edad);
            Assert.Equal(25, edad);
        }

        [Fact]
        public void CalcularEdad_CuandoNoTieneFechaNacimiento_RetornaNull()
        {
            // Arrange - paciente sin fecha de nacimiento
            var paciente = CrearPacientePrueba(null);

            // Act
            var edad = paciente.CalcularEdad();

            // Assert
            Assert.Null(edad);
        }

        [Fact]
        public void Desactivar_CuandoYaDesactivado_LanzaExcepcion()
        {
            // Arrange
            var paciente = CrearPacientePrueba();
            paciente.Desactivar(); // primera vez OK

            // Act & Assert - segunda vez debe fallar
            Assert.Throws<InvalidOperationException>(() => paciente.Desactivar());
        }
    }
}
