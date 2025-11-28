using Core.Interfaces.ApiClient;
using Infrastructure.ApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Http;

public static class HttpClientFactorySetup
{
    public static void AddJiraApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<IApiClient, ApiClient.ApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://issues.apache.org/jira/rest/api/2/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }
}