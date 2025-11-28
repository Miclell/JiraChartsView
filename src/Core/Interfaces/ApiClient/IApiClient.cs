using Core.Models;
using Core.Models.ApiClient;

namespace Core.Interfaces.ApiClient;

public interface IApiClient
{
    Task<ApiResponse<TResponse>> GetAsync<TResponse>(string additionalUri);
    Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string additionalUri, TRequest data);
}