using CommunityToolkit.Mvvm.ComponentModel;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System.Collections.ObjectModel;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class ModulesViewModel : BaseViewModel
    {
        private readonly IModuleService _moduleService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<ModuleDTO> _modules;

        public ModulesViewModel(IModuleService moduleService, INavigationService navigationService)
        {
            Title = "Modules";
            _moduleService = moduleService;
            _navigationService = navigationService;
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadModules();
        }

        private async Task LoadModules()
        {
            try
            {
                IsBusy = true;
                //Modules = await _moduleService.GetAllModulesAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
