using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    [Authorize]
    public class PageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/GetAllPages")]
        public async Task<List<Page>> GetAllPagesAsync()
        {
            try
            {
                var data = await _context.Pages.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        [Route("api/GetPageById")]
        public async Task<ActionResult<Page>> GetPageById(int id)
        {
            var Page = await _context.Pages.FirstOrDefaultAsync(m => m.Id == id);
            if (Page == null) return NotFound("Page not found");
            return Page;
        }

        [HttpGet]
        [Route("api/GetPagesByModuleId")]
        public async Task<List<Page>> GetPagesByModuleId(int moduleId)
        {
            // use try catch block to handle exceptions
            try
            {
                var Pages = await _context.Pages.Where(p => p.ModuleId == moduleId).ToListAsync();
                return Pages;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/AddPage")]
        public async Task<IActionResult> AddPage([FromBody] Page Page)
        {
            try
            {
                Page.CreatedAt = DateTime.UtcNow;
                _context.Pages.Add(Page);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPageById), new { id = Page.Id }, Page);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/UpdatePage")]
        public async Task<IActionResult> UpdatePage([FromBody] Page Page)
        {
            try
            {
                var existingPage = await _context.Pages.FindAsync(Page.Id);
                if (existingPage == null) return NotFound("Page not found");

                existingPage.Name = Page.Name;
                existingPage.Description = Page.Description;
                existingPage.IsActive = Page.IsActive;
                existingPage.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(existingPage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("api/DeletePage")]
        public async Task<IActionResult> DeletePage(int id)
        {
            try
            {
                var Page = await _context.Pages.FindAsync(id);
                if (Page == null) return NotFound("Page not found");

                _context.Pages.Remove(Page);
                await _context.SaveChangesAsync();
                return Ok("Page deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
