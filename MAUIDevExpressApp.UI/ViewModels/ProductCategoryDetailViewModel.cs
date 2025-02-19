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
        private string _image;

        [ObservableProperty]
        private bool _isEditing;

        public ProductCategoryDetailViewModel(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
            Category = new ProductCategoryDTO();
        }

        public void SetCategory(ProductCategoryDTO category)
        {
            if (category.Id != 0)
            {
                Category = new ProductCategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Image = category.Image
                };
                Image = category.Image;
                IsEditing = true;
                Title = "Edit Category";
            }
            else
            {
                Category = new ProductCategoryDTO();
                Image = null;
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

                // ✅ Ensure Image is valid and points to a file
                if (string.IsNullOrWhiteSpace(Image) || !File.Exists(Image))
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid image file path.", "OK");
                    return;
                }

                string filePath = Image;  // Source file path
                string fileName = Path.GetFileName(filePath);  // Extract the filename

                // ✅ Define local storage directory
                string localDirectory = Path.Combine("D:\\Sidhant\\MAUI\\Images", "CategoryImages");

                // ✅ Ensure directory exists
                if (!Directory.Exists(localDirectory))
                {
                    Directory.CreateDirectory(localDirectory);
                }

                // ✅ Define the local file path
                string localFilePath = Path.Combine(localDirectory, fileName);

                // ✅ Debugging logs (check if file paths are correct)
                Console.WriteLine($"Source File: {filePath}");
                Console.WriteLine($"Destination File: {localFilePath}");

                // ❗ Check if the target path is mistakenly a directory
                if (Directory.Exists(localFilePath))
                {
                    Console.WriteLine($"Error: {localFilePath} is a directory, not a file.");
                    await Shell.Current.DisplayAlert("Error", "Target file path is a directory!", "OK");
                    return;
                }

                // ✅ If the file already exists, delete it first
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }

                // ✅ Copy the image file
                File.Copy(filePath, localFilePath);

                // ✅ Store the new local path in the category
                Category.Image = localFilePath;

                // ✅ Call the appropriate API method
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
                    // Get the file path
                    string filePath = result.FullPath;

                    // Update the Category.Image property
                    Image = filePath;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to pick image: " + ex.Message, "OK");
            }
        }
    }
}