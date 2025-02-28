using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class PermissionPage : ContentPage
{
	private readonly PermissionViewModel _permissionViewModel;
	public PermissionPage(PermissionViewModel permissionViewModel)
	{
		InitializeComponent();
		_permissionViewModel = permissionViewModel;
		BindingContext = _permissionViewModel;
	}

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _permissionViewModel.OnNavigatedToAsync();
    }
}