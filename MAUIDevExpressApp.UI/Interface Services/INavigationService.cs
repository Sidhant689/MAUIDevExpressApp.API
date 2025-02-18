using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface INavigationService
    {
        Task NavigateToAsync(string route, Dictionary<string, object> parameters = null);
        Task GoBackAsync();
    }

}
