using MAUIDevExpressApp.UI.ViewModels;
using MAUIDevExpressApp.UI.ViewModels.Nodes;
using Microsoft.Maui;
using Syncfusion.Maui.Buttons;

namespace MAUIDevExpressApp.UI.Views;

public partial class MultiFormRolesPage : ContentPage
{
	private readonly MultiFormRolesViewModel _multiFormRolesViewModel;
	public MultiFormRolesPage(MultiFormRolesViewModel multiFormRolesViewModel)
	{
		InitializeComponent();
        _multiFormRolesViewModel = multiFormRolesViewModel;
        BindingContext = _multiFormRolesViewModel;
    }

    private void RoleName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is MultiFormRolesViewModel viewModel &&
            viewModel.FormManager.CurrentForm != null)
        {
            string formId = viewModel.FormManager.CurrentForm.Id;
            viewModel.NotifyFormChanged(formId);
            viewModel.UpdateFormTitle(formId);
        }
    }
}