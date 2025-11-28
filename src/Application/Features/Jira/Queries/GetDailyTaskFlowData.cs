using Application.Features.Jira.DTOs;
using Core.Interfaces.JiraClient;
using MediatR;

namespace Application.Features.Jira.Queries;

public record GetDailyTaskFlowDataQuery(string ProjectKey) : IRequest<DailyTaskFlowDto>;

public class GetDailyTaskFlowDataQueryHandler(IJiraClient jiraClient)
    : IRequestHandler<GetDailyTaskFlowDataQuery, DailyTaskFlowDto>
{
    public async Task<DailyTaskFlowDto> Handle(GetDailyTaskFlowDataQuery request, CancellationToken cancellationToken)
    {
        var response = await jiraClient.GetDailyTaskFlowDataAsync(request.ProjectKey);
        
        var dailyFlow = response.Issues
            .GroupBy(x => x.Fields.Created.Date)
            .Select(g => new DailyFlowItemDto
            {
                Date = g.Key,
                CreatedCount = g.Count(),
                ResolvedCount = g.Count(x => x.Fields.ResolutionDate.HasValue)
            })
            .OrderBy(x => x.Date)
            .ToList();

        return new DailyTaskFlowDto
        {
            ProjectKey = request.ProjectKey,
            DailyFlow = dailyFlow
        };
    }
}