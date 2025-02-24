using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services
{
    public class DialogService : IDialogService
    {
        private bool _currentLoadingDialog;
        public async Task<bool> ShowConfirmationAsync(string title, string message, string acceptText = "Yes", string cancelText = "No")
        {
            return await Shell.Current.DisplayAlert(title, message, acceptText, cancelText);
        }

        public async Task ShowAlertAsync(string title, string message, string acceptText = "OK")
        {
            await Shell.Current.DisplayAlert(title, message, acceptText);
        }

        public async Task ShowErrorAsync(string message, string title = "Error")
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }

        public async Task ShowSuccessAsync(string message, string title = "Success")
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }

        public async Task<string> ShowInputDialogAsync(string title, string message, string defaultValue = "", string acceptText = "OK", string cancelText = "Cancel")
        {
            return await Shell.Current.DisplayPromptAsync(title, message, acceptText, cancelText, placeholder: defaultValue) ?? string.Empty;
        }

        public async Task<T> ShowSelectionDialogAsync<T>(string title, List<T> items, Func<T, string> displayProperty, string cancelText = "Cancel")
        {
            var displayItems = items.Select(displayProperty).ToArray();
            var result = await Shell.Current.DisplayActionSheet(title, cancelText, null, displayItems);

            if (result == cancelText || string.IsNullOrEmpty(result))
                return default;

            return items.FirstOrDefault(item => displayProperty(item) == result);
        }

        public async Task<(bool Success, string Input)> ShowPromptAsync(string title, string message, string placeholder = "", string acceptText = "OK", string cancelText = "Cancel")
        {
            var result = await Shell.Current.DisplayPromptAsync(title, message, acceptText, cancelText, placeholder: placeholder);
            return (result != null, result ?? string.Empty);
        }

        //public async Task ShowLoadingAsync(string message = "Loading...")
        //{
        //    await HideLoadingAsync();
        //    _currentLoadingDialog = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long, 12);
        //    await _currentLoadingDialog.Show();
        //}

        //public async Task HideLoadingAsync()
        //{
        //    if (_currentLoadingDialog != null)
        //    {
        //        await _currentLoadingDialog.Dismiss();
        //        _currentLoadingDialog = null;
        //    }
        //}
    }
}
