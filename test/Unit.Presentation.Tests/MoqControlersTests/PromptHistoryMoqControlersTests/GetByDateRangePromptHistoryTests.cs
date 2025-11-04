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

public sealed class GetByDateRangePromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetByDateRange_ReturnsOkWithList_WhenRecordsInRangeExist()
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
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsOkWithEmptyList_WhenNoRecordsInRange()
    {
        // Arrange
        var from = new DateTime(2030, 1, 1);
        var to = new DateTime(2030, 12, 31);
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenFromDateIsAfterToDate()
    {
        // Arrange
        var from = new DateTime(2024, 12, 31);
        var to = new DateTime(2024, 1, 1); // 'from' date after 'to' date
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Date range is not chronological: 'From' is after 'To'");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenFromDateIsInFuture()
    {
        // Arrange
        var from = DateTime.UtcNow.AddDays(1);
        var to = DateTime.UtcNow.AddDays(2);
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Date cannot be in the future");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenToDateIsInFuture()
    {
        // Arrange
        var from = DateTime.UtcNow.AddDays(-1);
        var to = DateTime.UtcNow.AddDays(1);
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Date cannot be in the future");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenBothDatesAreInFuture()
    {
        // Arrange
        var from = DateTime.UtcNow.AddDays(1);
        var to = DateTime.UtcNow.AddDays(2);
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Dates cannot be in the future");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        GetHistoryByDateRange.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<PromptHistoryResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetHistoryByDateRange.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(from, capturedQuery!.From);
        Assert.Equal(to, capturedQuery.To);
    }

    [Fact]
    public async Task GetByDateRange_HandlesCancellationToken()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByDateRange(from, to, cts.Token));
    }

    [Fact]
    public async Task GetByDateRange_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsOk_WithSameDayRange()
    {
        // Arrange
        var from = new DateTime(2024, 6, 15);
        var to = new DateTime(2024, 6, 15); // Same day
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt on that day", "1.0", new DateTime(2024, 6, 15))
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1);
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-31", 5)]
    [InlineData("2024-06-01", "2024-06-30", 10)]
    [InlineData("2024-01-01", "2024-12-31", 100)]
    public async Task GetByDateRange_ReturnsOk_ForVariousDateRanges(string fromStr, string toStr, int count)
    {
        // Arrange
        var from = DateTime.Parse(fromStr);
        var to = DateTime.Parse(toStr);
        var historyRecords = Enumerable.Range(1, count)
            .Select(i => new PromptHistoryResponse(
                Guid.NewGuid(),
                $"Prompt {i}",
                "1.0",
                from.AddDays(i % (to - from).Days)))
            .ToList();

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, count);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Test prompt", "1.0", new DateTime(2024, 6, 15))
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByDateRange(from, to, CancellationToken.None);
        var actionResult2 = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult1, 1);
        AssertOkResult<PromptHistoryResponse>(actionResult2, 1);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsOk_WithVeryLongDateRange()
    {
        // Arrange
        var from = new DateTime(2020, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Old prompt", "1.0", new DateTime(2020, 6, 15)),
            new(Guid.NewGuid(), "Recent prompt", "1.0", new DateTime(2024, 6, 15))
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsOk_WithRecordsFromDifferentVersions()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt v1", "1.0", new DateTime(2024, 3, 15)),
            new(Guid.NewGuid(), "Prompt v2", "2.0", new DateTime(2024, 6, 15)),
            new(Guid.NewGuid(), "Prompt v5.2", "5.2", new DateTime(2024, 9, 15))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsOk_WithEdgeCaseDateAtMidnight()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1, 0, 0, 0);
        var to = new DateTime(2024, 12, 31, 23, 59, 59);
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Start of year", "1.0", new DateTime(2024, 1, 1, 0, 0, 1)),
            new(Guid.NewGuid(), "End of year", "1.0", new DateTime(2024, 12, 31, 23, 59, 58))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsOk_WithShortDateRange()
    {
        // Arrange
        var from = new DateTime(2024, 6, 15);
        var to = new DateTime(2024, 6, 16); // 1 day range
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt", "1.0", new DateTime(2024, 6, 15, 12, 0, 0))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1);
    }

    [Fact]
    public async Task GetByDateRange_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetHistoryByDateRange.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByDateRange(from, to, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}