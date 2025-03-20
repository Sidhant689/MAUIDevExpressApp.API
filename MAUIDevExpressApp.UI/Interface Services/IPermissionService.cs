using MAUIDevExpressApp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IPermissionService
    {
        Task<List<PermissionDTO>> GetAllPermissionsAsync();
        Task<PermissionDTO> GetPermissionByIdAsync(int id);
        Task<PermissionDTO> CreatePermissionAsync(PermissionDTO permission);
        Task<PermissionDTO> UpdatePermissionAsync(PermissionDTO permission);
        Task DeletePermissionAsync(int id);
        Task<List<PermissionDTO>> GetPermissionsByModuleAsync(int id);
        Task<List<PermissionDTO>> GetPermissionsByPageIdAsync(int id);
    }
}
