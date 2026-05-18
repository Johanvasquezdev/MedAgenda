namespace SGC.Application.DTOs.Payments
{
    // DTO para crear un nuevo pago asociado a una cita y paciente, con monto y moneda especificados
    public class CrearPagoRequest
    {
        public int CitaId { get; set; }
        public int PacienteId { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "DOP";
    }
}