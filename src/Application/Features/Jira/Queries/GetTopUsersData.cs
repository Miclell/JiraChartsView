using Application.Features.Jira.DTOs;
using Core.Interfaces.JiraClient;
using MediatR;

namespace Application.Features.Jira.Queries;

//GetTopUsersData
public record GetTopUsersDataQuery(string ProjectKey) : IRequest<TopUsersDto>;

public class GetTopUsersDataQueryHandler(IJiraClient jiraClient) : IRequestHandler<GetTopUsersDataQuery, TopUsersDto>
{
    public async Task<TopUsersDto> Handle(GetTopUsersDataQuery request, CancellationToken cancellationToken)
    {
        var response = await jiraClient.GetTopUsersDataAsync(request.ProjectKey);
        
        // Count tasks per user as reporter and assignee
        var userStats = new Dictionary<string, (int ReporterCount, int AssigneeCount)>();
        
        foreach (var issue in response.Issues)
        {
            if (issue.Fields.Reporter != null && !string.IsNullOrWhiteSpace(issue.Fields.Reporter.DisplayName))
            {
                var reporterName = issue.Fields.Reporter.DisplayName;
                if (!userStats.ContainsKey(reporterName))
                {
                    userStats[reporterName] = (0, 0);
                }
                var stats = userStats[reporterName];
                userStats[reporterName] = (stats.ReporterCount + 1, stats.AssigneeCount);
            }
            
            if (issue.Fields.Assignee != null && !string.IsNullOrWhiteSpace(issue.Fields.Assignee.DisplayName))
            {
                var assigneeName = issue.Fields.Assignee.DisplayName;
                if (!userStats.ContainsKey(assigneeName))
                {
                    userStats[assigneeName] = (0, 0);
                }
                var stats = userStats[assigneeName];
                userStats[assigneeName] = (stats.ReporterCount, stats.AssigneeCount + 1);
            }
        }
        
        var topUsers = userStats
            .Select(kvp => new UserStatsDto
            {
                UserName = kvp.Key,
                ReporterCount = kvp.Value.ReporterCount,
                AssigneeCount = kvp.Value.AssigneeCount,
                TotalCount = kvp.Value.ReporterCount + kvp.Value.AssigneeCount
            })
            .OrderByDescending(x => x.TotalCount)
            .Take(30)
            .ToList();

        return new TopUsersDto
        {
            ProjectKey = request.ProjectKey,
            TopUsers = topUsers
        };
    }
}