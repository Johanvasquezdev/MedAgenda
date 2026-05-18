using System.Globalization;

namespace DoctorApp.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isAvailable)
        {
            return isAvailable ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");
        }
        return Colors.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToTextoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isAvailable)
        {
            return isAvailable ? "✓ Disponible" : "✕ No Disponible";
        }
        return "N/A";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EstadoColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool confirmada)
        {
            return confirmada ? Color.FromArgb("#10B981") : Color.FromArgb("#F59E0B");
        }
        return Color.FromArgb("#9CA3AF");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EstadoTextoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool confirmada)
        {
            return confirmada ? "✓ Confirmada" : "⏳ Pendiente";
        }
        return "N/A";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EstadoCitaColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DoctorApp.Models.EstadoCita estado)
        {
            return estado switch
            {
                DoctorApp.Models.EstadoCita.Confirmada => Color.FromArgb("#10B981"),
                DoctorApp.Models.EstadoCita.Pendiente => Color.FromArgb("#F59E0B"),
                DoctorApp.Models.EstadoCita.EnCurso => Color.FromArgb("#3B82F6"),
                DoctorApp.Models.EstadoCita.Completada => Color.FromArgb("#6B7280"),
                DoctorApp.Models.EstadoCita.Cancelada => Color.FromArgb("#EF4444"),
                DoctorApp.Models.EstadoCita.Reprogramada => Color.FromArgb("#8B5CF6"),
                DoctorApp.Models.EstadoCita.NoAsistio => Color.FromArgb("#DC2626"),
                _ => Colors.Gray
            };
        }
        return Colors.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EstadoCitaTextoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DoctorApp.Models.EstadoCita estado)
        {
            return estado switch
            {
                DoctorApp.Models.EstadoCita.Pendiente => "Pendiente",
                DoctorApp.Models.EstadoCita.Confirmada => "Confirmada",
                DoctorApp.Models.EstadoCita.EnCurso => "En curso",
                DoctorApp.Models.EstadoCita.Completada => "Completada",
                DoctorApp.Models.EstadoCita.Cancelada => "Cancelada",
                DoctorApp.Models.EstadoCita.Reprogramada => "Reprogramada",
                DoctorApp.Models.EstadoCita.NoAsistio => "No asistio",
                DoctorApp.Models.EstadoCita.Todos => "Todos",
                _ => "N/A"
            };
        }

        return "N/A";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IntToNegativeBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            return count == 0;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectionColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool?)value == true ? Color.FromArgb("#3B82F6") : Color.FromArgb("#E5E7EB");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EqualsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PuedoConfirmarConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DoctorApp.Models.EstadoCita estado)
        {
            return estado == DoctorApp.Models.EstadoCita.Pendiente;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Nuevo converter para Panel de Consultas
public class PuedoIniciarConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DoctorApp.Models.EstadoConsulta estado)
        {
            return estado == DoctorApp.Models.EstadoConsulta.Esperando;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
