using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IAPIService _APIService;
        public ProductCategoryService(IAPIService aPIService)
        {
            _APIService = aPIService;
        }

        public async Task<List<ProductCategoryDTO>> GetAllProductCategoriesAsync()
        {
            return await _APIService.GetAsync<List<ProductCategoryDTO>>("GetAllProductCategories");
        }

        public async Task<ProductCategoryDTO> GetProductCategoryByIdAsync(int id)
        {
            return await _APIService.GetAsync<ProductCategoryDTO>($"GetProductCategoryById?Id={id}");
        }

        public async Task CreateProductCategoryAsync(ProductCategoryDTO productCategory)
        {
            await _APIService.PostAsync("AddProductCategory", productCategory);
        }

        public async Task UpdateProductCategoryAsync(ProductCategoryDTO productCategory)
        {
            await _APIService.PostAsync("UpdateProductCategory", productCategory);
        }

        public async Task DeleteProductCategoryAsync(int id)
        {
            await _APIService.DeleteAsync($"DeleteProductCategory?Id={id}");
        }
    }
}
