using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MAUIDevExpressApp.UI.Converters
{
    public class BoolToExpandImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? "bx_chevrons_up.png" : "bx_chevrons_down.png";
            }
            return "bx_chevrons_down.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}