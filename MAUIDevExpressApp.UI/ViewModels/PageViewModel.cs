using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class PageViewModel : BaseViewModel
    {
        private readonly IPageService _pageService;
        private readonly IModuleService _moduleService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<PageDTO> _pages;

        [ObservableProperty]
        private ObservableCollection<ModuleDTO> _modules;

        [ObservableProperty]
        private PageDTO _selectedPage;

        [ObservableProperty]
        private ModuleDTO _selectedModule;

        [ObservableProperty]
        private bool _isPopupOpen;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private string _popupTitle;

        private bool _isModulesLoaded = false;

        public PageViewModel(IPageService pageService, IModuleService moduleService, INavigationService navigationService, IDialogService dialogService)
        {
            Title = "Pages";
            _pageService = pageService;
            _moduleService = moduleService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            Pages = new ObservableCollection<PageDTO>();
            Modules = new ObservableCollection<ModuleDTO>();
            SelectedPage = new PageDTO { IsActive = false };
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadPages();
            await LoadModules();
        }

        private async Task LoadModules()
        {
            if (_isModulesLoaded) return;  // Prevent duplicate calls
            _isModulesLoaded = true;       // Mark as loaded
            try
            {
                var modules = await _moduleService.GetAllModulesAsync();
                Modules.Clear();
                foreach (var module in modules)
                {
                    Modules.Add(module);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", ex.Message);
            }
        }

        [RelayCommand]
        private async Task LoadPages()
        {
            try
            {
                IsBusy = true;
                var pages = await _pageService.GetAllPagesAsync();
                Pages.Clear();
                foreach (var page in pages)
                {
                    Pages.Add(page);
                }
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

        [RelayCommand]
        private async Task DeletePage(int id)
        {
            bool answer = await _dialogService.ShowConfirmationAsync("Delete Page",
                "Are you sure you want to delete this page?");
            if (answer)
            {
                try
                {
                    IsBusy = true;
                    await _pageService.DeletePageAsync(id);
                    await LoadPages();
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

        [RelayCommand]
        private void AddPage()
        {
            SelectedPage = new PageDTO { IsActive = false };
            PopupTitle = "Add New Page";
            IsEditMode = false;
            IsPopupOpen = true;
            SelectedModule = null;
        }

        [RelayCommand]
        private void EditPage(PageDTO page)
        {
            SelectedPage = new PageDTO
            {
                Id = page.Id,
                Name = page.Name,
                ModuleId = page.ModuleId,
                Description = page.Description,
                IsActive = page.IsActive,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt,
                Module = page.Module,
                ModuleObj = page.ModuleObj
            };

            SelectedModule = Modules.FirstOrDefault(m => m.Id == page.ModuleId);

            PopupTitle = "Edit Page";
            IsEditMode = true;
            IsPopupOpen = true;
        }

        [RelayCommand]
        private async Task SavePage()
        {
            if (string.IsNullOrWhiteSpace(SelectedPage.Name))
            {
                await _dialogService.ShowErrorAsync("Validation Error", "Page name is required");
                return;
            }

            if (SelectedModule != null)
            {
                SelectedPage.Module = SelectedModule.Name;
                SelectedPage.ModuleObj = SelectedModule;
                SelectedPage.ModuleId = SelectedModule.Id;
            }

            try
            {
                IsBusy = true;

                if (IsEditMode)
                {
                    SelectedPage.UpdatedAt = DateTime.Now;
                    await _pageService.UpdatePageAsync(SelectedPage);
                }
                else
                {
                    SelectedPage.CreatedAt = DateTime.Now;
                    await _pageService.CreatePageAsync(SelectedPage);
                }

                await LoadPages();
                IsPopupOpen = false;
                ResetForm();
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

        [RelayCommand]
        private void ClosePopup()
        {
            IsPopupOpen = false;
            ResetForm();
        }

        private void ResetForm()
        {
            SelectedPage = new PageDTO { IsActive = false };
            SelectedModule = null;
            IsEditMode = false;
        }
    }
}