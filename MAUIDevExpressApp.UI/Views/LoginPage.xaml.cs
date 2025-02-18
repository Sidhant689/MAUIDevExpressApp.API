using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class LoginPage : ContentPage
{
	private readonly LoginViewModel _loginViewModel;
	public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		_loginViewModel = loginViewModel;
		BindingContext = _loginViewModel;
	}
}