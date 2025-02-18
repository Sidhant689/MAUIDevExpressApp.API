using MAUIDevExpressApp.UI.ViewModels;

namespace MAUIDevExpressApp.UI.Views
{
    public partial class ProductCategoryDetailPage : ContentPage
    {
        private readonly ProductCategoryDetailViewModel _viewModel;

        public ProductCategoryDetailPage(ProductCategoryDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // You can add any page initialization logic here if needed
        }
    }
}