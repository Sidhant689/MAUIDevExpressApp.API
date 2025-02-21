using MAUIDevExpressApp.Shared.DTOs;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(int id);
        Task CreateRoleAsync(RoleDTO role);
        Task UpdateRoleAsync(RoleDTO role);
        Task DeleteRoleAsync(int id);
    }
}
