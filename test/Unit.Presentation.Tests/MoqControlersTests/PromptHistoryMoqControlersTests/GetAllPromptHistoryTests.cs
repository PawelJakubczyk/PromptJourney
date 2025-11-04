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

public sealed class GetAllPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOkWithList_WhenHistoryRecordsExist()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "A beautiful landscape", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Modern city skyline", "2.0", DateTime.UtcNow.AddMinutes(-5))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEmptyList_WhenNoHistoryRecordsExist()
    {
        // Arrange
        var emptyList = new List<PromptHistoryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_UsesSingletonQuery()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        GetAllHistoryRecords.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<PromptHistoryResponse>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetAllHistoryRecords.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Same(GetAllHistoryRecords.Query.Singletone, capturedQuery);
    }

    [Fact]
    public async Task GetAll_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetAll(cts.Token));
    }

    [Fact]
    public async Task GetAll_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Test prompt", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetAll(CancellationToken.None);
        var actionResult2 = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult1, 1);
        AssertOkResult<PromptHistoryResponse>(actionResult2, 1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task GetAll_ReturnsOk_ForVariousListSizes(int count)
    {
        // Arrange
        var historyRecords = Enumerable.Range(1, count)
            .Select(i => new PromptHistoryResponse(
                Guid.NewGuid(),
                $"Prompt {i}",
                $"{i % 6}.0",
                DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, count);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithDifferentVersions()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt for v1", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Prompt for v2", "2.0", DateTime.UtcNow.AddMinutes(-1)),
            new(Guid.NewGuid(), "Prompt for v5.2", "5.2", DateTime.UtcNow.AddMinutes(-2)),
            new(Guid.NewGuid(), "Prompt for v6", "6.0", DateTime.UtcNow.AddMinutes(-3))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 4);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<PromptHistoryResponse>, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAll_DoesNotRequireParameters()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act - Only requires CancellationToken
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAll_UsesSingletonPattern_VerifiesSameInstance()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Test", "1.0", DateTime.UtcNow)
        };
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        var capturedQueries = new List<GetAllHistoryRecords.Query>();

        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<PromptHistoryResponse>>>, CancellationToken>((query, ct) =>
            {
                if (query is GetAllHistoryRecords.Query q)
                    capturedQueries.Add(q);
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetAll(CancellationToken.None);
        await controller.GetAll(CancellationToken.None);
        await controller.GetAll(CancellationToken.None);

        // Assert
        capturedQueries.Should().HaveCount(3);
        capturedQueries.Should().AllSatisfy(q =>
            q.Should().BeSameAs(GetAllHistoryRecords.Query.Singletone));
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithChronologicallyOrderedRecords()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Most recent", "1.0", now),
            new(Guid.NewGuid(), "Second", "1.0", now.AddMinutes(-5)),
            new(Guid.NewGuid(), "Third", "1.0", now.AddMinutes(-10)),
            new(Guid.NewGuid(), "Oldest", "1.0", now.AddMinutes(-15))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 4);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithLongPromptTexts()
    {
        // Arrange
        var longPrompt = new string('a', 2000);
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), longPrompt, "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Short prompt", "1.0", DateTime.UtcNow.AddMinutes(-1))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithSpecialCharactersInPrompts()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), "Prompt with @#$% special chars", "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), "Unicode: ??? ?? ??", "1.0", DateTime.UtcNow.AddMinutes(-1))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithDuplicatePromptTexts()
    {
        // Arrange
        var samePrompt = "Identical prompt text";
        var historyRecords = new List<PromptHistoryResponse>
        {
            new(Guid.NewGuid(), samePrompt, "1.0", DateTime.UtcNow),
            new(Guid.NewGuid(), samePrompt, "2.0", DateTime.UtcNow.AddMinutes(-1)),
            new(Guid.NewGuid(), samePrompt, "5.2", DateTime.UtcNow.AddMinutes(-2))
        };

        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 3);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithLargeDataset()
    {
        // Arrange
        var largeList = Enumerable.Range(1, 1000)
            .Select(i => new PromptHistoryResponse(
                Guid.NewGuid(),
                $"Prompt number {i}",
                $"{i % 6}.0",
                DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        var result = Result.Ok(largeList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<PromptHistoryResponse>(actionResult, 1000);
    }

    [Fact]
    public async Task GetAll_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var historyRecords = new List<PromptHistoryResponse>();
        var result = Result.Ok(historyRecords);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllHistoryRecords.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetAll(CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}