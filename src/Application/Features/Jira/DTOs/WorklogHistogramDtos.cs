namespace Application.Features.Jira.DTOs;

public record WorklogHistogramDto
{
    public string ProjectKey { get; init; } = string.Empty;
    public List<WorklogHistogramItemDto> Histogram { get; init; } = new();
}

public record WorklogHistogramItemDto
{
    public string TimeRange { get; init; } = string.Empty;
    public int TaskCount { get; init; }
}

