using SGC.Domain.Entities.Catalog;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio para la creacion o actualizacion de un proveedor de salud
    public class ProveedorSaludValidator
    {
        public void Validar(ProveedorSalud proveedor)
        {
            // Regla: el nombre es obligatorio
            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                throw new InvalidOperationException(
                    "El nombre del proveedor de salud es obligatorio.");

            // Regla: el nombre debe tener al menos 3 caracteres
            if (proveedor.Nombre.Length < 3)
                throw new InvalidOperationException(
                    "El nombre del proveedor de salud debe tener al menos 3 caracteres.");

            // Regla: si se proporciona email, debe tener formato basico valido
            if (!string.IsNullOrWhiteSpace(proveedor.Email) && !proveedor.Email.Contains('@'))
                throw new InvalidOperationException(
                    "El email del proveedor de salud no tiene un formato valido.");
        }
    }
}
