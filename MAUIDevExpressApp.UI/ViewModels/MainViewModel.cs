using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        public string _welcomeMessage;

        public MainViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            Title = "Home";
            WelcomeMessage = $"Welcome, {_authService.CurrentUsername}";
            _navigationService = navigationService;
        }

    }
}
