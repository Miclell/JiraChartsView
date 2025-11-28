using Application.Features.Jira.Queries;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Jira.Queries;

public class GetWorklogHistogramDataTests
{
    private readonly Mock<IJiraClient> _jiraClientMock;
    private readonly GetWorklogHistogramDataQueryHandler _handler;

    public GetWorklogHistogramDataTests()
    {
        _jiraClientMock = new Mock<IJiraClient>();
        _handler = new GetWorklogHistogramDataQueryHandler(_jiraClientMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCalculateHistogram_WhenClosedIssuesExist()
    {
        // Arrange
        var projectKey = "KAFKA";
        var baseDate = DateTime.UtcNow;
        var issues = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>
            {
                new JiraIssue
                {
                    Key = "KAFKA-1",
                    Fields = new JiraFields
                    {
                        Created = baseDate.AddHours(-2),
                        ResolutionDate = baseDate
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-2",
                    Fields = new JiraFields
                    {
                        Created = baseDate.AddHours(-5),
                        ResolutionDate = baseDate
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-3",
                    Fields = new JiraFields
                    {
                        Created = baseDate.AddDays(-10),
                        ResolutionDate = baseDate
                    }
                }
            }
        };

        _jiraClientMock
            .Setup(x => x.GetOpenTimeHistogramDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetWorklogHistogramDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProjectKey.Should().Be(projectKey);
        result.Histogram.Should().NotBeEmpty();
        result.Histogram.Sum(h => h.TaskCount).Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyHistogram_WhenNoClosedIssues()
    {
        // Arrange
        var projectKey = "KAFKA";
        var issues = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>()
        };

        _jiraClientMock
            .Setup(x => x.GetOpenTimeHistogramDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetWorklogHistogramDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Histogram.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldSkipIssuesWithoutResolutionDate()
    {
        // Arrange
        var projectKey = "KAFKA";
        var baseDate = DateTime.UtcNow;
        var issues = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>
            {
                new JiraIssue
                {
                    Key = "KAFKA-1",
                    Fields = new JiraFields
                    {
                        Created = baseDate.AddHours(-2),
                        ResolutionDate = null // Open issue
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-2",
                    Fields = new JiraFields
                    {
                        Created = baseDate.AddHours(-5),
                        ResolutionDate = baseDate
                    }
                }
            }
        };

        _jiraClientMock
            .Setup(x => x.GetOpenTimeHistogramDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetWorklogHistogramDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Histogram.Sum(h => h.TaskCount).Should().Be(1);
    }
}

