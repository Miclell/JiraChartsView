using Application.Features.Jira.Queries;
using Core.Interfaces.JiraClient;
using Core.Models.JiraClient;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Jira.Queries;

public class GetPriorityDistributionDataTests
{
    private readonly Mock<IJiraClient> _jiraClientMock;
    private readonly GetPriorityDistributionDataQueryHandler _handler;

    public GetPriorityDistributionDataTests()
    {
        _jiraClientMock = new Mock<IJiraClient>();
        _handler = new GetPriorityDistributionDataQueryHandler(_jiraClientMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCalculatePriorityDistribution_WhenIssuesExist()
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
                        Priority = new JiraPriority { Name = "High" }
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-2",
                    Fields = new JiraFields
                    {
                        Priority = new JiraPriority { Name = "High" }
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-3",
                    Fields = new JiraFields
                    {
                        Priority = new JiraPriority { Name = "Low" }
                    }
                },
                new JiraIssue
                {
                    Key = "KAFKA-4",
                    Fields = new JiraFields
                    {
                        Priority = new JiraPriority { Name = "Medium" }
                    }
                }
            }
        };

        _jiraClientMock
            .Setup(x => x.GetPriorityDistributionDataAsync(projectKey))
            .ReturnsAsync(issues);

        var query = new GetPriorityDistributionDataQuery(projectKey);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProjectKey.Should().Be(projectKey);
        result.TotalIssues.Should().Be(4);
        result.Distribution.Should().HaveCount(3);
        
        var highPriority = result.Distribution.First(d => d.Priority == "High");
        highPriority.Count.Should().Be(2);
        highPriority.Percentage.Should().Be(50.0);

        var lowPriority = result.Distribution.First(d => d.Priority == "Low");
        lowPriority.Count.Should().Be(1);
        lowPriority.Percentage.Should().Be(25.0);
    }
}

