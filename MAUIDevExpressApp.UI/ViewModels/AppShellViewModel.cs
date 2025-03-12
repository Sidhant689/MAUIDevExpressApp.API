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
    public partial class AppShellViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _currentUsername;

        [ObservableProperty]
        private bool _isLoggedIn;

        public AppShellViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            // Initialize properties
            UpdateUserInfo();
        }

        private void UpdateUserInfo()
        {
            CurrentUsername = _authService.CurrentUsername;
            IsLoggedIn = _authService.IsLoggedIn;
        }


        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _authService.Logout();
            await _navigationService.NavigateToAsync("//LoginPage");

            if (Shell.Current?.BindingContext is AppShellViewModel shellViewModel)
            {
                shellViewModel.CurrentUsername = _authService.CurrentUsername;
                shellViewModel.IsLoggedIn = _authService.IsLoggedIn;
            }

        }
    }

}
