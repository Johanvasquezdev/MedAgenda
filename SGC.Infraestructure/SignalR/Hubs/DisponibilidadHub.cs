using Microsoft.AspNetCore.SignalR;

namespace SGC.Infraestructure.SignalR.Hubs
{
    // Hub para notificar cambios de disponibilidad de los médicos a los pacientes en tiempo real
    public class DisponibilidadHub : Hub
    {
        // Notifica cambio de disponibilidad del médico
        public async Task NotificarCambioDisponibilidad(
            int medicoId, object disponibilidadDto)
        {
            await Clients.All
                .SendAsync("DisponibilidadActualizada",
                    medicoId, disponibilidadDto);
        }

        // Une al usuario a su grupo personal para recibir notificaciones específicas
        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
