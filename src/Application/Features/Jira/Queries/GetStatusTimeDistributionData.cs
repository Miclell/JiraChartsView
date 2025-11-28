using Application.Features.Jira.DTOs;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using MediatR;

namespace Application.Features.Jira.Queries;

//GetStatusTimeDistributionData
public record GetStatusTimeDistributionDataQuery(string ProjectKey) : IRequest<StatusTimeDistributionDto>;

public class GetStatusTimeDistributionDataQueryHandler : IRequestHandler<GetStatusTimeDistributionDataQuery, StatusTimeDistributionDto>
{
    private readonly IJiraClient _jiraClient;

    public GetStatusTimeDistributionDataQueryHandler(IJiraClient jiraClient)
    {
        _jiraClient = jiraClient;
    }

    public async Task<StatusTimeDistributionDto> Handle(GetStatusTimeDistributionDataQuery request, CancellationToken cancellationToken)
    {
        var issues = await _jiraClient.GetStatusTimeDistributionDataAsync(request.ProjectKey);
        
        var result = new StatusTimeDistributionDto
        {
            ProjectKey = request.ProjectKey,
            Issues = []
        };

        foreach (var issue in issues.Issues)
        {
            // Only process closed tasks
            var currentStatus = issue.Fields.Status?.Name ?? string.Empty;
            var isClosed = currentStatus.Contains("Closed", StringComparison.OrdinalIgnoreCase) ||
                          currentStatus.Contains("Done", StringComparison.OrdinalIgnoreCase) ||
                          currentStatus.Contains("Resolved", StringComparison.OrdinalIgnoreCase) ||
                          currentStatus.Contains("Закрыт", StringComparison.OrdinalIgnoreCase) ||
                          currentStatus.Contains("Решено", StringComparison.OrdinalIgnoreCase);
            
            if (!isClosed) continue;
            
            var changelog = await _jiraClient.GetIssueChangelogAsync(issue.Key);
            var statusChanges = ExtractStatusChanges(issue, changelog);
            
            result.Issues.Add(new IssueStatusTimeDto
            {
                IssueKey = issue.Key,
                Created = issue.Fields.Created,
                ResolutionDate = issue.Fields.ResolutionDate,
                CurrentStatus = issue.Fields.Status?.Name,
                StatusChanges = statusChanges
            });
        }

        return result;
    }

    private List<StatusChangeDto> ExtractStatusChanges(JiraIssue issue, JiraChangelogResponse changelog)
    {
        var statusChanges = new List<StatusChangeDto>();
        
        foreach (var history in changelog.Histories)
        {
            var statusItem = history.Items.FirstOrDefault(x => x.Field == "status");
            if (statusItem != null)
            {
                statusChanges.Add(new StatusChangeDto
                {
                    FromStatus = statusItem.FromString,
                    ToStatus = statusItem.ToString,
                    ChangeDate = history.Created
                });
            }
        }
        
        return statusChanges;
    }
}