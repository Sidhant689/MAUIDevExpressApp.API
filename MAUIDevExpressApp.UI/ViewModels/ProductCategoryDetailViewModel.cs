using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;

namespace MAUIDevExpressApp.UI.ViewModels
{
    [QueryProperty(nameof(Id), "Id")]
    public partial class ProductCategoryDetailViewModel : BaseViewModel
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private int? id;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string image;

        [ObservableProperty]
        private ProductCategoryDTO productCategory;

        private string _selectedImagePath;


        public ProductCategoryDetailViewModel(IProductCategoryService productCategoryService, INavigationService navigationService)
        {
            _productCategoryService = productCategoryService;
            _navigationService = navigationService;
            Title = "New Product Category";
            ProductCategory = new ProductCategoryDTO();

            Id = 2;
        }

        partial void OnIdChanged(int? value)
        {
            if (value.HasValue)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadProductCategoryAsync(value.Value);
                });
            }
        }

        private async Task LoadProductCategoryAsync(int id)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var category = await _productCategoryService.GetProductCategoryByIdAsync(id);
                if (category != null)
                {
                    Title = "Edit Product Category";
                    Name = category.Name;
                    Description = category.Description;
                    Image = category.Image;
                    ProductCategory = category;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveProductCategoryAsync()
        {
            if (string.IsNullOrEmpty(Name))
            {
                await Shell.Current.DisplayAlert("Error", "Name is required.", "Ok");
                return;
            }

            if (IsBusy) return;

            try
            {
                IsBusy = true;

                string newImagePath = Image;

                if (!string.IsNullOrEmpty(_selectedImagePath))
                {
                    string fileName = Path.GetFileName(_selectedImagePath);
                    string destinationPath = Path.Combine(FileSystem.AppDataDirectory, "Resources", "Images", fileName);

                    string folderPath = Path.Combine(FileSystem.AppDataDirectory, "Resources", "Images");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    using (var sourceStream = File.OpenRead(_selectedImagePath))
                    using (var destinationStream = File.Create(destinationPath))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }

                    newImagePath = destinationPath;
                }

                ProductCategory.Name = Name;
                ProductCategory.Description = Description;
                ProductCategory.Image = newImagePath;

                if (ProductCategory.Id == 0)
                {
                    await _productCategoryService.CreateProductCategoryAsync(ProductCategory);
                }
                else
                {
                    await _productCategoryService.UpdateProductCategoryAsync(ProductCategory);
                }

                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await _navigationService.GoBackAsync();
        }

        [RelayCommand]
        private async Task PickImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select an image"
                });

                if (result != null)
                {
                    _selectedImagePath = result.FullPath;
                    Image = _selectedImagePath;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to pick an image: " + ex.Message, "OK");
            }
        }
    }
}