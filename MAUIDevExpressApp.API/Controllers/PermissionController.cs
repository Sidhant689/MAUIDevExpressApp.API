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

    }
}
