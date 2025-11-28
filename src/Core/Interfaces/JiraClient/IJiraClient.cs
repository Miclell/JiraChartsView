using Core.Models.JiraClient;

namespace Core.Interfaces.JiraClient;

public interface IJiraClient
{
    Task<JiraSearchResponse> GetOpenTimeHistogramDataAsync(string projectKey);
    Task<JiraSearchResponse> GetStatusTimeDistributionDataAsync(string projectKey);
    Task<JiraChangelogResponse> GetIssueChangelogAsync(string issueKey);
    Task<JiraSearchResponse> GetDailyTaskFlowDataAsync(string projectKey);
    Task<JiraSearchResponse> GetTopUsersDataAsync(string projectKey);
    Task<JiraSearchResponse> GetWorklogDistributionDataAsync(string projectKey);
    Task<JiraWorklogApiResponse> GetIssueWorklogAsync(string issueKey);
    Task<JiraSearchResponse> GetPriorityDistributionDataAsync(string projectKey);
}