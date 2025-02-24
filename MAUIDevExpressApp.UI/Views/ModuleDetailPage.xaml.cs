using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class ModuleDetailPage : ContentPage
{
	private readonly ModuleDetailViewModel viewModel;
	public ModuleDetailPage(ModuleDetailViewModel moduleDetailViewModel)
	{
		InitializeComponent();
		viewModel = moduleDetailViewModel;
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		viewModel.SetModule(viewModel.Module);
    }
}