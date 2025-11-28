using Application.Features.Jira.DTOs;
using Application.Features.Jira.Queries;
using Core.Models.JiraClient;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;

namespace Web.Tests.Controllers;

public class JiraAnalyticsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly JiraAnalyticsController _controller;

    public JiraAnalyticsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new JiraAnalyticsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetOpenTimeHistogram_ShouldReturnOkResult_WithData()
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
                    Fields = new JiraFields()
                }
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOpenTimeHistogramDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetOpenTimeHistogram(projectKey);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
        _mediatorMock.Verify(m => m.Send(It.Is<GetOpenTimeHistogramDataQuery>(q => q.ProjectKey == projectKey), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetStatusTimeDistribution_ShouldReturnOkResult_WithData()
    {
        // Arrange
        var projectKey = "KAFKA";
        var expectedResponse = new StatusTimeDistributionDto
        {
            ProjectKey = projectKey,
            Issues = new List<IssueStatusTimeDto>()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStatusTimeDistributionDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetStatusTimeDistribution(projectKey);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GetDailyTaskFlow_ShouldReturnOkResult_WithData()
    {
        // Arrange
        var projectKey = "KAFKA";
        var expectedResponse = new DailyTaskFlowDto
        {
            ProjectKey = projectKey,
            DailyFlow = new List<DailyFlowItemDto>()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDailyTaskFlowDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetDailyTaskFlow(projectKey);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GetTopUsers_ShouldReturnOkResult_WithData()
    {
        // Arrange
        var projectKey = "KAFKA";
        var expectedResponse = new TopUsersDto
        {
            ProjectKey = projectKey,
            TopUsers = new List<UserStatsDto>()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTopUsersDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetTopUsers(projectKey);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GetPriorityDistribution_ShouldReturnOkResult_WithData()
    {
        // Arrange
        var projectKey = "KAFKA";
        var expectedResponse = new PriorityDistributionDto
        {
            ProjectKey = projectKey,
            TotalIssues = 0,
            Distribution = new List<PriorityStatsDto>()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetPriorityDistributionDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetPriorityDistribution(projectKey);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GetWorklogHistogram_ShouldReturnOkResult_WithData()
    {
        // Arrange
        var projectKey = "KAFKA";
        var expectedResponse = new WorklogHistogramDto
        {
            ProjectKey = projectKey,
            Histogram = new List<WorklogHistogramItemDto>()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetWorklogHistogramDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetWorklogHistogram(projectKey);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedResponse);
    }

    [Theory]
    [InlineData("KAFKA")]
    [InlineData("PROJ")]
    [InlineData("TEST")]
    public async Task GetOpenTimeHistogram_ShouldAcceptDifferentProjectKeys(string projectKey)
    {
        // Arrange
        var expectedResponse = new JiraSearchResponse { Issues = new List<JiraIssue>() };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOpenTimeHistogramDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetOpenTimeHistogram(projectKey);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<GetOpenTimeHistogramDataQuery>(q => q.ProjectKey == projectKey), It.IsAny<CancellationToken>()), Times.Once);
    }
}

