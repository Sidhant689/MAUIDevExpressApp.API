using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class ProductsPage : ContentPage
{
	private readonly ProductsViewModel _productsViewModel;
	public ProductsPage(ProductsViewModel productsViewModel)
	{
		InitializeComponent();
		_productsViewModel = productsViewModel;
		BindingContext = _productsViewModel;
	}
}