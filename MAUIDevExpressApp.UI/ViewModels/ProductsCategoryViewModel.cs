using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.ViewModels
{
    public partial class ProductsCategoryViewModel : BaseViewModel
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<ProductCategoryDTO> _productCategories;

        [ObservableProperty]
        private ProductCategoryDTO _selectedProductCategory;

        [ObservableProperty]
        private bool isRefresheing;

        public ProductsCategoryViewModel(INavigationService navigationService, IProductCategoryService productCategoryService)
        {
            _navigationService = navigationService;
            _productCategoryService = productCategoryService;
            Title = "Prduct Categories";
            ProductCategories = new ObservableCollection<ProductCategoryDTO>();
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadProductCategoryAsync();
        }

        [RelayCommand]
        private async Task LoadProductCategoryAsync()
        {
            if(IsBusy) return;

            try
            {
                IsBusy = true;
                var productCategoryList = await _productCategoryService.GetAllProductCategoriesAsync();
                ProductCategories.Clear();
                foreach (var productCategory in productCategoryList)
                {
                    ProductCategories.Add(productCategory);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
                IsRefresheing = false;
            }
        }

        [RelayCommand]
        private async Task AddProductCategoryAsync()
        {
            await _navigationService.NavigateToAsync("ProductCategoryDetailPage");
        }

        [RelayCommand]
        private async Task ProductCategorySelectedAsync(ProductCategoryDTO productCategory)
        {
            if (productCategory == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "Id", productCategory.Id }
            };

            await _navigationService.NavigateToAsync(nameof(ProductCategoryDetailPage), parameters);
            SelectedProductCategory = null;
        }


        [RelayCommand]
        private async Task DeleteProductCategoryAsync(ProductCategoryDTO productCategory)
        {
            if (productCategory == null) return;

            bool answer = await Shell.Current.DisplayAlert(
                "Delete Product Category",
                $"Are you sure you want to delete {productCategory.Name}?",
                "Yes", "No");

            if (answer)
            {
                try
                {
                    IsBusy = true;
                    await _productCategoryService.DeleteProductCategoryAsync(productCategory.Id);
                    ProductCategories.Remove(productCategory);
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
