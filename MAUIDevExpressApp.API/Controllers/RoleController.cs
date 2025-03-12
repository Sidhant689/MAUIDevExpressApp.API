using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<List<Role>>> GetAllRolesAsync()
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
        public async Task<ActionResult<Role>> AddRole([FromBody] Role role)
        {
            try
            {
                role.CreatedAt = DateTime.UtcNow;
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/UpdateRole")]
        public async Task<ActionResult<Role>> UpdateRole([FromBody] Role role)
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
                return existingRole;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("api/DeleteRole")]
        public async Task<ActionResult> DeleteRole(int id)
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

        // New methods for permission management
        [HttpGet]
        [Route("api/GetRolePermissions")]
        public async Task<ActionResult<List<RolePermission>>> GetRolePermissions(int roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            return rolePermissions;
        }

        [HttpPost]
        [Route("api/AddPermissionsToRole")]
        public async Task<ActionResult> AddPermissionsToRole([FromBody] PermissionRoleRequest request)
        {
            try
            {
                var role = await _context.Roles.FindAsync(request.RoleId);
                if (role == null) return NotFound("Role not found");

                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == request.RoleId)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                var newPermissions = request.PermissionIds
                    .Where(id => !existingPermissions.Contains(id))
                    .ToList();

                foreach (var permissionId in newPermissions)
                {
                    // Check if permission exists
                    var permissionExists = await _context.Permissions.AnyAsync(p => p.Id == permissionId);
                    if (!permissionExists) continue;

                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = request.RoleId,
                        PermissionId = permissionId,
                        AssignedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                return Ok("Permissions added to role successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/RemovePermissionsFromRole")]
        public async Task<ActionResult> RemovePermissionsFromRole([FromBody] PermissionRoleRequest request)
        {
            try
            {
                var role = await _context.Roles.FindAsync(request.RoleId);
                if (role == null) return NotFound("Role not found");

                var rolePermissionsToRemove = await _context.RolePermissions
                    .Where(rp => rp.RoleId == request.RoleId && request.PermissionIds.Contains(rp.PermissionId))
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(rolePermissionsToRemove);
                await _context.SaveChangesAsync();

                return Ok("Permissions removed from role successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}