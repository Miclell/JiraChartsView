using Application.Features.Jira.Queries;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Jira.Queries;

public class GetOpenTimeHistogramDataTests
{
    private readonly Mock<IJiraClient> _jiraClientMock;
    private readonly GetOpenTimeHistogramDataQueryHandler _handler;

    public GetOpenTimeHistogramDataTests()
    {
        _jiraClientMock = new Mock<IJiraClient>();
        _handler = new GetOpenTimeHistogramDataQueryHandler(_jiraClientMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnJiraSearchResponse_WhenValidProjectKey()
    {
        // Arrange
        var projectKey = "KAFKA";
        var expectedResponse = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>
            {
                new JiraIssue
                {
                    Key = "KAFKA-123",
                    Fields = new JiraFields
                    {
                        Created = DateTime.UtcNow.AddDays(-10),
                        ResolutionDate = DateTime.UtcNow
                    }
                }
            }
        };

        _jiraClientMock
            .Setup(x => x.GetOpenTimeHistogramDataAsync(projectKey))
            .ReturnsAsync(expectedResponse);

        var query = new GetOpenTimeHistogramDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Issues.Should().HaveCount(1);
        result.Issues[0].Key.Should().Be("KAFKA-123");
        _jiraClientMock.Verify(x => x.GetOpenTimeHistogramDataAsync(projectKey), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyResponse_WhenNoIssuesFound()
    {
        // Arrange
        var projectKey = "EMPTY";
        var expectedResponse = new JiraSearchResponse
        {
            Issues = new List<JiraIssue>()
        };

        _jiraClientMock
            .Setup(x => x.GetOpenTimeHistogramDataAsync(projectKey))
            .ReturnsAsync(expectedResponse);

        var query = new GetOpenTimeHistogramDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Issues.Should().BeEmpty();
    }
}

