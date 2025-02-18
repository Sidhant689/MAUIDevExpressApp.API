using MAUIDevExpressApp.UI.Services;
using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class ProductCategoriesPage : ContentPage
{
	private readonly ProductsCategoryViewModel productsCategoryViewModel;
	public ProductCategoriesPage(ProductsCategoryViewModel viewModel)
	{
		InitializeComponent();
		productsCategoryViewModel = viewModel;
		BindingContext = productsCategoryViewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		await productsCategoryViewModel.OnNavigatedToAsync();
    }
}