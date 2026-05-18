using SGC.Domain.Entities.Appointments;
using SGC.Domain.Exceptions;
using SGC.Domain.Validators;
using Xunit;

namespace SGC.ApplicationTest.Validators
{
    // Pruebas unitarias para el validador de citas medicas
    public class CitaValidatorTests
    {
        private readonly CitaValidator _validator = new CitaValidator();

        // Crea una cita valida para usar como base en las pruebas
        private static Cita CrearCitaValida()
        {
            return new Cita
            {
                PacienteId = 10,
                MedicoId = 20,
                DisponibilidadId = 1,
                FechaHora = DateTime.UtcNow.AddDays(1).Date.AddHours(9) // manana a las 9am
            };
        }

        [Fact]
        public void Validar_CuandoFechaEnElPasado_LanzaExcepcion()
        {
            // Arrange - cita con fecha en el pasado
            var cita = CrearCitaValida();
            cita.FechaHora = DateTime.UtcNow.AddDays(-1);

            // Act & Assert
            Assert.Throws<CitaConflictoException>(() => _validator.Validar(cita));
        }

        [Fact]
        public void Validar_CuandoHorarioFueraDeRango_LanzaExcepcion()
        {
            // Arrange - cita a las 4am (fuera del rango 6am-10pm)
            var cita = CrearCitaValida();
            cita.FechaHora = DateTime.UtcNow.AddDays(1).Date.AddHours(4);

            // Act & Assert
            Assert.Throws<HorarioNoDisponibleException>(() => _validator.Validar(cita));
        }

        [Fact]
        public void Validar_CuandoPacienteEsIgualAlMedico_LanzaExcepcion()
        {
            // Arrange - paciente y medico son la misma persona
            var cita = CrearCitaValida();
            cita.PacienteId = 10;
            cita.MedicoId = 10;

            // Act & Assert
            Assert.Throws<CitaConflictoException>(() => _validator.Validar(cita));
        }

        [Fact]
        public void Validar_CuandoNoTieneDisponibilidad_LanzaExcepcion()
        {
            // Arrange - cita sin disponibilidad asignada
            var cita = CrearCitaValida();
            cita.DisponibilidadId = null;

            // Act & Assert
            Assert.Throws<HorarioNoDisponibleException>(() => _validator.Validar(cita));
        }
    }
}
