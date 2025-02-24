using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class ModulesPage : ContentPage
{
	private readonly ModulesViewModel _moduleViewModel;
	public ModulesPage(ModulesViewModel modulesViewModel)
	{
		InitializeComponent();
		_moduleViewModel = modulesViewModel;
		BindingContext = _moduleViewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		_moduleViewModel.OnNavigatedToAsync();
    }
}