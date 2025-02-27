using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using MAUIDevExpressApp.UI.Views;
using System.Collections.ObjectModel;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class ProductsViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigation;

        [ObservableProperty]
        private ObservableCollection<ProductDTO> _products;

        public ProductsViewModel(IProductService productService, INavigationService navigation)
        {
            Title = "Products";
            _productService = productService;
            _navigation = navigation;
            Products = new ObservableCollection<ProductDTO>();
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadProducts();
        }

        [RelayCommand]
        private async Task LoadProducts()
        {
            try
            {
                IsBusy = true;
                var products = await _productService.GetAllProductsAsync();
                Products.Clear();
                foreach (var product in products)
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
            }
        }

        [RelayCommand]
        private async Task DeleteProduct(int id)
        {
            bool answer = await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this product?", "Yes", "No");
            if (!answer) return;

            try
            {
                IsBusy = true;
                await _productService.DeleteProductAsync(id);
                var productToRemove = Products.FirstOrDefault(x => x.Id == id);
                if (productToRemove != null)
                {
                    Products.Remove(productToRemove);
                }
                await Shell.Current.DisplayAlert("Success", "Product deleted successfully", "OK");
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

        [RelayCommand]
        private async Task NavigateToAdd()
        {
            await _navigation.NavigateToAsync(nameof(ProductDetailPage));
        }

        [RelayCommand]
        private async Task NavigateToEdit(ProductDTO product)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Product", product }
            };
            await Shell.Current.GoToAsync(nameof(ProductDetailPage), parameters);
        }
    }
}