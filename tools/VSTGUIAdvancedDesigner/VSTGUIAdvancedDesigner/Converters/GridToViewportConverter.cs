using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VSTGUIAdvancedDesigner.Converters;

public sealed class GridToViewportConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double cellSize && cellSize > 0)
        {
            return new Rect(0, 0, cellSize, cellSize);
        }

        return new Rect(0, 0, 16, 16);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
