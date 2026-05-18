using SGC.Domain.Entities.Security;

namespace SGC.Domain.Validators
{
    // Valida las reglas de negocio basicas para cualquier usuario del sistema
    public class UsuarioValidator
    {
        public void Validar(Usuario usuario)
        {
            // Regla: el nombre es obligatorio
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new InvalidOperationException(
                    "El nombre del usuario es obligatorio.");

            // Regla: el email es obligatorio
            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new InvalidOperationException(
                    "El email del usuario es obligatorio.");

            // Regla: el email debe tener un formato basico valido
            if (!usuario.Email.Contains('@'))
                throw new InvalidOperationException(
                    "El email del usuario no tiene un formato valido.");

            // Regla: la contraseña es obligatoria
            if (string.IsNullOrWhiteSpace(usuario.PasswordHash))
                throw new InvalidOperationException(
                    "La contraseña del usuario es obligatoria.");
        }
    }
}
