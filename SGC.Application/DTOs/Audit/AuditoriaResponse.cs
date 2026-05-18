using System;

namespace SGC.Application.DTOs.Audit
{
    // DTO para representar una entrada de auditoria en las respuestas de la API
    public class AuditoriaResponse
    {
        // Propiedades que reflejan los campos de la entidad Auditoria para enviar al cliente
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public DateTime Fecha { get; set; }
        public string? DireccionIP { get; set; }
    }
}