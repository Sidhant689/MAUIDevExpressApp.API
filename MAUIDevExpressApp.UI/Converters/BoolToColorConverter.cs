using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return Colors.Transparent;

            bool boolValue = (bool)value;
            string[] colors = parameter.ToString().Split(',');

            if (colors.Length != 2)
                return Colors.Transparent;

            // Use the built-in Colors instead of trying to parse color names
            string colorName = boolValue ? colors[0].Trim() : colors[1].Trim();

            return colorName switch
            {
                "Gray" => Colors.Gray,
                "Blue" => Colors.Blue,
                "LightGray" => Colors.LightGray,
                "White" => Colors.White,
                "Red" => Colors.Red,
                "Green" => Colors.Green,
                // Add more colors as needed
                _ => Colors.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
