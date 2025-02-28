using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using MAUIDevExpressApp.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class PermissionViewModel : BaseViewModel
    {
        private readonly IPermissionService _permissionService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<PermissionDTO> _permissionsList;

        public PermissionViewModel(IPermissionService permissionService, INavigationService navigationService, IDialogService dialogService)
        {
            Title = "Permissions";
            _permissionService = permissionService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            PermissionsList = new ObservableCollection<PermissionDTO>();
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadPermissions();
        }

        [RelayCommand]
        private async Task LoadPermissions()
        {
            try
            {
                IsBusy = true;
                var permissions = await _permissionService.GetAllPermissionsAsync();
                PermissionsList.Clear();
                foreach (var permission in permissions)
                {
                    PermissionsList.Add(permission);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void AddPermission()
        {
            _navigationService.NavigateToAsync(nameof(PermissionDetailPage));
        }

        [RelayCommand]
        private async Task NavigateToEdit(PermissionDTO permission)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Permission", permission }
            };
            await _navigationService.NavigateToAsync(nameof(PermissionDetailPage), parameters);
        }

        [RelayCommand]
        private async Task DeletePermission(int id)
        {
           
            var answer = await _dialogService.ShowConfirmationAsync("Delete Permission", "Are you sure you want to delete this permission?", "Yes", "No");
            if (!answer) return;

            try
            {
                IsBusy = true;
                await _permissionService.DeletePermissionAsync(id);
                var permissionToRemove = PermissionsList.FirstOrDefault(x => x.Id == id);
                if (permissionToRemove != null)
                {
                    PermissionsList.Remove(permissionToRemove);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
