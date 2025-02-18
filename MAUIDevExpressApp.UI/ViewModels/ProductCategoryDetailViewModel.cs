using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;

namespace MAUIDevExpressApp.UI.ViewModels
{
    [QueryProperty(nameof(Category), "Category")]
    public partial class ProductCategoryDetailViewModel : BaseViewModel
    {
        private readonly IProductCategoryService _productCategoryService;

        [ObservableProperty]
        private ProductCategoryDTO _category;

        [ObservableProperty]
        private bool _isEditing;

        public ProductCategoryDetailViewModel(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
            Category = new ProductCategoryDTO();
        }

        public void SetCategory(ProductCategoryDTO category)
        {
            if (category != null)
            {
                Category = new ProductCategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Image = category.Image
                };
                IsEditing = true;
                Title = "Edit Category";
            }
            else
            {
                Category = new ProductCategoryDTO();
                IsEditing = false;
                Title = "Add Category";
            }
        }

        [RelayCommand]
        private async Task SaveCategory()
        {
            if (string.IsNullOrWhiteSpace(Category.Name))
            {
                await Shell.Current.DisplayAlert("Error", "Name is required", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                if (IsEditing)
                {
                    await _productCategoryService.UpdateProductCategoryAsync(Category);
                    await Shell.Current.DisplayAlert("Success", "Category updated successfully", "OK");
                }
                else
                {
                    await _productCategoryService.CreateProductCategoryAsync(Category);
                    await Shell.Current.DisplayAlert("Success", "Category added successfully", "OK");
                }

                await Shell.Current.GoToAsync("..");
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
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}