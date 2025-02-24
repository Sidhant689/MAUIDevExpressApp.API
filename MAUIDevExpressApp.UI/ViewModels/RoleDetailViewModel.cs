using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class RoleDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IRoleService _roleService;
        private readonly INavigationService _navigation;

        [ObservableProperty]
        private RoleDTO _role;

        [ObservableProperty]
        private bool _isEditing;

        public RoleDetailViewModel(IRoleService roleService, INavigationService navigation)
        {
            _roleService = roleService;
            _navigation = navigation;
            Role = new RoleDTO();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("Role"))
            {
                Role = (RoleDTO)query["Role"];
                IsEditing = true;
                Title = "Edit Role";
            }
            else
            {
                Role = new RoleDTO();
                IsEditing = false;
                Title = "New Role";
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                IsBusy = true;

                if (IsEditing)
                {
                    await _roleService.UpdateRoleAsync(Role);
                }
                else
                {
                    await _roleService.CreateRoleAsync(Role);
                }

                await _navigation.GoBackAsync();
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
        private async Task Cancel()
        {
            await _navigation.GoBackAsync();
        }
    }
}
