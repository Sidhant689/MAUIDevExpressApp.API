using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string errorMessage;

        public RegisterViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Register";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Please fill in all fields";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var response = await _authService.Register(Username, Email, Password);

                if (response.IsSuccessStatusCode)
                {
                    await _navigationService.NavigateToAsync("///LoginPage");
                }
                else
                {
                    // Read and display the error message from API response
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Registration failed: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
