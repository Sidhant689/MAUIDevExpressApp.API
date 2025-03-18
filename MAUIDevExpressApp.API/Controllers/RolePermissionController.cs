using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    [Authorize]
    public class RolePermissionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolePermissionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/GetAllRolePermissions")]
        public async Task<IActionResult> GetAllRolePermissions()
        {
            try
            {
                var modules = await _context.Modules
                    .Include(m => m.Pages)
                        .ThenInclude(p => p.Permissions)
                    .ToListAsync();

                return Ok(modules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/GetRolePermissionsByRoleId")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            try
            {
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Include(rp => rp.Permission)
                        .ThenInclude(p => p.Page)
                            .ThenInclude(pg => pg.Module)
                    .ToListAsync();

                return Ok(rolePermissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/AssignRolePermissions")]
        public async Task<IActionResult> AssignRolePermissions([FromBody] List<int> permissionIds, [FromQuery] int roleId)
        {
            try
            {
                var role = await _context.Roles.FindAsync(roleId);
                if (role == null)
                {
                    return NotFound("Role not found.");
                }

                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();

                // Remove existing permissions that are not in the new list
                _context.RolePermissions.RemoveRange(existingPermissions.Where(rp => !permissionIds.Contains(rp.PermissionId)));

                // Add new permissions that are not already assigned
                foreach (var permissionId in permissionIds)
                {
                    if (!existingPermissions.Any(rp => rp.PermissionId == permissionId))
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permissionId,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Permissions assigned successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
