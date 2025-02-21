using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Converters
{
    public class PermissionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var permission = parameter as string; // Format: "Module:Action"
            if (string.IsNullOrEmpty(permission)) return true;

            var parts = permission.Split(':');
            if (parts.Length != 2) return true;

            var authService = Application.Current.Handler.MauiContext.Services
                .GetService<IAuthService>();

            return authService.HasPermission(parts[0], parts[1]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
