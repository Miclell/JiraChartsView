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
        var response = await apiClient.GetAsync<JiraSearchResponse>(endpoint);
        return HandleApiResponse(response);
    }

    public async Task<JiraSearchResponse> GetStatusTimeDistributionDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetStatusTimeDistributionData(projectKey);
        var response = await apiClient.GetAsync<JiraSearchResponse>(endpoint);
        return HandleApiResponse(response);
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
        var response = await apiClient.GetAsync<JiraSearchResponse>(endpoint);
        return HandleApiResponse(response);
    }

    public async Task<JiraSearchResponse> GetTopUsersDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetTopUsersData(projectKey);
        var response = await apiClient.GetAsync<JiraSearchResponse>(endpoint);
        return HandleApiResponse(response);
    }

    public async Task<JiraSearchResponse> GetWorklogDistributionDataAsync(string projectKey)
    {
        var endpoint = JiraApiEndpoints.GetWorklogDistributionData(projectKey);
        var response = await apiClient.GetAsync<JiraSearchResponse>(endpoint);
        return HandleApiResponse(response);
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
        var response = await apiClient.GetAsync<JiraSearchResponse>(endpoint);
        return HandleApiResponse(response);
    }

    private static T HandleApiResponse<T>(ApiResponse<T> response)
    {
        if (!response.Success)
        {
            throw new JiraApiException($"JIRA API call failed: {response.Message}", response.ErrorCode);
        }

        return response.Result ?? throw new JiraApiException("JIRA API returned null result");
    }
}