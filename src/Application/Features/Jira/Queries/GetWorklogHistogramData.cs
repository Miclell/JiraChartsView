using Application.Features.Jira.DTOs;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using MediatR;

namespace Application.Features.Jira.Queries;

public record GetWorklogHistogramDataQuery(string ProjectKey) : IRequest<WorklogHistogramDto>;

public class GetWorklogHistogramDataQueryHandler : IRequestHandler<GetWorklogHistogramDataQuery, WorklogHistogramDto>
{
    private readonly IJiraClient _jiraClient;

    public GetWorklogHistogramDataQueryHandler(IJiraClient jiraClient)
    {
        _jiraClient = jiraClient;
    }

    public async Task<WorklogHistogramDto> Handle(GetWorklogHistogramDataQuery request, CancellationToken cancellationToken)
    {
        // Get closed tasks - use open time histogram endpoint which already has created and resolutiondate
        var issues = await _jiraClient.GetOpenTimeHistogramDataAsync(request.ProjectKey);
        
        if (issues?.Issues == null || issues.Issues.Count == 0)
        {
            return new WorklogHistogramDto
            {
                ProjectKey = request.ProjectKey,
                Histogram = new List<WorklogHistogramItemDto>()
            };
        }
        
        // Calculate time from creation to resolution (instead of worklog time)
        var taskTimes = new List<double>(); // hours
        
        foreach (var issue in issues.Issues)
        {
            try
            {
                if (issue.Fields?.Created == null || issue.Fields?.ResolutionDate == null)
                {
                    continue;
                }
                
                var created = issue.Fields.Created;
                var resolved = issue.Fields.ResolutionDate.Value;
                
                var timeSpan = resolved - created;
                var totalHours = timeSpan.TotalHours;
                
                if (totalHours > 0)
                {
                    taskTimes.Add(totalHours);
                }
            }
            catch
            {
                // Skip issues with invalid dates
                continue;
            }
        }
        
        // Create histogram bins (0-1, 1-4, 4-8, 8-16, 16-40, 40-80, 80+ hours)
        // But since we're using days, let's use more appropriate bins: 0-1 hour, 1-4 hours, 4-8 hours, 8-24 hours, 1-7 days, 7-30 days, 30+ days
        var bins = new[] { 0.0, 1.0, 4.0, 8.0, 24.0, 168.0, 720.0, double.PositiveInfinity }; // hours: 1h, 4h, 8h, 24h (1 day), 168h (7 days), 720h (30 days)
        var histogram = new List<WorklogHistogramItemDto>();
        
        for (int i = 0; i < bins.Length - 1; i++)
        {
            var start = bins[i];
            var end = bins[i + 1];
            var count = taskTimes.Count(time => time >= start && time < end);
            
            string range;
            if (end == double.PositiveInfinity)
            {
                var days = (int)(start / 24);
                range = $"{days}+ дней";
            }
            else if (start == 0)
            {
                if (end <= 24)
                {
                    range = $"<{end} часов";
                }
                else
                {
                    var days = (int)(end / 24);
                    range = $"<{days} дней";
                }
            }
            else if (end <= 24)
            {
                range = $"{start}-{end} часов";
            }
            else
            {
                var startDays = (int)(start / 24);
                var endDays = (int)(end / 24);
                range = $"{startDays}-{endDays} дней";
            }
            
            histogram.Add(new WorklogHistogramItemDto
            {
                TimeRange = range,
                TaskCount = count
            });
        }
        
        // Return histogram even if empty, so frontend can show appropriate message
        return new WorklogHistogramDto
        {
            ProjectKey = request.ProjectKey,
            Histogram = histogram
        };
    }
}

