using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.API.InterfaceServices;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MAUIDevExpressApp.API.Services
{
    public class PermissionManagementService : IPermissionManagementService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PermissionManagementService> _logger;

        public PermissionManagementService(AppDbContext context, IMemoryCache cache, ILogger<PermissionManagementService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> HasPermissionAsync(int userId, string moduleName, string permissionAction)
        {
            var cacheKey = $"user_permission_{userId}_{moduleName}_{permissionAction}";

            if (_cache.TryGetValue(cacheKey, out bool hasPermission))
            {
                return hasPermission;
            }

            hasPermission = await _context.Users
                .Where(u => u.Id == userId && u.IsActive == true)
                .SelectMany(u => u.UserRoles)
                .Where(ur => ur.Role.IsActive == true && ur.ExpiresAt == null && ur.ExpiresAt > DateTime.UtcNow)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.Permission.IsActive && rp.Permission.Module.IsActive &&
                    rp.Permission.Module.Name == moduleName && rp.Permission.Action == permissionAction).AnyAsync();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, hasPermission, cacheOptions);

            return hasPermission;
        }

        public async Task<bool> CreateModuleAsync(string name, string description)
        {
            try
            {
                if (await _context.Modules.AnyAsync(m => m.Name == name))
                {
                    return false;
                }

                var module = new Module
                {
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Modules.Add(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating module {ModuleName}", name);
                return false;
            }
        }

        public async Task<bool> CreatePermissionAsync(string moduleName, string name, string action)
        {
            try
            {
                var module = await _context.Modules
                    .FirstOrDefaultAsync(m => m.Name == moduleName);

                if (module == null) return false;

                if (await _context.Permissions
                    .AnyAsync(p => p.ModuleId == module.Id && p.Action == action))
                {
                    return false;
                }

                var permission = new Permission
                {
                    Name = name,
                    ModuleId = module.Id,
                    Action = action,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission {PermissionName}", name);
                return false;
            }
        }

        public async Task<bool> AssignPermissionToRoleAsync(
            string roleName, string moduleName, string permissionAction)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == roleName && r.IsActive);

                var permission = await _context.Permissions
                    .Include(p => p.Module)
                    .FirstOrDefaultAsync(p =>
                        p.Module.Name == moduleName &&
                        p.Action == permissionAction &&
                        p.IsActive);

                if (role == null || permission == null) return false;

                if (await _context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id))
                {
                    return false;
                }

                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id,
                    AssignedAt = DateTime.UtcNow
                };

                _context.RolePermissions.Add(rolePermission);
                await _context.SaveChangesAsync();

                // Clear related cache entries
                var cachePattern = $"user_permission_*_{moduleName}_{permissionAction}";
                ClearCacheByPattern(cachePattern);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permission to role {RoleName}", roleName);
                return false;
            }
        }

        private void ClearCacheByPattern(string pattern)
        {
            if (_cache is IEnumerable<KeyValuePair<object, object>> cache)
            {
                var keys = cache
                    .Where(kvp => kvp.Key.ToString().Contains(pattern))
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in keys)
                {
                    _cache.Remove(key);
                }
            }
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            var permissions = await _context.Users
                .Where(u => u.Id == userId && u.IsActive == true)
                .Include(u => u.UserRoles) // Load UserRoles
                    .ThenInclude(ur => ur.Role) // Load Role
                        .ThenInclude(r => r.RolePermissions) // Load RolePermissions
                            .ThenInclude(rp => rp.Permission) // Load Permission
                                .ThenInclude(p => p.Module) // Load Module
                .SelectMany(u => u.UserRoles)
                .Where(ur => ur.Role.IsActive == true && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.Permission.IsActive && rp.Permission.Module.IsActive)
                .Select(rp => rp.Permission)
                .ToListAsync();

            return permissions;
        }


        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            var roles = await _context.Users
                .Where(u => u.Id == userId && u.IsActive == true)
                .Include(u => u.UserRoles) // Load UserRoles
                    .ThenInclude(ur => ur.Role) // Load Role
                        .ThenInclude(r => r.RolePermissions) // Load RolePermissions
                            .ThenInclude(rp => rp.Permission) // Load Permission
                                .ThenInclude(p => p.Module) // Load Module
                .SelectMany(u => u.UserRoles)
                .Where(ur => ur.Role.IsActive == true && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
                .Select(ur => ur.Role)
                .ToListAsync();

            return roles;
        }

    }
}
