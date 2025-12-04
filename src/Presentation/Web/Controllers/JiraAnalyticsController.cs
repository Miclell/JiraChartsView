using Application.Features.Jira.DTOs;
using Application.Features.Jira.Queries;
using Core.Models.JiraClient;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]/{projectKey}")]
public class JiraAnalyticsController(IMediator mediator) : ControllerBase
{
    [HttpGet("open-time-histogram")]
    public async Task<ActionResult<JiraSearchResponse>> GetOpenTimeHistogram(string projectKey)
    {
        var query = new GetOpenTimeHistogramDataQuery(projectKey);
        var result = await mediator.Send(query);
        return Ok(result);
    }
                                            
    [HttpGet("status-time-distribution")]
    public async Task<ActionResult<StatusTimeDistributionDto>> GetStatusTimeDistribution(string projectKey)
    {
        var query = new GetStatusTimeDistributionDataQuery(projectKey);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("daily-task-flow")]
    public async Task<ActionResult<DailyTaskFlowDto>> GetDailyTaskFlow(string projectKey)
    {
        var query = new GetDailyTaskFlowDataQuery(projectKey);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("top-users")]
    public async Task<ActionResult<TopUsersDto>> GetTopUsers(string projectKey)
    {
        var query = new GetTopUsersDataQuery(projectKey);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("priority-distribution")]
    public async Task<ActionResult<PriorityDistributionDto>> GetPriorityDistribution(string projectKey)
    {
        var query = new GetPriorityDistributionDataQuery(projectKey);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("worklog-histogram")]
    public async Task<ActionResult<WorklogHistogramDto>> GetWorklogHistogram(string projectKey)
    {
        var query = new GetWorklogHistogramDataQuery(projectKey);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}