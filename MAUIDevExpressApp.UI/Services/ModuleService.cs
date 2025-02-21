using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IAPIService _apiService;

        public ModuleService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<ModuleDTO>> GetAllModulesAsync()
        {
            return await _apiService.GetAsync<List<ModuleDTO>>("GetAllModules");
        }

        public async Task<ModuleDTO> GetModuleByIdAsync(int id)
        {
            return await _apiService.GetAsync<ModuleDTO>($"GetModuleById?Id={id}");
        }

        public async Task CreateModuleAsync(ModuleDTO module)
        {
            await _apiService.PostAsync("AddModule", module);
        }

        public async Task UpdateModuleAsync(ModuleDTO module)
        {
            await _apiService.PostAsync("UpdateModule", module);
        }

        public async Task DeleteModuleAsync(int id)
        {
            await _apiService.DeleteAsync($"DeleteModule?Id={id}");
        }
    }
}
