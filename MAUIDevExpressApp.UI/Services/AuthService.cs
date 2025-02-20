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
        private System.Timers.Timer _refreshTimer;
        private const int REFRESH_INTERVAL = 45; // minutes

        public bool IsAuthenticated => _currentUser != null;
        public string CurrentUsername => _currentUser?.Username;

        public AuthService(IAPIService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
            InitializeRefreshTimer();
        }

        private void InitializeRefreshTimer()
        {
            _refreshTimer = new System.Timers.Timer(REFRESH_INTERVAL * 60 * 1000);
            _refreshTimer.Elapsed += async (sender, e) => await RefreshToken();
        }

        public async Task<LoginResponse> Login(string username, string password, bool rememberMe = false)
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password,
                RememberMe = rememberMe
            };

            _currentUser = await _apiService.PostAsync<LoginRequest, LoginResponse>(
                "auth/login", request);

            if (_currentUser != null)
            {
                _apiService.SetAuthToken(_currentUser.Token);
                await SecureStorage.SetAsync("auth_token", _currentUser.Token);
                await SecureStorage.SetAsync("refresh_token", _currentUser.RefreshToken);
                await SecureStorage.SetAsync("username", _currentUser.Username);

                if (rememberMe)
                {
                    await SecureStorage.SetAsync("remember_me", "true");
                    await SecureStorage.SetAsync("stored_username", username);
                    await SecureStorage.SetAsync("stored_password",
                        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)));
                }

                _refreshTimer.Start();
            }

            return _currentUser;
        }

        private async Task RefreshToken()
        {
            if (!IsAuthenticated) return;

            try
            {
                var refreshRequest = new RefreshTokenRequest
                {
                    Token = _currentUser.Token,
                    RefreshToken = _currentUser.RefreshToken
                };

                var response = await _apiService.PostAsync<RefreshTokenRequest, RefreshTokenResponse>(
                    "auth/refresh", refreshRequest);

                if (response?.Token != null)
                {
                    _currentUser.Token = response.Token;
                    _currentUser.RefreshToken = response.RefreshToken;
                    _apiService.SetAuthToken(response.Token);
                    await SecureStorage.SetAsync("auth_token", response.Token);
                    await SecureStorage.SetAsync("refresh_token", response.RefreshToken);
                }
            }
            catch (Exception)
            {
                await Logout();
            }
        }

        public async Task<bool> TryAutoLogin()
        {
            var rememberMe = await SecureStorage.GetAsync("remember_me");
            if (rememberMe == "true")
            {
                var username = await SecureStorage.GetAsync("stored_username");
                var encryptedPassword = await SecureStorage.GetAsync("stored_password");

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(encryptedPassword))
                {
                    var password = System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(encryptedPassword));
                    var response = await Login(username, password, true);
                    return response != null;
                }
            }
            return false;
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
            _refreshTimer.Stop();

            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("username");
            SecureStorage.Remove("remember_me");
            SecureStorage.Remove("stored_username");
            SecureStorage.Remove("stored_password");

            _apiService.SetAuthToken(null);
        }

    }
}
