using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Features.PromptHistory.Responses;
using Utilities.Constants;
using FluentAssertions;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetLastPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetLast_ReturnsOk_WhenRecordsExist()
    {
        // Arrange
        var count = 5;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Recent prompt 1", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Recent prompt 2", "1.0", DateTime.UtcNow.AddMinutes(-1)),
            new(Guid.NewGuid(), "Recent prompt 3", "1.0", DateTime.UtcNow.AddMinutes(-2)),
            new(Guid.NewGuid(), "Recent prompt 4", "1.0", DateTime.UtcNow.AddMinutes(-3)),
            new(Guid.NewGuid(), "Recent prompt 5", "1.0", DateTime.UtcNow.AddMinutes(-4))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 5);
    }

    [Fact]
    public async Task GetLast_ReturnsEmptyList_WhenNoRecordsExist()
    {
        // Arrange
        var count = 10;
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetLast_ReturnsBadRequest_WhenCountInvalid()
    {
        // Arrange
        var invalidCount = -1;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>>(
            StatusCodes.Status400BadRequest,
            "Count must be positive",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(invalidCount, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetLast_ReturnsPartialResults_WhenRequestedCountExceedsAvailable()
    {
        // Arrange
        var count = 10;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Only record 1", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Only record 2", "1.0", DateTime.UtcNow.AddMinutes(-1))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetLast_VerifiesCorrectQueryIsSent()
    {
        // Arrange
        var count = 5;
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetLast(count, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.Is<Application.Features.PromptHistory.Queries.GetLastHistoryRecords.Query>(
                q => q.Count == count),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task GetLast_PassesCountParameterCorrectly(int count)
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetLast(count, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.Is<Application.Features.PromptHistory.Queries.GetLastHistoryRecords.Query>(
                q => q.Count == count),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}