using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using MAUIDevExpressApp.UI.ViewModels.GenericViewModels;
using System.Collections.ObjectModel;

namespace MAUIDevExpressApp.UI.ViewModels
{
    [QueryProperty(nameof(Product), "Product")]
    public partial class ProductDetailViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;

        [ObservableProperty]
        private ProductDTO _product;

        [ObservableProperty]
        private string _image;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private ObservableCollection<ProductCategoryDTO> _categories;

        [ObservableProperty]
        private ProductCategoryDTO _selectedCategory;

        public ProductDetailViewModel(IProductService productService,
                                   IProductCategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
            Product = new ProductDTO();
            Categories = new ObservableCollection<ProductCategoryDTO>();
        }

        public async Task LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllProductCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load categories: " + ex.Message, "OK");
            }
        }

        public void SetProduct(ProductDTO product)
        {
            if (product?.Id != 0 && product != null)
            {
                Product = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Image = product.Image,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CategoryId = product.CategoryId
                    
                };
                Image = product.Image;
                IsEditing = true;
                Title = "Edit Product";

                // Set selected category
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }
            else
            {
                Product = new ProductDTO();
                Image = null;
                IsEditing = false;
                Title = "Add Product";
            }
        }

        [RelayCommand]
        private async Task SaveProduct()
        {
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                await Shell.Current.DisplayAlert("Error", "Name is required", "OK");
                return;
            }

            if (Product.Price <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Price must be greater than zero", "OK");
                return;
            }

            if (Product.StockQuantity < 0)
            {
                await Shell.Current.DisplayAlert("Error", "Stock quantity cannot be negative", "OK");
                return;
            }

            if (SelectedCategory == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a category", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Handle image upload if there's a new image
                if (!string.IsNullOrWhiteSpace(Image) && File.Exists(Image))
                {
                    string fileName = Path.GetFileName(Image);
                    string localDirectory = Path.Combine("D:\\Sidhant\\MAUI\\Images", "ProductImages");

                    if (!Directory.Exists(localDirectory))
                    {
                        Directory.CreateDirectory(localDirectory);
                    }

                    string localFilePath = Path.Combine(localDirectory, fileName);

                    // If the file does not exist, copy it; otherwise, just set the path
                    if (!File.Exists(localFilePath))
                    {
                        File.Copy(Image, localFilePath);
                    }

                    Product.Image = localFilePath;
                }


                // Set the category
                Product.CategoryId = SelectedCategory.Id;
                Product.CategoryName = SelectedCategory.Name;

                if (IsEditing)
                {
                    await _productService.UpdateProductAsync(Product);
                    await Shell.Current.DisplayAlert("Success", "Product updated successfully", "OK");
                }
                else
                {
                    await _productService.CreateProductAsync(Product);
                    await Shell.Current.DisplayAlert("Success", "Product added successfully", "OK");
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

        [RelayCommand]
        private async Task PickImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Pick an image"
                });

                if (result != null)
                {
                    Image = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to pick image: " + ex.Message, "OK");
            }
        }
    }
}