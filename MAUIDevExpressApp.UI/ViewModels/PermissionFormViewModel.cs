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
        private readonly IPageService _pageservice;

        [ObservableProperty]
        private ObservableCollection<PageDTO> _pages;

        [ObservableProperty]
        private PageDTO _selectedPage;

        public PermissionFormViewModel(IPermissionService permissionService, IPageService pageService, INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
            _permissionService = permissionService;
            _pageservice = pageService;
            Pages = new ObservableCollection<PageDTO>();
        }

        protected override string GetEditTitle(PermissionDTO permission)
        {
            return !string.IsNullOrWhiteSpace(permission.Name) ? $"Edit {permission.Name}" : "Edit Permission";
        }

        public override void UpdateFormTitle(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null && !string.IsNullOrEmpty(form.Entity.Name))
            {
                // update the title based on weather it's an edit or new form

                form.Title = form.IsEditing
                    ? $"Edit {form.Entity.Name}" : $"{form.Entity.Name}";
            }
        }

        private bool _isPagesLoaded = false;

        public async Task LoadPages()
        {
            if (_isPagesLoaded) return;  // Prevent duplicate calls
            _isPagesLoaded = true;       // Mark as loaded

            try
            {
                var pages = await _pageservice.GetAllPagesAsync();
                Pages.Clear();
                foreach (var category in pages)
                {
                    Pages.Add(category);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load categories: " + ex.Message, "OK");
            }
        }


        protected override async void OpenExistingEntity(PermissionDTO entity)
        {
            // Load Pages first to ensure they're available
            await LoadPages();

            // Find and set the selected Page based on the entity's PageId
            SelectedPage = Pages.FirstOrDefault(m => m.Id == entity.PageId);

            // Create a new session with the passed entity
            FormManager.CreateNewSession(entity, GetEditTitle(entity));
        }
        protected override async Task SaveForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);

            if (form == null) return;

            try
            {
                form.IsBusy = true;
                form.HasUnsavedChanges = false;

                form.Entity.PageId = SelectedPage.Id;

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
