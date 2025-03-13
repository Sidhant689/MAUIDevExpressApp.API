using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels.ControlsViewModel
{
    public partial class CustomFlyoutViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _userManagementExpanded;

        [ObservableProperty]
        private bool _productsExpanded;

        [ObservableProperty]
        private bool _settingsExpanded;

        public CustomFlyoutViewModel()
        {
            // Initialize expanded states
            UserManagementExpanded = false;
            ProductsExpanded = false;
            SettingsExpanded = false;
        }

        [RelayCommand]
        private void ToggleExpand(string section)
        {
            switch (section)
            {
                case "UserManagement":
                    UserManagementExpanded = !UserManagementExpanded;
                    break;
                case "Products":
                    ProductsExpanded = !ProductsExpanded;
                    break;
                case "Settings":
                    SettingsExpanded = !SettingsExpanded;
                    break;
            }
        }

        [RelayCommand]
        private async Task Navigate(string route)
        {
            await Shell.Current.GoToAsync(route);
            // Close the flyout after navigation
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}
