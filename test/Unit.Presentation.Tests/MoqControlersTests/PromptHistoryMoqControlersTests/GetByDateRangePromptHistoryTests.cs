using Application.UseCases.PromptHistory.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetByDateRangePromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetByDateRange_ReturnsOk_WhenRecordsInRangeExist()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt in range 1", "1.0", new DateTime(2024, 6, 15)),
            new(Guid.NewGuid(), "Prompt in range 2", "1.0", new DateTime(2024, 8, 20))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsEmptyList_WhenNoRecordsInRange()
    {
        // Arrange
        var from = new DateTime(2030, 1, 1);
        var to = new DateTime(2030, 12, 31);
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenDateRangeInvalid()
    {
        // Arrange
        var from = new DateTime(2024, 12, 31);
        var to = new DateTime(2024, 1, 1); // 'from' date after 'to' date
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "From date must be before to date");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenDateRangeTooLarge()
    {
        // Arrange
        var from = new DateTime(2000, 1, 1);
        var to = new DateTime(2030, 12, 31); // Very large range
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Date range is too large");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_VerifiesCorrectQueryIsSent()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.Is<Application.UseCases.PromptHistory.Queries.GetHistoryByDateRange.Query>(
                q => q.From == from && q.To == to),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByDateRange_HandlesMinMaxDates()
    {
        // Arrange
        var from = DateTime.MinValue;
        var to = DateTime.MaxValue;
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
        senderMock.Verify(s => s.Send(
            It.Is<Application.UseCases.PromptHistory.Queries.GetHistoryByDateRange.Query>(
                q => q.From == from && q.To == to),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
