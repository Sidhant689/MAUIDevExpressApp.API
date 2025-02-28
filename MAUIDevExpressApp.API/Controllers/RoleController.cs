using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/GetAllRoles")]
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }

        [HttpGet]
        [Route("api/GetRoleById")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return NotFound("Role not found");
            return role;
        }

        [HttpPost]
        [Route("api/AddRole")]
        public async Task<IActionResult> AddRole([FromBody] Role role)
        {
            try
            {
                role.CreatedAt = DateTime.UtcNow;
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] Role role)
        {
            try
            {
                var existingRole = await _context.Roles.FindAsync(role.Id);
                if (existingRole == null) return NotFound("Role not found");

                existingRole.Name = role.Name;
                existingRole.Description = role.Description;
                existingRole.IsActive = role.IsActive;
                existingRole.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(existingRole);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("api/DeleteRole")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null) return NotFound("Role not found");

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return Ok("Role deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
