namespace SGC.Application.DTOs.Appointments
{
    // Datos para actualizar una cita existente
    public class ActualizarCitaRequest
    {
        // Identificador de la cita a actualizar
        public int Id { get; set; }

        // Nueva fecha y hora para la cita (usado en reprogramacion)
        public DateTime? FechaHora { get; set; }

        // Motivo de la actualizacion o cancelacion
        public string? Motivo { get; set; }

        // Notas adicionales sobre la cita
        public string? Notas { get; set; }
    }
}
