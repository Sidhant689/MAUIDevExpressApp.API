using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool rememberMe;

        [ObservableProperty]
        private bool isLoading;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Login";

            // Try auto-login when view model is created
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                IsLoading = true;
                // Attempt auto-login if credentials are stored
                if (await _authService.TryAutoLogin())
                {
                    await NavigateToMainPage();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Auto-login failed: " + ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password";
                return;
            }

            try
            {
                IsBusy = true;
                IsLoading = true;
                ErrorMessage = string.Empty;

                var response = await _authService.Login(Username, Password, RememberMe);
                if (response != null)
                {
                    // Clear sensitive data
                    Password = string.Empty;
                    await NavigateToMainPage();
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = "Network error: Please check your internet connection";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Login failed: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
                IsLoading = false;
            }
        }

        private async Task NavigateToMainPage()
        {
            await _navigationService.NavigateToAsync("///MainPage");

            // Update the Shell's ViewModel with the new username
            if (Shell.Current?.BindingContext is AppShellViewModel shellViewModel)
            {
                shellViewModel.CurrentUsername = _authService.CurrentUsername;
                shellViewModel.IsLoggedIn = _authService.IsLoggedIn;
            }

            // Enable flyout after successful login
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await _navigationService.NavigateToAsync("///RegisterPage");
        }
    }
}