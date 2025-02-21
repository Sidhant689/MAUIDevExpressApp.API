using MAUIDevExpressApp.Shared.Models;

namespace MAUIDevExpressApp.API.InterfaceServices
{
    public interface IPermissionManagementService
    {
        Task<bool> HasPermissionAsync(int userId, string moduleName, string permissionAction);
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task<bool> CreateModuleAsync(string name, string description);
        Task<bool> CreatePermissionAsync(string moduleName, string name, string action);
        Task<bool> AssignPermissionToRoleAsync(string roleName, string moduleName, string permissionAction);
    }
}
