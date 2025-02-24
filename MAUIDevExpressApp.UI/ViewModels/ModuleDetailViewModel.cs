using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    [QueryProperty(nameof(Module), "Module")]
    public partial class ModuleDetailViewModel : BaseViewModel
    {
        private readonly IModuleService _moduleService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ModuleDTO _module;

        [ObservableProperty]
        private bool _isEditing;

        public ModuleDetailViewModel(IModuleService moduleService, INavigationService navigationService, IDialogService dialogService)
        {
            _moduleService = moduleService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            Module = new ModuleDTO();
        }

        public void SetModule(ModuleDTO module)
        {
            if (module?.Id != 0 && module != null)
            {
                Module = new ModuleDTO
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description,
                    IsActive = module.IsActive,
                };
                IsEditing = true;
                Title = "Edit Module";
            }
            else
            {
                Module = new ModuleDTO();
                IsEditing = false;
                Title = "Add Module";
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                IsBusy = true;

                if (IsEditing)
                {
                    await _moduleService.UpdateModuleAsync(Module);
                }
                else
                {
                    await _moduleService.CreateModuleAsync(Module);
                }

                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await _navigationService.GoBackAsync();
        }
    }
}
