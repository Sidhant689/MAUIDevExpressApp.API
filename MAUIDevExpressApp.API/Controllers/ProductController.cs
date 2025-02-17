using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{

    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ProductController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("api/GetAllProducts")]
        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                var data = await _context.Products.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("api/GetProductById")]
        public async Task<ActionResult<Product>> GetProductById(int Id)
        {
            var product = await _context.Products.FindAsync(Id);
            if (product == null) return NotFound("Product not found");
            return product;
        }

        [HttpPost]
        [Route("api/AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product data is invalid.");
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllProductsAsync), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("api/UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            try
            {

                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    return NotFound("Product not found.");
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.Image = product.Image;
                existingProduct.ProductCategory = product.ProductCategory;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ModifiedDate = DateTime.UtcNow;
                existingProduct.Description = product.Description;

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            try
            {
                var product = await _context.Products.FindAsync(Id);
                if (product == null)
                {
                    return NotFound("Product not found.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok("Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
