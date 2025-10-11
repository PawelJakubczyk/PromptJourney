using Application.UseCases.PromptHistory.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetByKeywordPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetByKeyword_ReturnsOk_WhenRecordsWithKeywordExist()
    {
        // Arrange
        var keyword = "landscape";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "A beautiful landscape with mountains", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Urban landscape at sunset", "2.0", DateTime.UtcNow.AddHours(-1))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsEmptyList_WhenNoRecordsMatchKeyword()
    {
        // Arrange
        var keyword = "nonexistent";
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenKeywordInvalid()
    {
        // Arrange
        var invalidKeyword = "";
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(invalidKeyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_ReturnsBadRequest_WhenKeywordTooShort()
    {
        // Arrange
        var shortKeyword = "a"; // Too short
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Keyword must be at least 2 characters long");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByKeyword(shortKeyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByKeyword_VerifiesCorrectQueryIsSent()
    {
        // Arrange
        var keyword = "test";
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.Is<Application.UseCases.PromptHistory.Queries.GetHistoryRecordsByPromptKeyword.Query>(
                q => q.Keyword == keyword),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("landscape")]
    [InlineData("portrait")]
    [InlineData("art")]
    [InlineData("modern")]
    public async Task GetByKeyword_PassesKeywordParameterCorrectly(string keyword)
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
        await controller.GetByKeyword(keyword, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(
            It.Is<Application.UseCases.PromptHistory.Queries.GetHistoryRecordsByPromptKeyword.Query>(
                q => q.Keyword == keyword),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
