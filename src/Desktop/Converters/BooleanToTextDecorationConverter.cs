using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Desktop.Converters;

public class BooleanToTextDecorationConverter:IValueConverter
{
    public static readonly BooleanToTextDecorationConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isCompleted and true)
        {
            return TextDecorations.Strikethrough;
        }
        return null!;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}