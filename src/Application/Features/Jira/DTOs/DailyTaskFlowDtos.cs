namespace Application.Features.Jira.DTOs;

public record DailyTaskFlowDto
{
    public string ProjectKey { get; init; } = string.Empty;
    public List<DailyFlowItemDto> DailyFlow { get; init; } = new();
}

public record DailyFlowItemDto
{
    public DateTime Date { get; init; }
    public int CreatedCount { get; init; }
    public int ResolvedCount { get; init; }
}