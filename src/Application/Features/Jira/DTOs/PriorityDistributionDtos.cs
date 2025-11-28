namespace Application.Features.Jira.DTOs;

public record PriorityDistributionDto
{
    public string ProjectKey { get; init; } = string.Empty;
    public int TotalIssues { get; init; }
    public List<PriorityStatsDto> Distribution { get; init; } = new();
}

public record PriorityStatsDto
{
    public string Priority { get; init; } = string.Empty;
    public int Count { get; init; }
    public double Percentage { get; init; }
}