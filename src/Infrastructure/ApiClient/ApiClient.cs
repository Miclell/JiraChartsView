using System.Net.Http.Json;
using System.Text.Json;
using Core.Converters.JsonConverters;
using Core.Interfaces.ApiClient;
using Core.Models.ApiClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClient;

public class ApiClient(
    HttpClient httpClient, 
    IMemoryCache cache, 
    ILogger<ApiClient> logger) : IApiClient
{
    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string additionalUri)
    {
        try
        {
            if (cache.TryGetValue<ApiResponse<TResponse>>(additionalUri, out var cachedData))
                if (cachedData != null)
                {
                    logger.LogInformation("Use cached for {URL}", additionalUri);
                    return cachedData;
                }

            var response = await httpClient.GetAsync(additionalUri);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = $"HTTP ошибка: {response.StatusCode}",
                    ErrorCode = (int)response.StatusCode
                };

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JiraDateTimeConverter(), new NullableJiraDateTimeConverter() }
            });

            if (result == null)
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = "Ошибка десериализации ответа"
                };

            var apiResponse = new ApiResponse<TResponse>
            {
                Success = true,
                Result = result,
                ErrorCode = 0
            };
            cache.Set(additionalUri, apiResponse, TimeSpan.FromMinutes(30));
            logger.LogInformation("Set {URL} as cache", additionalUri);

            return apiResponse;

        }
        catch (Exception ex)
        {
            return new ApiResponse<TResponse>
            {
                Success = false,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };
        }
    }

    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string additionalUri, TRequest data)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(additionalUri, data);

            if (!response.IsSuccessStatusCode)
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = $"HTTP ошибка: {response.StatusCode}",
                    ErrorCode = (int)response.StatusCode
                };

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null)
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = "Ошибка десериализации ответа"
                };

            return apiResponse;
        }
        catch (Exception ex)
        {
            return new ApiResponse<TResponse>
            {
                Success = false,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };
        }
    }
}