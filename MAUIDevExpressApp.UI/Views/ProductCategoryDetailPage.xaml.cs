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

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if(BindingContext is ProductCategoryDetailViewModel _viewModel)
            {
                _viewModel.SetCategory(_viewModel.Category);
            }
        }
    }
}