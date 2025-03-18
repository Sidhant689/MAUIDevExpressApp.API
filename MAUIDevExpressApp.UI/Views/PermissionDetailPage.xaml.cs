using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class PermissionDetailPage : ContentPage
{
	private readonly PermissionFormViewModel _viewModel;
	public PermissionDetailPage(PermissionFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _viewModel.LoadPages();
    }

    private void PermissionName_textChanged(object sender, TextChangedEventArgs e)
    {
		if(BindingContext is PermissionFormViewModel viewModel && viewModel.FormManager.CurrentForm != null)
		{
			string formId = viewModel.FormManager.CurrentForm.Id;
			viewModel.NotifyFormChanged(formId);
			viewModel.UpdateFormTitle(formId);
		}
    }
}