using MAUIDevExpressApp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDTO>> GetAllProductCategoriesAsync();
        Task<ProductCategoryDTO> GetProductCategoryByIdAsync(int id);
        Task CreateProductCategoryAsync(ProductCategoryDTO productCategory);
        Task UpdateProductCategoryAsync(ProductCategoryDTO productCategory);
        Task DeleteProductCategoryAsync(int id);
    }
}
