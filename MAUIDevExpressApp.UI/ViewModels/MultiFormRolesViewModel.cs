using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
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
        private readonly IModuleService _moduleService;

        [ObservableProperty]
        private string _permissionSearchText;

        [ObservableProperty]
        private bool _isLoadingPermissions;

        // To track the original permissions for comparison when saving
        private List<int> _originalPermissionIds;

        public MultiFormRolesViewModel(IRoleService roleService, IModuleService moduleService, INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
            Title = "Roles";
            _roleService = roleService;
            _moduleService = moduleService;
        }

        // Override the GetEditTitle method to provide role-specific titles
        protected override string GetEditTitle(RoleDTO role)
        {
            return !string.IsNullOrWhiteSpace(role.Name) ? $"Edit {role.Name}" : "Edit Role";
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
    }
}