using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class RoleDetailPage : ContentPage
{
	private readonly RoleDetailViewModel _viewModel;
	public RoleDetailPage(RoleDetailViewModel roleDetailViewModel)
	{
		InitializeComponent();
		_viewModel = roleDetailViewModel;
		BindingContext = _viewModel;
	}
}