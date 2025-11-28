namespace Application.Features.Jira.DTOs;

public record TopUsersDto
{
    public string ProjectKey { get; init; } = string.Empty;
    public List<UserStatsDto> TopUsers { get; init; } = new();
}

public record UserStatsDto
{
    public string UserName { get; init; } = string.Empty;
    public int TotalCount { get; init; }
    public int ReporterCount { get; init; }
    public int AssigneeCount { get; init; }
}