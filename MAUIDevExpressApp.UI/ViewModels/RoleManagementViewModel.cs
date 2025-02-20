using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class RoleManagementViewModel : BaseViewModel
    {
        private readonly IAPIService _apiService;

        [ObservableProperty]
        private ObservableCollection<RoleDto> roles;

        [ObservableProperty]
        private List<string> availablePermissions;

        [ObservableProperty]
        private RoleDto selectedRole;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string roleName;

        [ObservableProperty]
        private string roleDescription;

        [ObservableProperty]
        private ObservableCollection<string> selectedPermissions;

        public RoleManagementViewModel(IAPIService apiService)
        {
            _apiService = apiService;
            SelectedPermissions = new ObservableCollection<string>();
            LoadData();
        }

        private async Task LoadData()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Load roles and permissions in parallel
                var rolesTask = _apiService.GetAsync<List<RoleDto>>("roles");
                var permissionsTask = _apiService.GetAsync<List<string>>("roles/permissions");

                await Task.WhenAll(rolesTask, permissionsTask);

                Roles = new ObservableCollection<RoleDto>(await rolesTask);
                AvailablePermissions = await permissionsTask;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CreateRole()
        {
            if (string.IsNullOrWhiteSpace(RoleName))
            {
                await Shell.Current.DisplayAlert("Error", "Role name is required", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var request = new CreateRoleRequest
                {
                    Name = RoleName,
                    Description = RoleDescription,
                    Permissions = SelectedPermissions.ToList()
                };

                var newRole = await _apiService.PostAsync<CreateRoleRequest, RoleDto>("roles", request);
                Roles.Add(newRole);

                // Clear form
                RoleName = string.Empty;
                RoleDescription = string.Empty;
                SelectedPermissions.Clear();

                await Shell.Current.DisplayAlert("Success", "Role created successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateRole()
        {
            if (SelectedRole == null) return;

            try
            {
                IsBusy = true;

                var request = new UpdateRoleRequest
                {
                    Name = RoleName,
                    Description = RoleDescription,
                    Permissions = SelectedPermissions.ToList()
                };

                await _apiService.PostAsync($"roles/{SelectedRole.Id}", request);

                // Update the role in the collection
                var updatedRole = new RoleDto
                {
                    Id = SelectedRole.Id,
                    Name = RoleName,
                    Description = RoleDescription,
                    Permissions = SelectedPermissions.ToList()
                };

                var index = Roles.IndexOf(SelectedRole);
                Roles[index] = updatedRole;

                IsEditing = false;
                await Shell.Current.DisplayAlert("Success", "Role updated successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteRole(RoleDto role)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the role '{role.Name}'?",
                "Yes", "No");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _apiService.DeleteAsync($"roles/{role.Id}");
                Roles.Remove(role);
                await Shell.Current.DisplayAlert("Success", "Role deleted successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void EditRole(RoleDto role)
        {
            SelectedRole = role;
            RoleName = role.Name;
            RoleDescription = role.Description;
            SelectedPermissions = new ObservableCollection<string>(role.Permissions);
            IsEditing = true;
        }

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditing = false;
            SelectedRole = null;
            RoleName = string.Empty;
            RoleDescription = string.Empty;
            SelectedPermissions.Clear();
        }

        [RelayCommand]
        private void TogglePermission(string permission)
        {
            if (SelectedPermissions.Contains(permission))
                SelectedPermissions.Remove(permission);
            else
                SelectedPermissions.Add(permission);
        }
    }
}
