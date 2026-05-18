using SGC.Domain.Enums;

namespace SGC.Domain.Entities.Security
{
    // La clase Administrador representa a un usuario con rol de administrador en el sistema. Hereda de Usuario para compartir las propiedades comunes (Nombre, Email, PasswordHash, Rol, Activo). A diferencia de Medico y Paciente, el administrador no tiene atributos adicionales propios ya que su funcion es gestionar usuarios, roles, especialidades y catalogos del sistema. Sin esta clase concreta, no seria posible instanciar un Usuario con rol Administrador porque Usuario es abstracto.
    public sealed class Administrador : Usuario
    {
        public Administrador()
        {
            Rol = RolUsuario.Administrador;
        }
    }
}
