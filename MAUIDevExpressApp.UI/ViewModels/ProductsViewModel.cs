using CommunityToolkit.Mvvm.ComponentModel;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class ProductsViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<ProductDTO> _products;

        [ObservableProperty]
        private ProductDTO _selectedProduct;

        [ObservableProperty]
        private bool isRefresheing;

        public ProductsViewModel(IProductService productService, INavigationService navigationService)
        {
            _productService = productService;
            _navigationService = navigationService;
            Title = "Products";
            Products = new ObservableCollection<ProductDTO>();
        }
    }
}
