using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IAPIService _apiService;

        public RolePermissionService(IAPIService aPIService)
        {
            _apiService = aPIService;
        }

        /// <summary>
        /// Fetch all role permissions (without filtering by RoleId)
        /// </summary>
        public async Task<List<ModuleDTO>> GetAllRolePermissionsAsync()
        {
            return await _apiService.GetAsync<List<ModuleDTO>>("GetAllRolePermissions");
        }

        /// <summary>
        /// Fetch role permissions for a specific RoleId
        /// </summary>
        public async Task<List<RolePermissionDTO>> GetRolePermissionsByRoleIdAsync(int roleId)
        {
            return await _apiService.GetAsync<List<RolePermissionDTO>>($"GetRolePermissionsByRoleId?roleId={roleId}");
        }

        /// <summary>
        /// Assign permissions to a role
        /// </summary>
        public async Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var requestData = new
            {
                RoleId = roleId,
                PermissionIds = permissionIds
            };

            var response = await _apiService.PostAsync("AssignPermissionsToRole", requestData);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Remove a permission from a role
        /// </summary>
        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, List<int> permissionIds)
        {
            // use try catch block to handle exceptions
            try
            {
                var requestedData = new
                {
                    RoleId = roleId,
                    PermissionIds = permissionIds
                };
                var response = await _apiService.PostAsync("RemoveRolePermissions", requestedData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // log the exception
                return false;
            }
        }
    }
}
