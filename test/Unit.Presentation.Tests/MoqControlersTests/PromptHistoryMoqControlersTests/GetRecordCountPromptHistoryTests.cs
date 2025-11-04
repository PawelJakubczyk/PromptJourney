using Application.UseCases.PromptHistory.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PromptHistoryMoqControlersTests;

public sealed class GetRecordCountPromptHistoryTests : PromptHistoryControllerTestsBase
{
    [Fact]
    public async Task GetRecordCount_ReturnsOkWithCount_WhenRecordsExist()
    {
        // Arrange
        var count = 42;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<int>(actionResult);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsOkWithZero_WhenNoRecordsExist()
    {
        // Arrange
        var count = 0;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<int>(actionResult);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<int, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsOk_WithLargeNumbers()
    {
        // Arrange
        var count = 1000000; // Large number
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<int>(actionResult);
    }

    [Fact]
    public async Task GetRecordCount_UsesSingletonQuery()
    {
        // Arrange
        var count = 5;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        CalculateHistoricalRecordCount.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<int>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CalculateHistoricalRecordCount.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetRecordCount(CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Same(CalculateHistoricalRecordCount.Query.Singletone, capturedQuery);
    }

    [Fact]
    public async Task GetRecordCount_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetRecordCount(cts.Token));
    }

    [Fact]
    public async Task GetRecordCount_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var count = 10;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetRecordCount(CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public async Task GetRecordCount_ReturnsOk_ForVariousCounts(int count)
    {
        // Arrange
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<int>(actionResult);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var count = 42;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetRecordCount(CancellationToken.None);
        var actionResult2 = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<int>(actionResult1);
        AssertOkResult<int>(actionResult2);
    }

    [Fact]
    public async Task GetRecordCount_DoesNotRequireParameters()
    {
        // Arrange
        var count = 5;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act - Only requires CancellationToken
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<int>(actionResult);
    }

    [Fact]
    public async Task GetRecordCount_UsesSingletonPattern_VerifiesSameInstance()
    {
        // Arrange
        var count = 10;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        var capturedQueries = new List<CalculateHistoricalRecordCount.Query>();

        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<int>>, CancellationToken>((query, ct) =>
            {
                if (query is CalculateHistoricalRecordCount.Query q)
                    capturedQueries.Add(q);
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetRecordCount(CancellationToken.None);
        await controller.GetRecordCount(CancellationToken.None);
        await controller.GetRecordCount(CancellationToken.None);

        // Assert
        capturedQueries.Should().HaveCount(3);
        capturedQueries.Should().AllSatisfy(q =>
            q.Should().BeSameAs(CalculateHistoricalRecordCount.Query.Singletone));
    }

    [Fact]
    public async Task GetRecordCount_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<int, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<int, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsOk_WithMaxIntValue()
    {
        // Arrange
        var count = int.MaxValue;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<int>(actionResult);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsOk_WhenCountChangesOverTime()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        senderMock
            .SetupSequence(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(5))
            .ReturnsAsync(Result.Ok(10))
            .ReturnsAsync(Result.Ok(15));

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetRecordCount(CancellationToken.None);
        var actionResult2 = await controller.GetRecordCount(CancellationToken.None);
        var actionResult3 = await controller.GetRecordCount(CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        actionResult3.Should().NotBeNull();
        AssertOkResult<int>(actionResult1);
        AssertOkResult<int>(actionResult2);
        AssertOkResult<int>(actionResult3);
    }

    [Fact]
    public async Task GetRecordCount_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var count = 1000;
        var result = Result.Ok(count);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CalculateHistoricalRecordCount.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetRecordCount(CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}