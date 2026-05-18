namespace DoctorApp.Models;

/// <summary>
/// Modelo para representar un paciente en el panel de consultas del día
/// </summary>
public class PacienteConsultaModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DateTime HoraConsulta { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public EstadoConsulta Estado { get; set; } = EstadoConsulta.Esperando;
    public string Cedula { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Edad { get; set; } = string.Empty;
    public string AntecedentesRelevantes { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Propiedades calculadas
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string HoraFormato => HoraConsulta.ToString("HH:mm");
    public string DuracionDesdeAhora
    {
        get
        {
            var ahora = DateTime.Now;
            var diferencia = HoraConsulta - ahora;

            if (diferencia.TotalMinutes < 0)
                return "Vencida";
            else if (diferencia.TotalMinutes < 5)
                return "Ahora";
            else if (diferencia.TotalMinutes < 60)
                return $"En {(int)diferencia.TotalMinutes}m";
            else
                return $"En {diferencia.Hours}h {diferencia.Minutes}m";
        }
    }

    public Color ColorEstado
    {
        get => Estado switch
        {
            EstadoConsulta.Esperando => Color.FromArgb("#F59E0B"),    // Amarillo
            EstadoConsulta.EnConsulta => Color.FromArgb("#3B82F6"),   // Azul
            EstadoConsulta.Completada => Color.FromArgb("#10B981"),   // Verde
            EstadoConsulta.NoPresento => Color.FromArgb("#EF4444"),   // Rojo
            EstadoConsulta.Reprogramada => Color.FromArgb("#8B5CF6"), // Púrpura
            _ => Colors.Gray
        };
    }

    public string TextoEstado
    {
        get => Estado switch
        {
            EstadoConsulta.Esperando => "⏳ Esperando",
            EstadoConsulta.EnConsulta => "🏥 En Consulta",
            EstadoConsulta.Completada => "✅ Completada",
            EstadoConsulta.NoPresento => "❌ No Presentó",
            EstadoConsulta.Reprogramada => "📅 Reprogramada",
            _ => "Desconocido"
        };
    }
}

/// <summary>
/// Enum para los estados de una consulta
/// </summary>
public enum EstadoConsulta
{
    Esperando,
    EnConsulta,
    Completada,
    NoPresento,
    Reprogramada
}
