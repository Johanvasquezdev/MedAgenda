using SGC.Application.DTOs.Medical;
using SGC.Domain.Entities.Medical;

namespace SGC.Application.Mappers
{
    // Mapper para convertir entre la entidad Paciente y los DTOs de solicitud/respuesta
    public static class PacienteMapper
    {
        public static PacienteResponse ToResponse(Paciente paciente)
        {
            return new PacienteResponse
            {
                Id = paciente.Id,
                Nombre = paciente.Nombre,
                Email = paciente.Email,
                Cedula = paciente.Cedula,
                Telefono = paciente.Telefono,
                FechaNacimiento = paciente.FechaNacimiento,
                Edad = paciente.CalcularEdad(),
                TipoSeguro = paciente.TipoSeguro,
                NumeroSeguro = paciente.NumeroSeguro,
                Activo = paciente.Activo
            };
        }

        // Para crear un nuevo paciente, se mapea el DTO de solicitud a la entidad. El PasswordHash se asigna directamente desde el Password del request, pero en una implementacion real se deberia hashear antes de guardar en la base de datos.
        public static Paciente ToEntity(CrearPacienteRequest request)
        {
            return new Paciente
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = request.Password,
                Cedula = request.Cedula,
                Telefono = request.Telefono,
                FechaNacimiento = request.FechaNacimiento,
                TipoSeguro = request.TipoSeguro,
                NumeroSeguro = request.NumeroSeguro
            };
        }

        // Para actualizar un paciente existente, se actualizan solo los campos que vienen en el DTO de solicitud (si no son null). El PasswordHash no se actualiza aqui, ya que se deberia manejar en un endpoint separado para cambiar la contraseña.
        public static void UpdateEntity(Paciente paciente,
            ActualizarPacienteRequest request)
        {
            paciente.Nombre = request.Nombre ?? paciente.Nombre;
            paciente.Telefono = request.Telefono ?? paciente.Telefono;
            paciente.TipoSeguro = request.TipoSeguro ?? paciente.TipoSeguro;
            paciente.NumeroSeguro = request.NumeroSeguro ?? paciente.NumeroSeguro;
        }
    }
}
