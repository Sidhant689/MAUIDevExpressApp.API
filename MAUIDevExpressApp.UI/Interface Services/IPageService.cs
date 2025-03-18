using MAUIDevExpressApp.Shared.DTOs;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IPageService
    {
        Task<List<PageDTO>> GetAllPagesAsync();
        Task<PageDTO> GetPageByIdAsync(int id);
        Task CreatePageAsync(PageDTO Page);
        Task UpdatePageAsync(PageDTO Page);
        Task DeletePageAsync(int id);
    }
}
