using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;

namespace MAUIDevExpressApp.UI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IAPIService _apiService;

        public RoleService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            return await _apiService.GetAsync<List<RoleDTO>>("GetAllRoles");
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            return await _apiService.GetAsync<RoleDTO>($"GetRoleById?Id={id}");
        }

        public async Task CreateRoleAsync(RoleDTO role)
        {
            await _apiService.PostAsync("AddRole", role);
        }

        public async Task UpdateRoleAsync(RoleDTO role)
        {
            await _apiService.PostAsync("UpdateRole", role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            await _apiService.DeleteAsync($"DeleteRole?Id={id}");
        }
    }
}
