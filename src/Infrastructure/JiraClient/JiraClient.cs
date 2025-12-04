using Core.Exceptions;
using Core.Interfaces.ApiClient;
using Core.Interfaces.JiraClient;
using Core.Models.ApiClient;
using Core.Models.JiraClient;
using Infrastructure.ApiClient;

namespace Infrastructure.JiraClient;

public class JiraClient(IApiClient apiClient) : IJiraClient
{
    public async Task<JiraSearchResponse> GetOpenTimeHistogramDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetOpenTimeHistogramData(projectKey);
        return await GetAllIssuesAsync(endpoint);
    }

    public async Task<JiraSearchResponse> GetStatusTimeDistributionDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetStatusTimeDistributionData(projectKey);
        return await GetAllIssuesAsync(endpoint);
    }

    public async Task<JiraChangelogResponse> GetIssueChangelogAsync(string issueKey)
    {
        var endpoint = JiraApiEndpoints.GetIssueChangelog(issueKey);
        var response = await apiClient.GetAsync<JiraChangelogResponse>(endpoint);
        return HandleApiResponse(response);
    }

    public async Task<JiraSearchResponse> GetDailyTaskFlowDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetDailyTaskFlowData(projectKey);
        return await GetAllIssuesAsync(endpoint);
    }

    public async Task<JiraSearchResponse> GetTopUsersDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetTopUsersData(projectKey);
        return await GetAllIssuesAsync(endpoint);
    }

    public async Task<JiraSearchResponse> GetWorklogDistributionDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetWorklogDistributionData(projectKey);
        return await GetAllIssuesAsync(endpoint);
    }

    public async Task<JiraWorklogApiResponse> GetIssueWorklogAsync(string issueKey)
    {
        var endpoint = JiraApiEndpoints.GetIssueWorklog(issueKey);
        var response = await apiClient.GetAsync<JiraWorklogApiResponse>(endpoint);
        return HandleApiResponse(response);
    }

    public async Task<JiraSearchResponse> GetPriorityDistributionDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetPriorityDistributionData(projectKey);
        return await GetAllIssuesAsync(endpoint);
    }

    private static T HandleApiResponse<T>(ApiResponse<T> response)
    {
        if (!response.Success)
        {
            throw new JiraApiException($"JIRA API call failed: {response.Message}", response.ErrorCode);
        }

        return response.Result ?? throw new JiraApiException("JIRA API returned null result");
    }

    private async Task<JiraSearchResponse> GetAllIssuesAsync(string baseEndpoint, int pageSize = 1000)
    {
        var allIssues = new List<JiraIssue>();
        var startAt = 0;
        int? total = null;

        while (true)
        {
            var endpointWithPaging = AddPagingParameters(baseEndpoint, startAt, pageSize);
            var pageResponse = await apiClient.GetAsync<JiraSearchResponse>(endpointWithPaging);
            var page = HandleApiResponse(pageResponse);

            total ??= page.Total != 0 ? page.Total : page.Issues.Count;

            if (page.Issues.Count == 0)
                break;

            allIssues.AddRange(page.Issues);

            if (allIssues.Count >= total)
                break;

            startAt += page.Issues.Count;
        }

        return new JiraSearchResponse
        {
            StartAt = 0,
            MaxResults = allIssues.Count,
            Total = (int)total,
            Issues = allIssues
        };
    }

    private static string AddPagingParameters(string endpoint, int startAt, int maxResults)
    {
        var separator = endpoint.Contains('?') ? '&' : '?';
        return $"{endpoint}{separator}startAt={startAt}&maxResults={maxResults}";
    }
}