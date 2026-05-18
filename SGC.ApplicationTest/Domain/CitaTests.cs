using SGC.Domain.Entities.Appointments;
using SGC.Domain.Enums;
using SGC.Domain.Exceptions;
using Xunit;

namespace SGC.ApplicationTest.Domain
{
    // Pruebas unitarias para la maquina de estados de la entidad Cita
    public class CitaTests
    {
        // Crea una cita de prueba con estado Solicitada (por defecto)
        private static Cita CrearCitaPrueba()
        {
            return new Cita
            {
                Id = 1,
                PacienteId = 10,
                MedicoId = 20,
                DisponibilidadId = 1,
                FechaHora = DateTime.UtcNow.AddDays(1),
                Motivo = "Consulta general"
            };
        }

        // =============================================
        // Confirmar
        // =============================================

        [Fact]
        public void Confirmar_CuandoEstadoSolicitada_CambiaAConfirmada()
        {
            // Arrange
            var cita = CrearCitaPrueba();

            // Act
            cita.Confirmar();

            // Assert
            Assert.Equal(EstadoCita.Confirmada, cita.Estado);
        }

        [Fact]
        public void Confirmar_CuandoEstadoNoEsSolicitada_LanzaExcepcion()
        {
            // Arrange - cita ya confirmada
            var cita = CrearCitaPrueba();
            cita.Confirmar();

            // Act & Assert - intentar confirmar de nuevo debe fallar
            Assert.Throws<CitaConflictoException>(() => cita.Confirmar());
        }

        // =============================================
        // Cancelar
        // =============================================

        [Fact]
        public void Cancelar_CuandoEstadoSolicitada_CambiaACancelada()
        {
            // Arrange
            var cita = CrearCitaPrueba();

            // Act
            cita.Cancelar("Ya no puedo asistir");

            // Assert
            Assert.Equal(EstadoCita.Cancelada, cita.Estado);
            Assert.Equal("Ya no puedo asistir", cita.Motivo);
        }

        [Fact]
        public void Cancelar_CuandoEstadoCompletada_LanzaExcepcion()
        {
            // Arrange - llevar la cita a estado Completada
            var cita = CrearCitaPrueba();
            cita.Confirmar();
            cita.IniciarConsulta();
            cita.Completar();

            // Act & Assert - no se puede cancelar una cita completada
            Assert.Throws<CitaConflictoException>(() => cita.Cancelar("Motivo"));
        }

        // =============================================
        // Rechazar
        // =============================================

        [Fact]
        public void Rechazar_CuandoEstadoSolicitada_CambiaARechazada()
        {
            // Arrange
            var cita = CrearCitaPrueba();

            // Act
            cita.Rechazar("Horario no disponible");

            // Assert
            Assert.Equal(EstadoCita.Rechazada, cita.Estado);
            Assert.Equal("Horario no disponible", cita.Motivo);
        }

        // =============================================
        // IniciarConsulta y Completar
        // =============================================

        [Fact]
        public void IniciarConsulta_CuandoConfirmada_CambiaAEnProgreso()
        {
            // Arrange
            var cita = CrearCitaPrueba();
            cita.Confirmar();

            // Act
            cita.IniciarConsulta();

            // Assert
            Assert.Equal(EstadoCita.EnProgreso, cita.Estado);
        }

        [Fact]
        public void Completar_CuandoEnProgreso_CambiaACompletada()
        {
            // Arrange - llevar la cita a EnProgreso
            var cita = CrearCitaPrueba();
            cita.Confirmar();
            cita.IniciarConsulta();

            // Act
            cita.Completar();

            // Assert
            Assert.Equal(EstadoCita.Completada, cita.Estado);
        }

        // =============================================
        // MarcarNoAsistio
        // =============================================

        [Fact]
        public void MarcarNoAsistio_CuandoNoConfirmada_LanzaExcepcion()
        {
            // Arrange - cita en estado Solicitada (no confirmada)
            var cita = CrearCitaPrueba();

            // Act & Assert - solo se puede marcar NoAsistio cuando esta Confirmada
            Assert.Throws<CitaConflictoException>(() => cita.MarcarNoAsistio());
        }
    }
}
