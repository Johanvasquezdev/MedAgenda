using SGC.Domain.Entities.Catalog;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio para la creacion o actualizacion de una especialidad medica
    public class EspecialidadValidator
    {
        public void Validar(Especialidad especialidad)
        {
            // Regla: el nombre es obligatorio
            if (string.IsNullOrWhiteSpace(especialidad.Nombre))
                throw new InvalidOperationException(
                    "El nombre de la especialidad es obligatorio.");

            // Regla: el nombre debe tener al menos 3 caracteres
            if (especialidad.Nombre.Length < 3)
                throw new InvalidOperationException(
                    "El nombre de la especialidad debe tener al menos 3 caracteres.");
        }
    }
}
