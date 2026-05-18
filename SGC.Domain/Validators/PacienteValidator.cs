using SGC.Domain.Entities.Medical;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio para la creacion o actualizacion de un paciente
    public class PacienteValidator
    {
        public void Validar(Paciente paciente)
        {
            // Regla: el nombre es obligatorio
            if (string.IsNullOrWhiteSpace(paciente.Nombre))
                throw new InvalidOperationException(
                    "El nombre del paciente es obligatorio.");

            // Regla: el email es obligatorio
            if (string.IsNullOrWhiteSpace(paciente.Email))
                throw new InvalidOperationException(
                    "El email del paciente es obligatorio.");

            // Regla: la cedula es obligatoria
            if (string.IsNullOrWhiteSpace(paciente.Cedula))
                throw new InvalidOperationException(
                    "La cedula del paciente es obligatoria.");

            // Regla: la fecha de nacimiento no puede ser futura
            if (paciente.FechaNacimiento != null &&
                paciente.FechaNacimiento > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new InvalidOperationException(
                    "La fecha de nacimiento no puede ser una fecha futura.");
        }
    }
}
