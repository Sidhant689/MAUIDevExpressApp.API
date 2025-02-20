using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAUIDevExpressApp.API.Controllers
{
    // RolesController.cs
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            return Ok(roles.Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.RolePermissions
                    .Select(rp => rp.Permission.Name)
                    .ToList()
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.RolePermissions
                    .Select(rp => rp.Permission.Name)
                    .ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create new role
                var role = new Role
                {
                    Name = request.Name,
                    Description = request.Description
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                // Add permissions
                if (request.Permissions != null)
                {
                    foreach (var permissionName in request.Permissions)
                    {
                        var permission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Name == permissionName);

                        if (permission != null)
                        {
                            var rolePermission = new RolePermission
                            {
                                RoleId = role.Id,
                                PermissionId = permission.Id
                            };
                            _context.RolePermissions.Add(rolePermission);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetRole), new { id = role.Id },
                    new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        Permissions = request.Permissions ?? new List<string>()
                    });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, UpdateRoleRequest request)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update basic role information
                role.Name = request.Name;
                role.Description = request.Description;

                // Remove existing permissions
                _context.RolePermissions.RemoveRange(role.RolePermissions);
                await _context.SaveChangesAsync();

                // Add new permissions
                if (request.Permissions != null)
                {
                    foreach (var permissionName in request.Permissions)
                    {
                        var permission = await _context.Permissions
                            .FirstOrDefaultAsync(p => p.Name == permissionName);

                        if (permission != null)
                        {
                            var rolePermission = new RolePermission
                            {
                                RoleId = role.Id,
                                PermissionId = permission.Id
                            };
                            _context.RolePermissions.Add(rolePermission);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remove user associations
                _context.UserRoles.RemoveRange(role.UserRoles);

                // Remove permission associations
                _context.RolePermissions.RemoveRange(role.RolePermissions);

                // Remove the role
                _context.Roles.Remove(role);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailablePermissions()
        {
            var permissions = await _context.Permissions
                .Select(p => p.Name)
                .ToListAsync();

            return Ok(permissions);
        }
    }
}
