using CommunityToolkit.Mvvm.ComponentModel;

namespace MAUIDevExpressApp.UI.ViewModels.GenericViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private string _title;

        public virtual Task OnNavigatedToAsync()
        {
            return Task.CompletedTask;
        }
    }
}