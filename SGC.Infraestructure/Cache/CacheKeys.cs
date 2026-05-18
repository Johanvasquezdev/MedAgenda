using System;

namespace SGC.Infraestructure.Cache
{
    // Clase estática para definir las claves de caché utilizadas en la aplicación
    public static class CacheKeys
    {
        public static string Medico(int id) => $"medico:{id}";

        public static string MedicosPorEspecialidad(int especialidadId) => $"medicos:especialidad:{especialidadId}";

        public static string Disponibilidad(int medicoId) => $"disponibilidad:medico:{medicoId}";

        public static string DisponibilidadFecha(int medicoId, DateTime fecha) => $"disponibilidad:medico:{medicoId}:fecha:{fecha:yyyy-MM-dd}";
        public static string Especialidades() => "especialidades:todas";

        public static string Especialidad(int id) => $"especialidad:{id}";

        public static string Sesion(string token) => $"sesion:{token}";
    }
}
