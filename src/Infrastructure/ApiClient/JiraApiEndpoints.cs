namespace Infrastructure.ApiClient;

public static class JiraApiEndpoints
{
    public static string GetOpenTimeHistogramData(string projectKey)
    {
        return $"search?jql=project={projectKey}+AND+status=Closed&fields=created,resolutiondate";
    }

    public static string GetStatusTimeDistributionData(string projectKey)
    {
        return $"search?jql=project={projectKey}+AND+status+was+Closed&fields=status,created,resolutiondate";
    }

    public static string GetIssueChangelog(string issueKey)
    {
        return $"issue/{issueKey}?expand=changelog";
    }

    public static string GetDailyTaskFlowData(string projectKey)
    {
        return $"search?jql=project={projectKey}&fields=created,status,resolutiondate";
    }

    public static string GetTopUsersData(string projectKey)
    {
        return $"search?jql=project={projectKey}&fields=reporter,assignee";
    }

    public static string GetWorklogDistributionData(string projectKey)
    {
        return $"search?jql=project={projectKey}+AND+status=Closed&fields=worklog";
    }

    public static string GetIssueWorklog(string issueKey)
    {
        return $"issue/{issueKey}/worklog";
    }

    public static string GetPriorityDistributionData(string projectKey)
    {
        return $"search?jql=project={projectKey}&fields=priority";
    }
}