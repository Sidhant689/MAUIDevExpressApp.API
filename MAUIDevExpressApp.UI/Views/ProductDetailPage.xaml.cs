using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views;

public partial class ProductDetailPage : ContentPage
{
    private readonly ProductDetailViewModel _viewModel;

    public ProductDetailPage(ProductDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await _viewModel.LoadCategories();

        if (BindingContext is ProductDetailViewModel viewModel)
        {
            viewModel.SetProduct(viewModel.Product);
        }
    }
}