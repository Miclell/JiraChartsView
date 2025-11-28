using Application.Features.Jira.Queries;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Jira.Queries;

public class GetTopUsersDataTests
{
    private readonly Mock<IJiraClient> _jiraClientMock;
    private readonly GetTopUsersDataQueryHandler _handler;

    public GetTopUsersDataTests()
    {
        _jiraClientMock = new Mock<IJiraClient>();
        _handler = new GetTopUsersDataQueryHandler(_jiraClientMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTop30Users_WhenMultipleUsersExist()
    {
        // Arrange
        var projectKey = "KAFKA";
        var issues = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>()
        };

        // Create 40 users with varying task counts
        for (int i = 1; i <= 40; i++)
        {
            var issue = new JiraIssue
            {
                Key = $"KAFKA-{i}",
                Fields = new JiraFields
                {
                    Reporter = new JiraUser { DisplayName = $"User{i}" },
                    Assignee = new JiraUser { DisplayName = $"User{i}" }
                }
            };
            
            // Add multiple issues for some users to test sorting
            for (int j = 0; j < i; j++)
            {
                issues.Issues.Add(issue);
            }
        }

        _jiraClientMock
            .Setup(x => x.GetTopUsersDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetTopUsersDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TopUsers.Should().HaveCount(30);
        result.TopUsers.First().TotalCount.Should().BeGreaterThan(result.TopUsers.Last().TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldCalculateTotalCountCorrectly()
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
                        Reporter = new JiraUser { DisplayName = "John Doe" },
                        Assignee = new JiraUser { DisplayName = "Jane Smith" }
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-2",
                    Fields = new JiraFields
                    {
                        Reporter = new JiraUser { DisplayName = "John Doe" },
                        Assignee = new JiraUser { DisplayName = "John Doe" }
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-3",
                    Fields = new JiraFields
                    {
                        Reporter = new JiraUser { DisplayName = "Jane Smith" },
                        Assignee = new JiraUser { DisplayName = "Jane Smith" }
                    }
                }
            }
        };

        _jiraClientMock
            .Setup(x => x.GetTopUsersDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetTopUsersDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TopUsers.Should().HaveCount(2);
        
        var johnDoe = result.TopUsers.First(u => u.UserName == "John Doe");
        johnDoe.ReporterCount.Should().Be(2);
        johnDoe.AssigneeCount.Should().Be(1);
        johnDoe.TotalCount.Should().Be(3);

        var janeSmith = result.TopUsers.First(u => u.UserName == "Jane Smith");
        janeSmith.ReporterCount.Should().Be(1);
        janeSmith.AssigneeCount.Should().Be(2);
        janeSmith.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldIgnoreNullUsers()
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
                        Reporter = null,
                        Assignee = new JiraUser { DisplayName = "John Doe" }
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-2",
                    Fields = new JiraFields
                    {
                        Reporter = new JiraUser { DisplayName = "Jane Smith" },
                        Assignee = null
                    }
                }
            }
        };

        _jiraClientMock
            .Setup(x => x.GetTopUsersDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetTopUsersDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TopUsers.Should().HaveCount(2);
        result.TopUsers.Should().Contain(u => u.UserName == "John Doe" && u.AssigneeCount == 1);
        result.TopUsers.Should().Contain(u => u.UserName == "Jane Smith" && u.ReporterCount == 1);
    }
}

