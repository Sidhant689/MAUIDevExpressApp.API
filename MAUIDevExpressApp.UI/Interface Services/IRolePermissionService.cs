using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IRolePermissionService
    {
        /// <summary>
        /// Fetch all role permissions (Modules → Pages → Permissions)
        /// </summary>
        Task<List<ModuleDTO>> GetAllRolePermissionsAsync();

        /// <summary>
        /// Fetch role permissions assigned to a specific role
        /// </summary>
        Task<List<RolePermissionDTO>> GetRolePermissionsByRoleIdAsync(int roleId);

        /// <summary>
        /// Assign multiple permissions to a role
        /// </summary>
        Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);

        /// <summary>
        /// Remove a permission from a role
        /// </summary>
        Task<bool> RemovePermissionFromRoleAsync(int roleId, List<int> permissionId);
    }
}
