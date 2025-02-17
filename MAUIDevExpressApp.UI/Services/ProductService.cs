using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services
{
    public class ProductService : IProductService
    {

        private readonly IAPIService _APIService;

        public ProductService(IAPIService apiService)
        {
            _APIService = apiService;
        }
        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            return await _APIService.GetAsync<List<ProductDTO>>("GetAllProducts");
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            return await _APIService.GetAsync<ProductDTO>($"GetProductById?Id={id}");
        }

        public async Task CreateProductAsync(ProductDTO product)
        {
            await _APIService.PostAsync("AddProduct", product);
        }

        public async Task UpdateProductAsync(ProductDTO product)
        {
            await _APIService.PostAsync($"UpdateProduct", product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _APIService.DeleteAsync($"DeleteProduct?Id={id}");
        }

    }
}
