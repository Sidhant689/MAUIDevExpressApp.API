using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using System.Text.Json;

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
            var roles =  await _apiService.GetAsync<List<RoleDTO>>("GetAllRoles");
            return roles;
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            return await _apiService.GetAsync<RoleDTO>($"GetRoleById?Id={id}");
        }

        public async Task<RoleDTO> CreateRoleAsync(RoleDTO role)
        {
            var response = await _apiService.PostAsync("AddRole", role);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RoleDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            throw new Exception($"Error creating role: {response.ReasonPhrase}");
        }

        public async Task<RoleDTO> UpdateRoleAsync(RoleDTO role)
        {
            var response = await _apiService.PostAsync("UpdateRole", role);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RoleDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            throw new Exception($"Error updating role: {response.ReasonPhrase}");
        }

        public async Task DeleteRoleAsync(int id)
        {
            await _apiService.DeleteAsync($"DeleteRole?Id={id}");
        }

        public async Task<List<RolePermissionDTO>> GetRolePermissionsAsync(int roleId)
        {
            var role = await _apiService.GetAsync<RoleDTO>($"GetRoleById?id={roleId}");
            if (role == null || role.RolePermissions == null)
                return new List<RolePermissionDTO>();

            return role.RolePermissions.Select(rp => new RolePermissionDTO
            {
                Id = rp.Id,
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId,
                Permission = rp.Permission != null ? new PermissionDTO
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    ModuleId = rp.Permission.ModuleId,
                    IsActive = rp.Permission.IsActive
                } : null
            }).ToList();
        }

        public async Task<bool> AddPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var request = new { RoleId = roleId, PermissionIds = permissionIds };
            var response = await _apiService.PostAsync($"AddPermissionsToRole", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            var request = new { RoleId = roleId, PermissionIds = permissionIds };
            var response = await _apiService.PostAsync($"RemovePermissionsFromRole", request);
            return response.IsSuccessStatusCode;
        }
    }
}
