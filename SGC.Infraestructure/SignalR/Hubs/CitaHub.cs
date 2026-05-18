using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SGC.Infraestructure.SignalR.Hubs
{
    // Hub para notificar a médicos y pacientes sobre nuevas citas, cambios de estado de citas y actualizaciones en tiempo real
    public class CitaHub : Hub
    {
        // Notifica al médico sobre nueva cita
        public async Task NotificarNuevaCita(int medicoId,
            object citaDto)
        {
            await Clients.Group($"medico-{medicoId}")
                .SendAsync("NuevaCita", citaDto);
        }

        // Notifica al paciente sobre cambio de estado
        public async Task NotificarEstadoCita(int pacienteId,
            object citaDto)
        {
            await Clients.Group($"paciente-{pacienteId}")
                .SendAsync("EstadoCitaActualizado", citaDto);
        }

        // Une al usuario a su grupo personal
        public async Task UnirseAGrupo(string tipoUsuario, int usuarioId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"{tipoUsuario}-{usuarioId}");
        }

        // Elimina al usuario de su grupo personal al desconectarse
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
