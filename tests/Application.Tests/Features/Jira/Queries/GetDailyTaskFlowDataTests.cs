using Application.Features.Jira.Queries;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Jira.Queries;

public class GetDailyTaskFlowDataTests
{
    private readonly Mock<IJiraClient> _jiraClientMock;
    private readonly GetDailyTaskFlowDataQueryHandler _handler;

    public GetDailyTaskFlowDataTests()
    {
        _jiraClientMock = new Mock<IJiraClient>();
        _handler = new GetDailyTaskFlowDataQueryHandler(_jiraClientMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCalculateDailyFlow_WhenIssuesExist()
    {
        // Arrange
        var projectKey = "KAFKA";
        var baseDate = new DateTime(2024, 1, 1);
        var issues = new JiraSearchResponse
        {
            Issues =
            [
                new JiraIssue
                {
                    Key = "KAFKA-1",
                    Fields = new JiraFields
                    {
                        Created = baseDate,
                        ResolutionDate = baseDate.AddDays(5)
                    }
                },

                new JiraIssue
                {
                    Key = "KAFKA-2",
                    Fields = new JiraFields
                    {
                        Created = baseDate,
                        ResolutionDate = baseDate.AddDays(3)
                    }
                },

                new JiraIssue
                {
                    Key = "KAFKA-3",
                    Fields = new JiraFields
                    {
                        Created = baseDate.AddDays(1),
                        ResolutionDate = null // Open issue
                    }
                }
            ]
        };

        _jiraClientMock
            .Setup(x => x.GetDailyTaskFlowDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetDailyTaskFlowDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProjectKey.Should().Be(projectKey);
        result.DailyFlow.Should().NotBeEmpty();
        
        var firstDay = result.DailyFlow.First(d => d.Date.Date == baseDate.Date);
        firstDay.CreatedCount.Should().Be(2);
        firstDay.ResolvedCount.Should().Be(2);
    }
}

