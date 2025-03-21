using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using System;
using Syncfusion.TreeView.Engine;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class RolePermissionViewModel : BaseViewModel
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IModuleService _moduleService;
        private readonly IPageService _pageService;
        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;

        [ObservableProperty]
        private ObservableCollection<RoleDTO> roles = new();

        [ObservableProperty]
        private int selectedRoleId;

        [ObservableProperty]
        private RoleDTO selectedRole;

        [ObservableProperty]
        private ObservableCollection<ModuleDTO> modules = new();

        [ObservableProperty]
        private ObservableCollection<PermissionDTO> selectedPermissions = new();

        [ObservableProperty]
        private ObservableCollection<PermissionWithSelectionDTO> availablePermissions = new();

        [ObservableProperty]
        private string selectedItemTitle = "Select a Module or Page";

        [ObservableProperty]
        private object selectedTreeItem;

        [ObservableProperty]
        private ObservableCollection<object> checkedTreeItems = new();

        [ObservableProperty]
        private Dictionary<int, List<PermissionWithSelectionDTO>> _permissionsByContainer = new();

        public RolePermissionViewModel(IRolePermissionService rolePermissionService, IModuleService moduleService, IRoleService roleService, IPageService pageService, IPermissionService permissionService)
        {
            _rolePermissionService = rolePermissionService;
            _moduleService = moduleService;
            _roleService = roleService;
            _pageService = pageService;
            _permissionService = permissionService;
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadModulesAsync();
            await LoadRolesAsync();
        }

        #region Load Methods

        [RelayCommand]
        private async Task LoadRolesAsync()
        {
            var roleList = await _roleService.GetAllRolesAsync();
            Roles = new ObservableCollection<RoleDTO>(roleList);
        }

        [RelayCommand]
        private async Task LoadModulesAsync()
        {
            var moduleList = await _moduleService.GetAllModulesAsync();
            foreach (var module in moduleList)
            {
                var pages = await _pageService.GetPagesByModuleIdAsync(module.Id);
                module.Pages = new ObservableCollection<PageDTO>(pages);
                foreach (var page in module.Pages)
                {
                    page.Permissions = new ObservableCollection<PermissionDTO>(await _permissionService.GetPermissionsByPageIdAsync(page.Id));
                }
            }
            Modules = new ObservableCollection<ModuleDTO>(moduleList);
        }

        private async Task LoadRolePermissionsAsync()
        {
            if (SelectedRole == null)
                return;

            SelectedPermissions.Clear();
            var rolePermissions = await _rolePermissionService.GetRolePermissionsByRoleIdAsync(SelectedRole.Id);
            foreach (var permission in rolePermissions)
            {
                SelectedPermissions.Add(permission.Permission);
            }
        }

        #endregion

        #region Permission Management Methods

        [RelayCommand]
        private async Task AssignPermissionsAsync()
        {
            if (SelectedRole == null)
                return;

            var permissionsToAssign = new List<int>();

            // Add permissions from the right panel that are checked (now sourced from our dictionary)
            permissionsToAssign.AddRange(AvailablePermissions
                .Where(p => p.IsSelected)
                .Select(p => p.Id));

            // Add permissions from tree items that are checked
            foreach (var item in CheckedTreeItems)
            {
                if (item is TreeViewNode node)
                {
                    if (node.Content is ModuleDTO module)
                    {
                        // Get module ID to find permissions in the dictionary
                        int moduleId = module.Id;

                        // If we have stored permissions for this module in our dictionary
                        foreach (var page in module.Pages)
                        {
                            permissionsToAssign.AddRange(page.Permissions.Select(p => p.Id));
                        }
                    }
                    else if (node.Content is PageDTO page)
                    {
                        // Get page ID
                        int pageId = page.Id;

                        // Add all permissions for this page
                        permissionsToAssign.AddRange(page.Permissions.Select(p => p.Id));
                    }
                }
            }

            // Remove duplicates
            permissionsToAssign = permissionsToAssign.Distinct().ToList();

            if (permissionsToAssign.Count == 0)
                return;

            await _rolePermissionService.AssignPermissionsToRoleAsync(SelectedRole.Id, permissionsToAssign);

            // Refresh role permissions
            await LoadRolePermissionsAsync();

            // Update the available permissions display to reflect the new role permissions
            UpdateAvailablePermissionsDisplay();
        }

        [RelayCommand]
        private async Task RemovePermissionsAsync()
        {
            try
            {
                if (SelectedRole == null)
                    return;

                var permissionsToRemove = new List<int>();

                // Add permissions from the right panel that are checked
                permissionsToRemove.AddRange(AvailablePermissions
                    .Where(p => p.IsSelected)
                    .Select(p => p.Id));

                // Add permissions from tree items that are checked
                foreach (var item in CheckedTreeItems)
                {
                    if (item is TreeViewNode node)
                    {
                        if (node.Content is ModuleDTO module)
                        {
                            foreach (var page in module.Pages)
                            {
                                permissionsToRemove.AddRange(page.Permissions.Select(p => p.Id));
                            }
                        }
                        else if (node.Content is PageDTO page)
                        {
                            permissionsToRemove.AddRange(page.Permissions.Select(p => p.Id));
                        }
                    }
                }

                // Remove duplicates
                permissionsToRemove = permissionsToRemove.Distinct().ToList();

                if (permissionsToRemove.Count == 0)
                    return;

                await _rolePermissionService.RemovePermissionFromRoleAsync(SelectedRole.Id, permissionsToRemove);

                // Refresh role permissions
                await LoadRolePermissionsAsync();

                // Update our UI to reflect the changes
                UpdateAvailablePermissionsDisplay();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error removing permissions: {ex.Message}");
            }
        }

        [RelayCommand]
        private void PermissionChecked(PermissionWithSelectionDTO permission)
        {
            if (permission.IsSelected)
            {
                if (!SelectedPermissions.Any(p => p.Id == permission.Id))
                {
                    SelectedPermissions.Add(permission);
                }
            }
            else
            {
                var permToRemove = SelectedPermissions.FirstOrDefault(p => p.Id == permission.Id);
                if (permToRemove != null)
                {
                    SelectedPermissions.Remove(permToRemove);
                }
            }
        }

        #endregion

        #region Module and Page Permission Methods

        public async Task LoadModulePermissionsAsync(ModuleDTO module)
        {
            await UpdatePermissionsForContainer(module, true);
        }

        public async Task LoadPagePermissionsAsync(PageDTO page)
        {
            await UpdatePermissionsForContainer(page, true);
        }

        public void RemoveModulePermissions(ModuleDTO module)
        {
            UpdatePermissionsForContainer(module, false).Wait();
        }

        public async Task RemovePagePermissions(PageDTO page)
        {
            await UpdatePermissionsForContainer(page, false);
        }

        public async Task UpdatePermissionsForContainer(object container, bool isChecked)
        {
            // Get the container ID and permissions
            int containerId;
            List<PermissionDTO> permissions = new();
            string containerName = "";

            if (container is ModuleDTO module)
            {
                containerId = module.Id;
                containerName = module.Name;
                foreach (var page in module.Pages)
                {
                    permissions.AddRange(page.Permissions);
                }
            }
            else if (container is PageDTO page)
            {
                containerId = page.Id;
                containerName = page.Name;
                permissions = page.Permissions.ToList();
            }
            else
            {
                return;
            }

            // If checked, add or update permissions for this container
            if (isChecked)
            {
                // Update selected item title
                SelectedItemTitle = $"Permissions for {containerName}";

                // Load role permissions if needed
                if (SelectedPermissions.Count == 0)
                {
                    await LoadRolePermissionsAsync();
                }

                // Create selection DTOs
                var permissionsWithSelection = permissions.Select(p =>
                    new PermissionWithSelectionDTO(p, SelectedPermissions.Any(sp => sp.Id == p.Id))
                ).ToList();

                // Store in dictionary
                PermissionsByContainer[containerId] = permissionsWithSelection;

                foreach (var permission in permissionsWithSelection)
                {
                    permission.IsSelected = true;
                }

                // Update available permissions
                UpdateAvailablePermissionsDisplay();
            }
            else
            {
                // Remove from dictionary
                if (PermissionsByContainer.ContainsKey(containerId))
                {
                    PermissionsByContainer.Remove(containerId);
                }

                // Update available permissions
                UpdateAvailablePermissionsDisplay();
            }
        }

        #endregion

        #region Helper Methods

        // Update the display based on the current state
        private void UpdateAvailablePermissionsDisplay()
        {
            // Combine all permissions from selected containers
            var allPermissions = new List<PermissionWithSelectionDTO>();
            foreach (var containerPermissions in PermissionsByContainer.Values)
            {
                allPermissions.AddRange(containerPermissions);
            }

            // Remove duplicates by ID
            allPermissions = allPermissions
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();

            // Update the observable collection
            AvailablePermissions = new ObservableCollection<PermissionWithSelectionDTO>(allPermissions);

            System.Diagnostics.Debug.WriteLine($"Updated AvailablePermissions count = {AvailablePermissions.Count}");
        }

        #endregion
    }
}
