using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return await _apiService.GetAsync<List<PermissionDTO>>("GetAllPermissions");
        }

        public async Task<PermissionDTO> GetPermissionByIdAsync(int id)
        {
            return await _apiService.GetAsync<PermissionDTO>($"GetPermissionById?Id={id}");
        }

        public async Task CreatePermissionAsync(PermissionDTO Permission)
        {
            await _apiService.PostAsync("AddPermission", Permission);
        }

        public async Task UpdatePermissionAsync(PermissionDTO Permission)
        {
            await _apiService.PostAsync("UpdatePermission", Permission);
        }

        public async Task DeletePermissionAsync(int id)
        {
            await _apiService.DeleteAsync($"DeletePermission?Id={id}");
        }
    }
}
