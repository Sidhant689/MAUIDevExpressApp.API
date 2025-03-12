using MAUIDevExpressApp.Shared.DTOs;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(int id);
        Task<RoleDTO> CreateRoleAsync(RoleDTO role);
        Task<RoleDTO> UpdateRoleAsync(RoleDTO role);
        Task DeleteRoleAsync(int id);
        Task<bool> AddPermissionsToRoleAsync(int roleId, List<int> permissionsToAdd);
        Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionsToRemove);
        Task<List<RolePermissionDTO>> GetRolePermissionsAsync(int id);
    }
}
