using Application.Features.Jira.DTOs;
using Core.Interfaces.JiraClient;
using MediatR;

namespace Application.Features.Jira.Queries;

//GetPriorityDistributionData
public record GetPriorityDistributionDataQuery(string ProjectKey) : IRequest<PriorityDistributionDto>;

public class GetPriorityDistributionDataQueryHandler : IRequestHandler<GetPriorityDistributionDataQuery, PriorityDistributionDto>
{
    private readonly IJiraClient _jiraClient;

    public GetPriorityDistributionDataQueryHandler(IJiraClient jiraClient)
    {
        _jiraClient = jiraClient;
    }

    public async Task<PriorityDistributionDto> Handle(GetPriorityDistributionDataQuery request, CancellationToken cancellationToken)
    {
        var response = await _jiraClient.GetPriorityDistributionDataAsync(request.ProjectKey);
        
        var distribution = response.Issues
            .Where(x => x.Fields.Priority != null)
            .GroupBy(x => x.Fields.Priority.Name)
            .Select(g => new PriorityStatsDto
            {
                Priority = g.Key,
                Count = g.Count(),
                Percentage = (double)g.Count() / response.Issues.Count * 100
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return new PriorityDistributionDto
        {
            ProjectKey = request.ProjectKey,
            TotalIssues = response.Issues.Count,
            Distribution = distribution
        };
    }
}