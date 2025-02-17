using MAUIDevExpressApp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(string username, string password);
        Task<HttpResponseMessage> Register(string username, string email, string password);
        Task Logout();
        bool IsAuthenticated { get; }
        string CurrentUsername { get; }

    }
}
