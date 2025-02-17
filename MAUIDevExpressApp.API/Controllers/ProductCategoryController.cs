using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    public class ProductCategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ProductCategoryController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Get All Categories
        [HttpGet]
        [Route("api/GetAllProductCategories")]
        public async Task<ActionResult<List<ProductCategory>>> GetAllCategories()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        // Get Category by ID
        [HttpGet]
        [Route("api/GetProductCategoryById")]
        public async Task<ActionResult<ProductCategory>> GetProductCategoryById(int Id)
        {
            var category = await _context.ProductCategories.FindAsync(Id);
            if (category == null) return NotFound("Category not found");
            return category;
        }

        // Add Category
        [HttpPost]
        [Route("api/AddProductCategory")]
        public async Task<IActionResult> AddProductCategory([FromBody] ProductCategory category)
        {
            if (category == null) return BadRequest("Invalid Category Data");

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductCategoryById), new { id = category.Id }, category);
        }

        // Update Category
        [HttpPost]
        [Route("api/UpdateProductCategory")]
        public async Task<IActionResult> UpdateProductCategory([FromBody] ProductCategory category)
        {
            var existingCategory = await _context.ProductCategories.FindAsync(category.Id);
            if (existingCategory == null) return NotFound("Category not found");

            existingCategory.Name = category.Name;
            await _context.SaveChangesAsync();

            return Ok(existingCategory);
        }

        // Delete Category
        [HttpGet]
        [Route("api/DeleteProductCategory")]
        public async Task<IActionResult> DeleteProductCategory(int Id)
        {
            var category = await _context.ProductCategories.FindAsync(Id);
            if (category == null) return NotFound("Category not found");

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Category deleted successfully");
        }
    }
}
