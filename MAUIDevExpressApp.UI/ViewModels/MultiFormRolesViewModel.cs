using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    [QueryProperty(nameof(EntityToEdit), "Role")]
    public partial class MultiFormRolesViewModel : MultiFormBaseViewModel<RoleDTO>
    {
        private readonly IRoleService _roleService;

        public MultiFormRolesViewModel(
            IRoleService roleService,
            INavigationService navigationService,
            IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            Title = "Roles";
            _roleService = roleService;
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

        //[RelayCommand]
        //private async Task OpenExistingRole(RoleDTO role)
        //{
        //    OpenExistingEntity(role);
        //}

        protected override async Task SaveForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form == null) return;

            try
            {
                form.IsBusy = true;
                form.HasUnsavedChanges = false;

                if (form.IsEditing)
                {
                    await _roleService.UpdateRoleAsync(form.Entity);
                    await _dialogService.ShowSuccessAsync("Success", "Role updated successfully");
                }
                else
                {
                    await _roleService.CreateRoleAsync(form.Entity);
                    // Update with ID and any server-side changes
                    form.IsEditing = true;
                    await _dialogService.ShowSuccessAsync("Success", "Role created successfully");
                }

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