using MAUIDevExpressApp.Shared.DTOs;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IModuleService
    {
        Task<List<ModuleDTO>> GetAllModulesAsync();
        Task<ModuleDTO> GetModuleByIdAsync(int id);
        Task CreateModuleAsync(ModuleDTO module);
        Task UpdateModuleAsync(ModuleDTO module);
        Task DeleteModuleAsync(int id);
    }
}
