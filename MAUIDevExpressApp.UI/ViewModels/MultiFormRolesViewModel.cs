using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.Services.Multiform;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class MultiFormRolesViewModel : BaseViewModel
    {
        private readonly IRoleService _roleService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private MultiFormManager _formManager;

        public MultiFormRolesViewModel(IRoleService roleService, INavigationService navigationService, IDialogService dialogService)
        {
            Title = "Roles";
            _roleService = roleService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            FormManager = new MultiFormManager(dialogService);
        }

        [RelayCommand]
        private void CreateNewForm()
        {
            FormManager.CreateNewSession();
        }

        [RelayCommand]
        private void SelectForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                form.IsMinimized = false;
                FormManager.CurrentForm = form;
            }
        }

        // Add this to MultiFormRolesViewModel
        public void UpdateFormTitle(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null && !string.IsNullOrWhiteSpace(form.Role.Name))
            {
                // Update the title based on whether it's an edit or new form
                form.Title = form.IsEditing
                    ? $"Edit {form.Role.Name}"
                    : $"{form.Role.Name}";

                // Also mark as having unsaved changes
                form.HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private async Task CloseForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null && form.HasUnsavedChanges)
            {
                bool shouldClose = await _dialogService.ShowConfirmationAsync(
                    "Unsaved Changes",
                    "You have unsaved changes. Are you sure you want to close this form?",
                    "Yes", "No");

                if (!shouldClose)
                    return;
            }

            FormManager.CloseSession(formId);
        }

        [RelayCommand]
        private void MinimizeForm(string formId)
        {
            FormManager.MinimizeSession(formId, true);
        }

        [RelayCommand]
        private async Task SaveForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form == null) return;

            try
            {
                form.IsBusy = true;
                form.HasUnsavedChanges = false;

                if (form.IsEditing)
                {
                    await _roleService.UpdateRoleAsync(form.Role);
                    await _dialogService.ShowSuccessAsync("Success", "Role updated successfully");
                }
                else
                {
                    await _roleService.CreateRoleAsync(form.Role);
                      // Update with ID and any server-side changes
                    form.IsEditing = true;
                    //form.Title = $"Edit {form.Role.Name}";
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


        // Called when property changes occur in form fields
        public void NotifyFormChanged(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                form.HasUnsavedChanges = true;
            }
        }

        [RelayCommand]
        private async Task OpenExistingRole(RoleDTO role)
        {
            var session = FormManager.CreateNewSession(role, $"Edit {role.Name}");
        }

    }
}
