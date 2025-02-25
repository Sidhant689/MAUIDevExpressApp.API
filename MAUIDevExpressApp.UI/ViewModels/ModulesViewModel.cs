using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.Views;
using System.Collections.ObjectModel;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class ModulesViewModel : BaseViewModel
    {
        private readonly IModuleService _moduleService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<ModuleDTO> _modules;

        public ModulesViewModel(IModuleService moduleService, INavigationService navigationService, IDialogService dialogService)
        {
            Title = "Modules";
            _moduleService = moduleService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            Modules = new ObservableCollection<ModuleDTO>();
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadModules();
        }

        [RelayCommand]
        private async Task LoadModules()
        {
            try
            {
                IsBusy = true;
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
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteModule(int id)
        {
            bool answer = await _dialogService.ShowConfirmationAsync("Delete Module",
                "Are you sure you want to delete this module?", "Yes", "No");

            if (answer)
            {
                try
                {
                    IsBusy = true;
                    await _moduleService.DeleteModuleAsync(id);
                    var moduleToRemove = Modules.FirstOrDefault(m => m.Id == id);
                    Modules.Remove(moduleToRemove);
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
        private async Task NavigateToAdd()
        {
            await _navigationService.NavigateToAsync(nameof(ModuleDetailPage));
        }

        [RelayCommand]
        private async Task NavigateToEdit(ModuleDTO module)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Module", module }
            };
            await _navigationService.NavigateToAsync(nameof(ModuleDetailPage), parameters);
        }

        [RelayCommand]
        private async Task ItemTappedAsync(ModuleDTO module)
        {
            if (module != null)
            {
                await NavigateToEdit(module);
            }
        }
    }
}