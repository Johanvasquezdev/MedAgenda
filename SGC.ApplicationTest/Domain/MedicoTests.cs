using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Medical;
using SGC.Domain.Enums;
using Xunit;

namespace SGC.ApplicationTest.Domain
{
    // Pruebas unitarias para la entidad Medico
    public class MedicoTests
    {
        // Crea un medico de prueba con un horario de disponibilidad para Lunes 8am-12pm
        private static Medico CrearMedicoPrueba()
        {
            var medico = new Medico
            {
                Id = 1,
                Nombre = "Dr. Garcia",
                Email = "garcia@email.com",
                PasswordHash = "hash123",
                Rol = RolUsuario.Medico,
                Exequatur = "EX-12345",
                EspecialidadId = 1,
                MedicoActivo = true
            };

            // Agregar horario de Lunes 8:00 - 12:00
            medico.Horarios.Add(new Disponibilidad
            {
                Id = 1,
                MedicoId = 1,
                DiaSemana = DiaSemana.Lunes,
                HoraInicio = TimeSpan.FromHours(8),
                HoraFin = TimeSpan.FromHours(12),
                DuracionCitaMin = 30,
                EsRecurrente = true
            });

            return medico;
        }

        [Fact]
        public void TieneDisponibilidad_CuandoHorarioCoincide_RetornaTrue()
        {
            // Arrange - crear medico con horario Lunes 8-12
            var medico = CrearMedicoPrueba();

            // Act - buscar un Lunes a las 9am (proximo Lunes)
            var proximoLunes = ObtenerProximoDia(DayOfWeek.Monday);
            var fechaHora = proximoLunes.AddHours(9);

            // Assert
            Assert.True(medico.TieneDisponibilidad(fechaHora));
        }

        [Fact]
        public void TieneDisponibilidad_CuandoNoHayHorario_RetornaFalse()
        {
            // Arrange - crear medico con horario solo Lunes
            var medico = CrearMedicoPrueba();

            // Act - buscar un Martes (no tiene horario)
            var proximoMartes = ObtenerProximoDia(DayOfWeek.Tuesday);
            var fechaHora = proximoMartes.AddHours(9);

            // Assert
            Assert.False(medico.TieneDisponibilidad(fechaHora));
        }

        [Fact]
        public void Desactivar_CuandoActivo_CambiaEstadoCorrectamente()
        {
            // Arrange
            var medico = CrearMedicoPrueba();
            Assert.True(medico.Activo);

            // Act
            medico.Desactivar();

            // Assert
            Assert.False(medico.Activo);
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
