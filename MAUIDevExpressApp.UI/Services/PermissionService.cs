using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IAPIService _apiService;

        public PermissionService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<PermissionDTO>> GetAllPermissionsAsync()
        {
            var data =  await _apiService.GetAsync<List<PermissionDTO>>("GetAllPermissions");
            return data;
        }

        public async Task<List<PermissionDTO>> GetPermissionsByModuleAsync(int moduleId)
        {
            var permissions = await _apiService.GetAsync<List<PermissionDTO>>($"GetPermissionsByModule?moduleId={moduleId}");
            return permissions;
        }

        public async Task<PermissionDTO> GetPermissionByIdAsync(int id)
        {
            var data =  await _apiService.GetAsync<PermissionDTO>($"GetPermissionById?Id={id}");
            return data;
        }

        public async Task<PermissionDTO> CreatePermissionAsync(PermissionDTO Permission)
        {
            var response = await _apiService.PostAsync("AddPermission", Permission);
            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PermissionDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
                return result;
            }
            throw new Exception($"Error creating Role : {response.RequestMessage}");
        }

        public async Task<PermissionDTO> UpdatePermissionAsync(PermissionDTO Permission)
        {
            var response = await _apiService.PostAsync("UpdatePermission", Permission);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PermissionDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }
            throw new Exception($"Error creating Role : {response.RequestMessage}");
        }

        public async Task DeletePermissionAsync(int id)
        {
            await _apiService.DeleteAsync($"DeletePermission?Id={id}");
        }
    }
}
