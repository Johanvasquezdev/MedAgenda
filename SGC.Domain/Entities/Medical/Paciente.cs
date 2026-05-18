using SGC.Domain.Entities.Appointments;
using SGC.Domain.Entities.Security;

namespace SGC.Domain.Entities.Medical
{
    // El Paciente hereda de Usuario (TPT) con datos clinicos y de seguro
    public sealed class Paciente : Usuario
    {
        public string? Cedula { get; set; }
        public string? Telefono { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public string? TipoSeguro { get; set; }
        public string? NumeroSeguro { get; set; }

        // Navegacion inversa
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();

        // Calcula la edad del paciente. Retorna null si no se ha registrado la fecha de nacimiento.
        public int? CalcularEdad()
        {
            if (FechaNacimiento == null) return null;

            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            var edad = hoy.Year - FechaNacimiento.Value.Year;

            if (FechaNacimiento.Value > hoy.AddYears(-edad)) edad--;
            return edad;
        }
    }
}
