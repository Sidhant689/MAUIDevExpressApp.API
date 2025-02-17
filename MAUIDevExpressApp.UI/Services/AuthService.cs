using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;

namespace MAUIDevExpressApp.UI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAPIService _apiService;
        private LoginResponse _currentUser;

        public bool IsAuthenticated => _currentUser != null;
        public string CurrentUsername => _currentUser?.Username;

        public AuthService(IAPIService apiService)
        {
            _apiService = apiService;
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


        public async Task Logout()
        {
            _currentUser = null;
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("username");
        }

    }
}
