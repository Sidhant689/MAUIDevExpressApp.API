using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermissionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
        {
            return await _context.Permissions
                .Include(p => p.RolePermissions)
                .ThenInclude(rp => rp.Role)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> GetPermission(int id)
        {
            var permission = await _context.Permissions
                .Include(p => p.RolePermissions)
                .ThenInclude(rp => rp.Role)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (permission == null) return NotFound();
            return permission;
        }

        [HttpPost]
        public async Task<ActionResult<Permission>> CreatePermission(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, Permission permission)
        {
            if (id != permission.Id) return BadRequest();
            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return NotFound();
            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
