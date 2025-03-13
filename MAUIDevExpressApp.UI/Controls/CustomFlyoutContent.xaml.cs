using MAUIDevExpressApp.UI.ViewModels.ControlsViewModel;

namespace MAUIDevExpressApp.UI.Controls;

public partial class CustomFlyoutContent : ContentView
{
    public CustomFlyoutContent()
	{
		InitializeComponent();
        BindingContext = new CustomFlyoutViewModel();
    }
}