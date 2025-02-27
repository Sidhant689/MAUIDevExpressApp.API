using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermissionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/GetAllPermissions")]
        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions.Include(m => m.RolePermissions).ToListAsync();
        }

        [HttpGet]
        [Route("api/GetPermissionById")]
        public async Task<ActionResult<Permission>> GetPermissionById(int id)
        {
            var Permission = await _context.Permissions.Include(m => m.RolePermissions).FirstOrDefaultAsync(m => m.Id == id);
            if (Permission == null) return NotFound("Permission not found");
            return Permission;
        }

        [HttpPost]
        [Route("api/AddPermission")]
        public async Task<IActionResult> AddPermission([FromBody] Permission Permission)
        {
            try
            {
                Permission.CreatedAt = DateTime.UtcNow;
                _context.Permissions.Add(Permission);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPermissionById), new { id = Permission.Id }, Permission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("api/UpdatePermission")]
        public async Task<IActionResult> UpdatePermission([FromBody] Permission Permission)
        {
            try
            {
                var existingPermission = await _context.Permissions.FindAsync(Permission.Id);
                if (existingPermission == null) return NotFound("Permission not found");

                existingPermission.Name = Permission.Name;
                existingPermission.Description = Permission.Description;
                existingPermission.IsActive = Permission.IsActive;
                existingPermission.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(existingPermission);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("api/DeletePermission")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var Permission = await _context.Permissions.FindAsync(id);
                if (Permission == null) return NotFound("Permission not found");

                _context.Permissions.Remove(Permission);
                await _context.SaveChangesAsync();
                return Ok("Permission deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
