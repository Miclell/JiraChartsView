using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using MediatR;

namespace Application.Features.Jira.Queries;

public record GetOpenTimeHistogramDataQuery(string ProjectKey) 
    : IRequest<JiraSearchResponse>;

public class GetOpenTimeHistogramDataQueryHandler(IJiraClient jiraClient)
    : IRequestHandler<GetOpenTimeHistogramDataQuery, JiraSearchResponse>
{
    public async Task<JiraSearchResponse> Handle(GetOpenTimeHistogramDataQuery request, 
        CancellationToken cancellationToken)
    {
        return await jiraClient.GetOpenTimeHistogramDataAsync(request.ProjectKey);
    }
}