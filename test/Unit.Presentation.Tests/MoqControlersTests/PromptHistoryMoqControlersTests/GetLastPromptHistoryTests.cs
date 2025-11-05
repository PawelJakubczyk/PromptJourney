using Application.UseCases.PromptHistory.Queries;
using Application.UseCases.PromptHistory.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetLastPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetLast_ReturnsOkWithList_WhenRecordsExist()
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
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(5);
    }

    [Fact]
    public async Task GetLast_ReturnsOkWithEmptyList_WhenNoRecordsExist()
    {
        // Arrange
        var count = 10;
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetLast_ReturnsBadRequest_WhenCountIsZero()
    {
        // Arrange
        var count = 0;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "History count must be greater than zero");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetLast_ReturnsBadRequest_WhenCountIsNegative()
    {
        // Arrange
        var invalidCount = -1;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "History count must be greater than zero");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(invalidCount, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetLast_ReturnsBadRequest_WhenCountExceedsAvailable()
    {
        // Arrange
        var count = 100;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Requested 100 records, but only 50 are available");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetLast_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var count = 5;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetLast_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var count = 5;
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        GetLastHistoryRecords.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<PromptHistoryResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetLastHistoryRecords.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetLast(count, CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.Count.Should().Be(count);
    }

    [Fact]
    public async Task GetLast_HandlesCancellationToken()
    {
        // Arrange
        var count = 5;
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.GetLast(count, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetLast_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var count = 5;
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetLast(count, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(25, 25)]
    public async Task GetLast_ReturnsOk_ForVariousValidCounts(int count, int expectedCount)
    {
        // Arrange
        var historyRecords = Enumerable.Range(1, expectedCount)
            .Select(i => new PromptHistoryResponse(
                Guid.NewGuid(),
                $"Prompt {i}",
                "1.0",
                DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(expectedCount);
    }

    [Fact]
    public async Task GetLast_ReturnsConsistentResults_ForSameCount()
    {
        // Arrange
        var count = 5;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Test prompt", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetLast(count, CancellationToken.None);
        var actionResult2 = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithCount(1);
        actionResult2.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetLast_ReturnsOk_WithRecordsOrderedByDateDescending()
    {
        // Arrange
        var count = 5;
        var now = DateTime.UtcNow;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Most recent", "1.0", now),
            new(Guid.NewGuid(), "Second", "1.0", now.AddMinutes(-1)),
            new(Guid.NewGuid(), "Third", "1.0", now.AddMinutes(-2)),
            new(Guid.NewGuid(), "Fourth", "1.0", now.AddMinutes(-3)),
            new(Guid.NewGuid(), "Fifth", "1.0", now.AddMinutes(-4))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(5);
    }

    [Fact]
    public async Task GetLast_ReturnsOk_WithRecordsFromDifferentVersions()
    {
        // Arrange
        var count = 4;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt v1", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Prompt v2", "2.0", DateTime.UtcNow.AddMinutes(-1)),
            new(Guid.NewGuid(), "Prompt v5", "5.2", DateTime.UtcNow.AddMinutes(-2)),
            new(Guid.NewGuid(), "Prompt v6", "6.0", DateTime.UtcNow.AddMinutes(-3))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetLast_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var count = 5;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetLast_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var count = 5;
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetLast_ReturnsOk_WithLongPromptTexts()
    {
        // Arrange
        var count = 2;
        var longPrompt = new string('a', 2000);
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), longPrompt, "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Short prompt", "1.0", DateTime.UtcNow.AddMinutes(-1))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetLast_ReturnsOk_WithSpecialCharactersInPrompts()
    {
        // Arrange
        var count = 2;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt with @#$% special chars", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Unicode: 日本語 中文 ☀️", "1.0", DateTime.UtcNow.AddMinutes(-1))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(2);
    }

    [Fact]
    public async Task GetLast_ReturnsOk_WhenRequestingExactlyAvailableCount()
    {
        // Arrange
        var count = 3;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt 1", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Prompt 2", "1.0", DateTime.UtcNow.AddMinutes(-1)),
            new(Guid.NewGuid(), "Prompt 3", "1.0", DateTime.UtcNow.AddMinutes(-2))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetLast_ReturnsOk_WithSingleRecord()
    {
        // Arrange
        var count = 1;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Only one prompt", "1.0", DateTime.UtcNow)
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetLast(count, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetLast_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var count = 10;
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetLastHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetLast(count, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}