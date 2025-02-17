using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.UI.Interface_Services
{
    public interface IAPIService
    {
        Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data);
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
        Task<T> GetAsync<T>(string endpoint);
        Task<HttpResponseMessage> DeleteAsync(string endpoint);
        void SetAuthToken(string token);

    }
}
