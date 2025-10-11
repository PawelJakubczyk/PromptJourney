using Application.Features.PromptHistory.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetAllPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOk_WhenHistoryRecordsExist()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "A beautiful landscape", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Modern city skyline", "2.0", DateTime.UtcNow)
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoHistoryRecordsExist()
    {
        // Arrange
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_WhenHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetAll_VerifiesCorrectQueryIsSent()
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
        await controller.GetAll(CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.IsAny<Application.Features.PromptHistory.Queries.GetAllHistoryRecords.Query>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_HandlesCancellationTokenCorrectly()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        var cancellationToken = new CancellationToken(true); // Cancelled token

        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(cancellationToken);

        // Assert
        senderMock.Verify(s => s.Send(
            It.IsAny<object>(),
            cancellationToken),
            Times.Once);
    }
}
