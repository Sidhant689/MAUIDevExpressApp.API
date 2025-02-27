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
        Task CreatePermissionAsync(PermissionDTO permission);
        Task UpdatePermissionAsync(PermissionDTO permission);
        Task DeletePermissionAsync(int id);
    }
}
