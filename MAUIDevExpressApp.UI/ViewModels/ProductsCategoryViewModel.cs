using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
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

        public ProductsCategoryViewModel(IProductCategoryService productCategoryService, INavigationService navigationService)
        {
            Title = "Product Categories";
            _productCategoryService = productCategoryService;
            _navigationService = navigationService;
            ProductCategories = new ObservableCollection<ProductCategoryDTO>();
        }

        public override async Task OnNavigatedToAsync()
        {
            await LoadProductCategories();
        }

        [RelayCommand]
        private async Task LoadProductCategories()
        {
            try
            {
                IsBusy = true;
                var categories = await _productCategoryService.GetAllProductCategoriesAsync();
                ProductCategories.Clear();
                foreach (var category in categories)
                {
                    ProductCategories.Add(category);
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
        private async Task DeleteProductCategory(int id)
        {
            bool answer = await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this category?", "Yes", "No");
            if (!answer) return;

            try
            {
                IsBusy = true;
                await _productCategoryService.DeleteProductCategoryAsync(id);
                var categoryToRemove = ProductCategories.FirstOrDefault(x => x.Id == id);
                if (categoryToRemove != null)
                {
                    ProductCategories.Remove(categoryToRemove);
                }
                await Shell.Current.DisplayAlert("Success", "Product category deleted successfully", "OK");
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
            await Shell.Current.GoToAsync(nameof(ProductCategoryDetailPage));
        }

        [RelayCommand]
        private async Task NavigateToEdit(ProductCategoryDTO category)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Category", category }
            };
            //await Shell.Current.GoToAsync(nameof(ProductCategoryDetailPage), parameters);
            await _navigationService.NavigateToAsync(nameof(ProductCategoryDetailPage), parameters);
        }
    }

}

