using SGC.Domain.Entities.Medical;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio para la creacion o actualizacion de un medico
    public class MedicoValidator
    {
        public void Validar(Medico medico)
        {
            // Regla: el nombre es obligatorio
            if (string.IsNullOrWhiteSpace(medico.Nombre))
                throw new InvalidOperationException(
                    "El nombre del medico es obligatorio.");

            // Regla: el email es obligatorio
            if (string.IsNullOrWhiteSpace(medico.Email))
                throw new InvalidOperationException(
                    "El email del medico es obligatorio.");

            // Regla: el exequatur es obligatorio para ejercer
            if (string.IsNullOrWhiteSpace(medico.Exequatur))
                throw new InvalidOperationException(
                    "El exequatur es obligatorio para registrar un medico.");

            // Regla: debe tener una especialidad asignada
            if (medico.EspecialidadId == null || medico.EspecialidadId <= 0)
                throw new InvalidOperationException(
                    "El medico debe tener una especialidad asignada.");
        }
    }
}
