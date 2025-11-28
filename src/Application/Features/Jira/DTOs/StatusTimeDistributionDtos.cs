namespace Application.Features.Jira.DTOs;


public record StatusTimeDistributionDto
{
    public string ProjectKey { get; init; } = string.Empty;
    public List<IssueStatusTimeDto> Issues { get; init; } = new();
}

public record IssueStatusTimeDto
{
    public string IssueKey { get; init; } = string.Empty;
    public DateTime? Created { get; init; }
    public DateTime? ResolutionDate { get; init; }
    public string? CurrentStatus { get; init; }
    public List<StatusChangeDto> StatusChanges { get; init; } = new();
}

public record StatusChangeDto
{
    public string FromStatus { get; init; } = string.Empty;
    public string ToStatus { get; init; } = string.Empty;
    public DateTime ChangeDate { get; init; }
}