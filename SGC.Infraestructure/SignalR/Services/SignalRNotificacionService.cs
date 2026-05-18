using Microsoft.AspNetCore.SignalR;
using SGC.Infraestructure.SignalR.Hubs;
using System.Threading.Tasks;

namespace SGC.Infraestructure.SignalR.Services
{
    // Servicio para enviar notificaciones en tiempo real a través de SignalR
    public class SignalRNotificacionService
    {
        private readonly IHubContext<CitaHub> _citaHub;
        private readonly IHubContext<DisponibilidadHub> _dispHub;

        public SignalRNotificacionService(
            IHubContext<CitaHub> citaHub,
            IHubContext<DisponibilidadHub> dispHub)
        {
            _citaHub = citaHub;
            _dispHub = dispHub;
        }

        // Métodos para enviar notificaciones a los médicos y pacientes sobre nuevas citas, cambios de estado de citas y actualizaciones de disponibilidad
        public async Task NotificarNuevaCitaAsync(
            int medicoId, object citaDto)
        {
            await _citaHub.Clients
                .Group($"medico-{medicoId}")
                .SendAsync("NuevaCita", citaDto);
        }

        // Notifica al paciente sobre cambio de estado de su cita
        public async Task NotificarEstadoCitaAsync(
            int pacienteId, object citaDto)
        {
            await _citaHub.Clients
                .Group($"paciente-{pacienteId}")
                .SendAsync("EstadoCitaActualizado", citaDto);
        }

        // Notifica a los pacientes sobre cambios en la disponibilidad del médico
        public async Task NotificarDisponibilidadAsync(
            int medicoId, object disponibilidadDto)
        {
            await _dispHub.Clients.All
                .SendAsync("DisponibilidadActualizada",
                    medicoId, disponibilidadDto);
        }
    }
}
