using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class RolePermissionViewModel : BaseViewModel
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IModuleService _moduleService;
        private readonly IRoleService _roleService;

        [ObservableProperty]
        private ObservableCollection<RoleDTO> roles = new();

        [ObservableProperty]
        private int selectedRoleId;

        [ObservableProperty]
        private ObservableCollection<ModuleDTO> modules = new();

        [ObservableProperty]
        private ObservableCollection<PermissionDTO> selectedPermissions = new();

        public RolePermissionViewModel(IRolePermissionService rolePermissionService, IModuleService moduleService, IRoleService roleService)
        {
            _rolePermissionService = rolePermissionService;
            _moduleService = moduleService;
            _roleService = roleService;

        }

        [RelayCommand]
        private async Task LoadRolesAsync()
        {
            var roleList = await _roleService.GetAllRolesAsync();
            Roles = new ObservableCollection<RoleDTO>(roleList);
        }

        //[RelayCommand]
        //private async Task LoadModulesAsync()
        //{
        //    var moduleList = await _moduleService.GetAllModulesAsync();
        //    foreach (var module in moduleList)
        //    {
        //        module..Pages = await _moduleService.GetPagesByModuleIdAsync(module.Id);
        //        foreach (var page in module.Pages)
        //        {
        //            page.Permissions = await _moduleService.GetPermissionsByPageIdAsync(page.Id);
        //        }
        //    }
        //    Modules = new ObservableCollection<ModuleDTO>(moduleList);
        //}

        //public async Task AssignPermissionsAsync()
        //{
        //    if (SelectedRoleId == 0 || SelectedPermissions.Count == 0)
        //        return;

        //    await _rolePermissionService.AssignPermissionsAsync(SelectedRoleId, SelectedPermissions.ToList());
        //    await LoadModulesAsync();
        //}

        //public async Task RemovePermissionsAsync()
        //{
        //    if (SelectedRoleId == 0 || SelectedPermissions.Count == 0)
        //        return;

        //    foreach (var permission in SelectedPermissions)
        //    {
        //        await _rolePermissionService.RemovePermissionAsync(SelectedRoleId, permission.Id);
        //    }

        //    await LoadModulesAsync();
        //}
    }
}
