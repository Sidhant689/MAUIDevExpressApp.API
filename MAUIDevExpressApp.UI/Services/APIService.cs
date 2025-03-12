using MAUIDevExpressApp.UI.Interface_Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;


namespace MAUIDevExpressApp.UI.Services
{
    public class APIService : IAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly IDialogService _dialogService;

        public APIService(HttpClient httpClient, IConfiguration configuration, IDialogService dialogService)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _dialogService = dialogService;
            _baseUrl = _configuration["API:BaseUrl"]?.ToString() ?? throw new ArgumentNullException("API:BaseUrl not configured");
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                await HandleHttpErrorIfNeeded(response);
                return response;
            }
            catch (HttpRequestException ex)
            {
                await HandleHttpException(ex);
                throw;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", $"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                await HandleHttpErrorIfNeeded(response);

                var result = await response.Content.ReadFromJsonAsync<TResponse>();
                return result;
            }
            catch (HttpRequestException ex)
            {
                await HandleHttpException(ex);
                throw;
            }
            catch (JsonException ex)
            {
                await _dialogService.ShowErrorAsync("Data Error", $"Failed to parse server response: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", $"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                await HandleHttpErrorIfNeeded(response);

                var result = await response.Content.ReadFromJsonAsync<T>();
                return result;
            }
            catch (HttpRequestException ex)
            {
                await HandleHttpException(ex);
                throw;
            }
            catch (JsonException ex)
            {
                await _dialogService.ShowErrorAsync("Data Error", $"Failed to parse server response: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", $"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                await HandleHttpErrorIfNeeded(response);
                return response;
            }
            catch (HttpRequestException ex)
            {
                await HandleHttpException(ex);
                throw;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", $"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        // Helper methods for error handling
        private async Task HandleHttpErrorIfNeeded(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage;

                try
                {
                    // Try to read error details from response body
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMessage = !string.IsNullOrWhiteSpace(errorContent)
                        ? errorContent
                        : $"Server returned {(int)response.StatusCode} ({response.StatusCode})";

                    // Try to parse as JSON error message if possible
                    try
                    {
                        var errorJson = JsonDocument.Parse(errorContent);
                        if (errorJson.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            errorMessage = messageElement.GetString();
                        }
                        else if (errorJson.RootElement.TryGetProperty("error", out var errorElement))
                        {
                            errorMessage = errorElement.GetString();
                        }
                    }
                    catch (JsonException)
                    {
                        // Not JSON or doesn't have expected properties, use the raw content
                    }
                }
                catch
                {
                    errorMessage = $"Server returned {(int)response.StatusCode} ({response.StatusCode})";
                }

                string title = GetErrorTitle(response.StatusCode);
                await _dialogService.ShowErrorAsync(title, errorMessage);

                throw new HttpRequestException(errorMessage, null, response.StatusCode);
            }
        }

        private async Task HandleHttpException(HttpRequestException ex)
        {
            string title = "Network Error";
            string message;

            if (ex.StatusCode.HasValue)
            {
                title = GetErrorTitle(ex.StatusCode.Value);
                message = $"Server returned {(int)ex.StatusCode.Value} ({ex.StatusCode.Value}): {ex.Message}";
            }
            else
            {
                message = $"Could not connect to the server: {ex.Message}";
            }

            await _dialogService.ShowErrorAsync(title, message);
        }

        private string GetErrorTitle(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => "Invalid Request",
                HttpStatusCode.Unauthorized => "Authentication Required",
                HttpStatusCode.Forbidden => "Access Denied",
                HttpStatusCode.NotFound => "Resource Not Found",
                HttpStatusCode.InternalServerError => "Server Error",
                HttpStatusCode.BadGateway => "Gateway Error",
                HttpStatusCode.ServiceUnavailable => "Service Unavailable",
                _ => $"HTTP Error {(int)statusCode}"
            };
        }
    }
}
