using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using MAUIDevExpressApp.UI.ViewModels.Nodes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    [QueryProperty(nameof(EntityToEdit), "Role")]
    public partial class MultiFormRolesViewModel : MultiFormBaseViewModel<RoleDTO>
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IModuleService _moduleService;

        [ObservableProperty]
        private ObservableCollection<ModuleNodeViewModel> _moduleNodes;

        [ObservableProperty]
        private string _permissionSearchText;

        [ObservableProperty]
        private bool _isLoadingPermissions;

        // To track the original permissions for comparison when saving
        private List<int> _originalPermissionIds;

        public MultiFormRolesViewModel(IRoleService roleService, IPermissionService permissionService, IModuleService moduleService, INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "Roles";
            _roleService = roleService;
            _permissionService = permissionService;
            _moduleService = moduleService;
            ModuleNodes = new ObservableCollection<ModuleNodeViewModel>();
        }

        // Override the GetEditTitle method to provide role-specific titles
        protected override string GetEditTitle(RoleDTO role)
        {
            return !string.IsNullOrWhiteSpace(role.Name) ? $"Edit {role.Name}" : "Edit Role";
        }

        protected override async void CreateNewForm()
        {
            base.CreateNewForm();
            if (FormManager.CurrentForm != null)
            {
                await LoadPermissionsForRole(FormManager.CurrentForm.Id);
            }
        }

        private async Task LoadPermissionsForRole(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form == null) return;

            try
            {
                IsLoadingPermissions = true;

                // Clear existing nodes
                ModuleNodes.Clear();

                // Load all modules and their permissions
                var modules = await _moduleService.GetAllModulesAsync();

                // If it's an existing role, load its permissions
                List<int> rolePermissionIds = new();
                if (form.IsEditing && form.Entity.Id > 0)
                {
                    var rolePermissions = await _roleService.GetRolePermissionsAsync(form.Entity.Id);
                    rolePermissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();
                }

                // Save as original for comparison during save
                _originalPermissionIds = new List<int>(rolePermissionIds);

                // Create module nodes with their permissions
                foreach (var module in modules.Where(m => m.IsActive))
                {
                    var moduleNode = new ModuleNodeViewModel
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Description = module.Description,
                        IsModule = true,
                        Permissions = new ObservableCollection<ModuleNodeViewModel>()
                    };

                    // Load permissions for this module
                    var permissions = await _permissionService.GetPermissionsByModuleAsync(module.Id);

                    foreach (var permission in permissions.Where(p => p.IsActive))
                    {
                        var permissionNode = new ModuleNodeViewModel
                        {
                            Id = permission.Id,
                            Name = permission.Name,
                            Description = permission.Description,
                            Action = permission.Action,
                            IsModule = false,
                            IsSelected = rolePermissionIds.Contains(permission.Id)
                        };

                        moduleNode.Permissions.Add(permissionNode);
                    }

                    // Automatically check/uncheck module based on its permissions
                    UpdateModuleSelection(moduleNode);

                    ModuleNodes.Add(moduleNode);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", $"Failed to load permissions: {ex.Message}");
            }
            finally
            {
                IsLoadingPermissions = false;
            }
        }

        public override void UpdateFormTitle(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null && !string.IsNullOrWhiteSpace(form.Entity.Name))
            {
                // Update the title based on whether it's an edit or new form
                form.Title = form.IsEditing
                    ? $"Edit {form.Entity.Name}"
                    : $"{form.Entity.Name}";

                // Also mark as having unsaved changes
                form.HasUnsavedChanges = true;
            }
        }

        protected override async Task SaveForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form == null) return;

            try
            {
                form.IsBusy = true;
                form.HasUnsavedChanges = false;

                RoleDTO savedRole;
                if (form.IsEditing)
                {
                    // Update the role
                    savedRole = await _roleService.UpdateRoleAsync(form.Entity);
                }
                else
                {
                    // Create the role
                    savedRole = await _roleService.CreateRoleAsync(form.Entity);
                    // Update with ID and any server-side changes
                    form.Entity.Id = savedRole.Id;
                    form.IsEditing = true;
                }

                // Save the permissions
                await SaveRolePermissions(savedRole.Id);

                await _dialogService.ShowSuccessAsync("Success",
                    form.IsEditing ? "Role updated successfully" : "Role created successfully");

                // Close form after successful save
                FormManager.CloseSession(formId);
            }
            catch (Exception ex)
            {
                form.HasUnsavedChanges = true;
                await _dialogService.ShowErrorAsync("Error", ex.Message);
            }
            finally
            {
                form.IsBusy = false;
            }
        }

        private async Task SaveRolePermissions(int roleId)
        {
            // Get all selected permissions across all modules
            var selectedPermissionIds = new List<int>();

            foreach (var module in ModuleNodes)
            {
                foreach (var permission in module.Permissions)
                {
                    if (permission.IsSelected)
                    {
                        selectedPermissionIds.Add(permission.Id);
                    }
                }
            }

            // Determine which permissions to add and which to remove
            var permissionsToAdd = selectedPermissionIds
                .Except(_originalPermissionIds)
                .ToList();

            var permissionsToRemove = _originalPermissionIds
                .Except(selectedPermissionIds)
                .ToList();

            // Add new permissions
            if (permissionsToAdd.Any())
            {
                await _roleService.AddPermissionsToRoleAsync(roleId, permissionsToAdd);
            }

            // Remove permissions
            if (permissionsToRemove.Any())
            {
                await _roleService.RemovePermissionsFromRoleAsync(roleId, permissionsToRemove);
            }

            // Update our tracking list
            _originalPermissionIds = new List<int>(selectedPermissionIds);
        }

        public void PermissionChecked(ModuleNodeViewModel node)
        {
            if (node == null) return;

            if (node.IsModule)
            {
                // If it's a module, check/uncheck all permissions
                foreach (var permission in node.Permissions)
                {
                    permission.IsSelected = node.IsSelected;
                }
            }
            else
            {
                // Update parent module selection based on children
                var parentModule = ModuleNodes.FirstOrDefault(m =>
                    m.Permissions.Any(p => p.Id == node.Id));

                if (parentModule != null)
                {
                    UpdateModuleSelection(parentModule);
                }
            }

            // Mark form as having unsaved changes
            var form = FormManager.CurrentForm;
            if (form != null)
            {
                form.HasUnsavedChanges = true;
            }
        }

        private void UpdateModuleSelection(ModuleNodeViewModel module)
        {
            if (module == null || !module.IsModule) return;

            bool allSelected = module.Permissions.Count > 0 &&
                               module.Permissions.All(p => p.IsSelected);
            bool anySelected = module.Permissions.Any(p => p.IsSelected);

            // Update module selection without triggering events
            module.IsSelected = allSelected;
            module.IsIndeterminate = anySelected && !allSelected;
        }

        [RelayCommand]
        private async Task SearchPermissions()
        {
            if (string.IsNullOrWhiteSpace(PermissionSearchText))
            {
                // If search text is cleared, reload all permissions
                await LoadPermissionsForRole(FormManager.CurrentForm?.Id);
                return;
            }

            try
            {
                IsLoadingPermissions = true;

                // Keep a copy of the full module nodes
                var allModules = new ObservableCollection<ModuleNodeViewModel>(ModuleNodes);
                ModuleNodes.Clear();

                string searchText = PermissionSearchText.ToLowerInvariant();

                foreach (var moduleNode in allModules)
                {
                    // Check if module name matches
                    bool moduleMatches = moduleNode.Name.ToLowerInvariant().Contains(searchText) ||
                                         (moduleNode.Description?.ToLowerInvariant()?.Contains(searchText) ?? false);

                    // Filter permissions
                    var matchingPermissions = moduleNode.Permissions
                        .Where(p => p.Name.ToLowerInvariant().Contains(searchText) ||
                                    (p.Description?.ToLowerInvariant()?.Contains(searchText) ?? false) ||
                                    (p.Action?.ToLowerInvariant()?.Contains(searchText) ?? false))
                        .ToList();

                    // Add module if it matches or has matching permissions
                    if (moduleMatches || matchingPermissions.Any())
                    {
                        var filteredModule = new ModuleNodeViewModel
                        {
                            Id = moduleNode.Id,
                            Name = moduleNode.Name,
                            Description = moduleNode.Description,
                            IsModule = true,
                            IsSelected = moduleNode.IsSelected,
                            IsIndeterminate = moduleNode.IsIndeterminate,
                            Permissions = new ObservableCollection<ModuleNodeViewModel>()
                        };

                        // Add all permissions if module matches, otherwise just add matching ones
                        if (moduleMatches)
                        {
                            foreach (var permission in moduleNode.Permissions)
                            {
                                filteredModule.Permissions.Add(permission);
                            }
                        }
                        else
                        {
                            foreach (var permission in matchingPermissions)
                            {
                                filteredModule.Permissions.Add(permission);
                            }
                        }

                        ModuleNodes.Add(filteredModule);
                    }
                }
            }
            finally
            {
                IsLoadingPermissions = false;
            }
        }
    }
}