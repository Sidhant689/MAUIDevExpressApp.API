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
        bool IsAuthenticated { get; }
        string CurrentUsername { get; }
        Task<LoginResponse> Login(string username, string password, bool rememberMe = false);
        Task<bool> TryAutoLogin();
        Task Logout();
        Task<HttpResponseMessage> Register(string username, string email, string password);
    }
}
