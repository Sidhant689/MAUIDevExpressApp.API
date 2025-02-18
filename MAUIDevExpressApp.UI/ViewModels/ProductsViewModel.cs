using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        [RelayCommand]
        private async Task LoadProductsAsync()
        {
            if(IsBusy) return;

            try
            {
                IsBusy = true;
                var productList = await _productService.GetAllProductsAsync();
                Products.Clear();

                foreach (var product in productList)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefresheing = false;
            }
        }

        [RelayCommand]
        private async Task AddProductAsync()
        {
            await _navigationService.NavigateToAsync("ProductDetailPage");
        }

        [RelayCommand]
        private async Task ProductSelectedAsync(ProductDTO product)
        {
            if (product == null) return;
            await _navigationService.NavigateToAsync($"ProductDetailPage?id={product.Id}");
            SelectedProduct = null;
        }

        [RelayCommand]
        private async Task DeleteProductAsync(ProductDTO product)
        {
            if(product == null) return;


            bool answer = await Shell.Current.DisplayAlert(
                    "Delete Product",
                    $"Are you sure you want to delete {product.Name}?",
                    "Yes", "No");

            if (answer)
            {
                try
                {
                    IsBusy = true;
                    await _productService.DeleteProductAsync(product.Id);
                    Products.Remove(product);
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
