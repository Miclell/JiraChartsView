using Core.Interfaces.JiraClient;
using Infrastructure.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();
        
        // ApiClient
        services.AddJiraApiClient();
        
        // JiraClient
        services.AddScoped<IJiraClient, JiraClient.JiraClient>();
        
        return services;
    }
}