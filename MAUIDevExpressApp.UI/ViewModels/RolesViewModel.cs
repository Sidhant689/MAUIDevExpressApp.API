using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class RolesViewModel : BaseViewModel
    {
        private readonly IRoleService _roleService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<RoleDTO> _rolesList;

        public RolesViewModel(IRoleService roleService, INavigationService navigationService, IDialogService dialogService)
        {
            Title = "Roles";
            _roleService = roleService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            RolesList = new ObservableCollection<RoleDTO>();
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadRoles();
        }

        [RelayCommand]
        private async Task LoadRoles()
        {
            try
            {
                IsBusy = true;
                var roles = await _roleService.GetAllRolesAsync();
                RolesList.Clear();
                foreach (var role in roles)
                {
                    RolesList.Add(role);
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
        private async Task DeleteRole(int id)
        {
            bool answer = await _dialogService.ShowConfirmationAsync("Delete", "Are you sure you want to delete this role?", "Yes", "No");
            if (!answer) return;

            try
            {
                IsBusy = true;
                await _roleService.DeleteRoleAsync(id);
                var roleToRemove = RolesList.FirstOrDefault(x => x.Id == id);
                if (roleToRemove != null)
                {
                    RolesList.Remove(roleToRemove);
                }
                await _dialogService.ShowSuccessAsync("Success", "Role deleted successfully");
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
        private async Task NavigateToAdd()
        {
            await _navigationService.NavigateToAsync(nameof(RoleDetailPage));
        }

        [RelayCommand]
        private async Task NavigateToEdit(RoleDTO role)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Role", role }
            };
            await _navigationService.NavigateToAsync(nameof(RoleDetailPage), parameters);
        }

        [RelayCommand]
        private async Task ManagePermissions(RoleDTO role)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Role", role }
            };
            await _navigationService.NavigateToAsync(nameof(RolePermissionsPage), parameters);
        }
    }
}
