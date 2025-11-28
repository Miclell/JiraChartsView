using Application.Features.Jira.Queries;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Jira.Queries;

public class GetStatusTimeDistributionDataTests
{
    private readonly Mock<IJiraClient> _jiraClientMock;
    private readonly GetStatusTimeDistributionDataQueryHandler _handler;

    public GetStatusTimeDistributionDataTests()
    {
        _jiraClientMock = new Mock<IJiraClient>();
        _handler = new GetStatusTimeDistributionDataQueryHandler(_jiraClientMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldProcessClosedIssuesOnly()
    {
        // Arrange
        var projectKey = "KAFKA";
        var issues = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>
            {
                new JiraIssue
                {
                    Key = "KAFKA-1",
                    Fields = new JiraFields
                    {
                        Created = DateTime.UtcNow.AddDays(-10),
                        Status = new JiraStatus { Name = "Closed" }
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-2",
                    Fields = new JiraFields
                    {
                        Created = DateTime.UtcNow.AddDays(-5),
                        Status = new JiraStatus { Name = "Open" }
                    }
                }
            }
        };

        var changelog = new JiraChangelogResponse
        {
            Histories = new List<JiraHistory>()
        };

        _jiraClientMock
            .Setup(x => x.GetStatusTimeDistributionDataAsync(projectKey))
            .ReturnsAsync(issues);

        _jiraClientMock
            .Setup(x => x.GetIssueChangelogAsync(It.IsAny<string>()))
            .ReturnsAsync(changelog);

        var query = new GetStatusTimeDistributionDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProjectKey.Should().Be(projectKey);
        result.Issues.Should().HaveCount(1);
        result.Issues[0].IssueKey.Should().Be("KAFKA-1");
    }

    [Fact]
    public async Task Handle_ShouldExtractStatusChanges_WhenChangelogExists()
    {
        // Arrange
        var projectKey = "KAFKA";
        var issueKey = "KAFKA-1";
        var issues = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>
            {
                new JiraIssue
                {
                    Key = issueKey,
                    Fields = new JiraFields
                    {
                        Created = DateTime.UtcNow.AddDays(-10),
                        Status = new JiraStatus { Name = "Closed" },
                        ResolutionDate = DateTime.UtcNow
                    }
                }
            }
        };

        var changelog = new JiraChangelogResponse
        {
            Histories = new List<JiraHistory>
            {
                new JiraHistory
                {
                    Created = DateTime.UtcNow.AddDays(-5),
                    Items = new List<JiraChangeItem>
                    {
                        new JiraChangeItem
                        {
                            Field = "status",
                            FromString = "Open",
                            ToString = "In Progress"
                        }
                    }
                },
                new JiraHistory
                {
                    Created = DateTime.UtcNow.AddDays(-1),
                    Items = new List<JiraChangeItem>
                    {
                        new JiraChangeItem
                        {
                            Field = "status",
                            FromString = "In Progress",
                            ToString = "Closed"
                        }
                    }
                }
            }
        };

        _jiraClientMock
            .Setup(x => x.GetStatusTimeDistributionDataAsync(projectKey))
            .ReturnsAsync(issues);

        _jiraClientMock
            .Setup(x => x.GetIssueChangelogAsync(issueKey))
            .ReturnsAsync(changelog);

        var query = new GetStatusTimeDistributionDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Issues[0].StatusChanges.Should().HaveCount(2);
        result.Issues[0].StatusChanges[0].FromStatus.Should().Be("Open");
        result.Issues[0].StatusChanges[0].ToStatus.Should().Be("In Progress");
        result.Issues[0].StatusChanges[1].FromStatus.Should().Be("In Progress");
        result.Issues[0].StatusChanges[1].ToStatus.Should().Be("Closed");
    }
}

