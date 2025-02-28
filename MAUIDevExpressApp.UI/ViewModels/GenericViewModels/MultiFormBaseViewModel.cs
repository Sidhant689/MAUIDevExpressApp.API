using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.Services.Multiform;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels.GenericViewModels
{
    public abstract partial class MultiFormBaseViewModel<T> : BaseViewModel where T : class, new()
    {
        protected readonly IDialogService _dialogService;
        protected readonly INavigationService _navigationService;

        [ObservableProperty]
        private MultiFormManager<T> _formManager;

        [ObservableProperty]
        private T _entityToEdit;

        partial void OnEntityToEditChanged(T value)
        {
            if (value != null)
            {
                // Create a new session with the passed entity
                OpenExistingEntity(value);
            }
        }

        public MultiFormBaseViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            FormManager = new MultiFormManager<T>(dialogService);
        }

        // Add this method to handle opening an existing entity
        protected virtual void OpenExistingEntity(T entity)
        {
            // Default implementation, override in derived classes for custom behavior
            FormManager.CreateNewSession(entity, GetEditTitle(entity));
        }

        // Add this method to get a title for an entity, override in derived classes
        protected virtual string GetEditTitle(T entity)
        {
            return "Edit Item";
        }

        [RelayCommand]
        protected virtual void CreateNewForm()
        {
            FormManager.CreateNewSession();
        }

        [RelayCommand]
        protected virtual void SelectForm(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                form.IsMinimized = false;
                FormManager.CurrentForm = form;
            }
        }

        [RelayCommand]
        protected virtual async Task CloseForm(string formId)
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
        protected virtual void MinimizeForm(string formId)
        {
            FormManager.MinimizeSession(formId, true);
        }

        [RelayCommand]
        protected abstract Task SaveForm(string formId);

        // Called when property changes occur in form fields
        public virtual void NotifyFormChanged(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                form.HasUnsavedChanges = true;
            }
        }

        // Update form title based on entity properties
        public virtual void UpdateFormTitle(string formId)
        {
            var form = FormManager.ActiveForms.FirstOrDefault(f => f.Id == formId);
            if (form != null)
            {
                // Derived classes should override this to set appropriate titles
                form.HasUnsavedChanges = true;
            }
        }
    }
}