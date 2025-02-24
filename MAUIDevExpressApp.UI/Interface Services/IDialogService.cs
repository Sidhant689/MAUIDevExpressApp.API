using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IDialogService
    {
        Task<bool> ShowConfirmationAsync(string title, string message, string acceptText = "Yes", string cancelText = "No");
        Task ShowAlertAsync(string title, string message, string acceptText = "OK");
        Task ShowErrorAsync(string message, string title = "Error");
        Task ShowSuccessAsync(string message, string title = "Success");
        Task<string> ShowInputDialogAsync(string title, string message, string defaultValue = "", string acceptText = "OK", string cancelText = "Cancel");
        Task<T> ShowSelectionDialogAsync<T>(string title, List<T> items, Func<T, string> displayProperty, string cancelText = "Cancel");
        Task<(bool Success, string Input)> ShowPromptAsync(string title, string message, string placeholder = "", string acceptText = "OK", string cancelText = "Cancel");
        //Task ShowLoadingAsync(string message = "Loading...");
        //Task HideLoadingAsync();
    }
}
