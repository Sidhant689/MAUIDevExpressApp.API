using CommunityToolkit.Mvvm.ComponentModel;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    [QueryProperty(nameof(EntityToEdit), "Permission")]
    public partial class PermissionFormViewModel : MultiFormBaseViewModel<PermissionDTO>
    {
        private readonly IPermissionService _permissionService;
        private readonly IModuleService _moduleService;

        [ObservableProperty]
        private ObservableCollection<ModuleDTO> _modules;

        [ObservableProperty]
        private ModuleDTO _selectedModule;

        public PermissionFormViewModel(IPermissionService permissionService, IModuleService moduleService, INavigationService navigationService, IDialogService dialogService) : base(navigationService,dialogService)
        {
            _permissionService = permissionService;
            _moduleService = moduleService;
            Modules = new ObservableCollection<ModuleDTO>();
        }

        protected override string GetEditTitle(PermissionDTO permission)
        {
            return !string.IsNullOrWhiteSpace(permission.Name) ? $"Edit {permission.Name}" : "Edit Permission";
        }

        public override void UpdateFormTitle(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f=> f.Id == formId);
            if(form != null && !string.IsNullOrEmpty(form.Entity.Name))
            {
                // update the title based on weather it's an edit or new form

                form.Title = form.IsEditing
                    ? $"Edit {form.Entity.Name}" : $"{form.Entity.Name}";
            }
        }

        public void LoadModules()
        {
            try
            {
                var modulesTask = _moduleService.GetAllModulesAsync();
                var modules = modulesTask.Result; // Wait for the task to complete and get the result
                Modules.Clear();
                foreach (var category in modules)
                {
                    Modules.Add(category);
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", "Failed to load categories: " + ex.Message, "OK");
            }
        }
        protected override async Task SaveForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f=> f.Id == formId);

            if(form == null) return;

            try
            {
                form.IsBusy = true;
                form.HasUnsavedChanges = false;

                form.Entity.ModuleId = SelectedModule.Id;   

                if (form.IsEditing)
                {
                    await _permissionService.UpdatePermissionAsync(form.Entity);
                    await _dialogService.ShowSuccessAsync("Success", "Permission updated Successfully");
                }
                else
                {
                    await _permissionService.CreatePermissionAsync(form.Entity);
                    await _dialogService.ShowSuccessAsync("Success", "Permission Created Successfully");
                }

                // Close form after successful save
                FormManager.CloseSession(formId);

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
