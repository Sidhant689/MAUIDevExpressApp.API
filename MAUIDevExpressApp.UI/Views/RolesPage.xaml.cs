using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class RolesPage : ContentPage
{
	private readonly RolesViewModel _rolesViewModel;
	public RolesPage(RolesViewModel rolesViewModel)
	{
		InitializeComponent();
		_rolesViewModel = rolesViewModel;
		BindingContext = _rolesViewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		await _rolesViewModel.OnNavigatedToAsync();
    }
}