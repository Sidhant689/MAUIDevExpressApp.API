using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using Microsoft.Extensions.Configuration;

namespace MAUIDevExpressApp.UI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAPIService _apiService;
        private readonly IConfiguration _configuration;
        private LoginResponse _currentUser;

        public bool IsAuthenticated => _currentUser != null;
        public string CurrentUsername => _currentUser?.Username;

        public AuthService(IAPIService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        public async Task<LoginResponse> Login(string username, string password)
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };

            _currentUser = await _apiService.PostAsync<LoginRequest, LoginResponse>(
                "auth/login", request);

            if (_currentUser != null)
            {
                _apiService.SetAuthToken(_currentUser.Token);
                await SecureStorage.SetAsync("auth_token", _currentUser.Token);
                await SecureStorage.SetAsync("username", _currentUser.Username);
                Preferences.Set("login_timestamp", DateTime.UtcNow.ToString("o")); // Save login time
            }

            return _currentUser;
        }

        public async Task<HttpResponseMessage> Register(string username, string email, string password)
        {
            var request = new RegisterRequest
            {
                Username = username,
                Email = email,
                Password = password
            };

            var response = await _apiService.PostAsync("auth/register", request);

            return response;  // Return full response so ViewModel can handle errors
        }

        public async Task<bool> IsSessionValidAsync()
        {
            string token = await SecureStorage.GetAsync("auth_token");
            string loginTimeStr = Preferences.Get("login_timestamp", string.Empty);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(loginTimeStr))
            {
                return false; // No session found
            }

            if (!DateTime.TryParse(loginTimeStr, out DateTime loginTime))
            {
                return false; // Invalid timestamp
            }

            // ✅ Safe retrieval of login time with default value
            int totalLoginAllowed = 30; // Default to 30 minutes if not found
            if (int.TryParse(_configuration["Login_time"], out int configLoginTime))
            {
                totalLoginAllowed = configLoginTime;
            }

            if ((DateTime.UtcNow - loginTime).TotalMinutes > totalLoginAllowed)
            {
                SecureStorage.Remove("auth_token"); // ✅ Properly clear token
                Preferences.Remove("login_timestamp");
                return false; // Session expired
            }

            return true; // Session is still valid
        }

        public async Task Logout()
        {
            _currentUser = null;
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("username");
        }

    }
}
