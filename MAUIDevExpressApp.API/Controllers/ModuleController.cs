using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    [Authorize]
    public class ModuleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModuleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/GetAllModules")]
        public async Task<List<Module>> GetAllModulesAsync()
        {
            try
            {
                var data = await _context.Modules.Include(m => m.Permissions).ToListAsync();    
                return data;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            
        }

        [HttpGet]
        [Route("api/GetModuleById")]
        public async Task<ActionResult<Module>> GetModuleById(int id)
        {
            var module = await _context.Modules.Include(m => m.Permissions).FirstOrDefaultAsync(m => m.Id == id);
            if (module == null) return NotFound("Module not found");
            return module;
        }

        [HttpPost]
        [Route("api/AddModule")]
        public async Task<IActionResult> AddModule([FromBody] Module module)
        {
            try
            {
                module.CreatedAt = DateTime.UtcNow;
                _context.Modules.Add(module);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetModuleById), new { id = module.Id }, module);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("api/UpdateModule")]
        public async Task<IActionResult> UpdateModule([FromBody] Module module)
        {
            try
            {
                var existingModule = await _context.Modules.FindAsync(module.Id);
                if (existingModule == null) return NotFound("Module not found");

                existingModule.Name = module.Name;
                existingModule.Description = module.Description;
                existingModule.IsActive = module.IsActive;
                existingModule.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(existingModule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("api/DeleteModule")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            try
            {
                var module = await _context.Modules.FindAsync(id);
                if (module == null) return NotFound("Module not found");

                _context.Modules.Remove(module);
                await _context.SaveChangesAsync();
                return Ok("Module deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
