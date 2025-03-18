using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.UI.Interface_Services;

namespace MAUIDevExpressApp.UI.Services
{
    public class PageService : IPageService
    {
        private readonly IAPIService _apiService;

        public PageService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<PageDTO>> GetAllPagesAsync()
        {
            var data = await _apiService.GetAsync<List<PageDTO>>("GetAllPages");
            return data;
        }

        public async Task<PageDTO> GetPageByIdAsync(int id)
        {
            return await _apiService.GetAsync<PageDTO>($"GetPageById?Id={id}");
        }

        public async Task CreatePageAsync(PageDTO Page)
        {
            await _apiService.PostAsync("AddPage", Page);
        }

        public async Task UpdatePageAsync(PageDTO Page)
        {
            await _apiService.PostAsync("UpdatePage", Page);
        }

        public async Task DeletePageAsync(int id)
        {
            await _apiService.DeleteAsync($"DeletePage?Id={id}");
        }
    }
}
