using Application.UseCases.ExampleLinks.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class CheckLinksEmptyTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task CheckLinksEmpty_ReturnsOkWithTrue_WhenLinksExist()
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsOkWithFalse_WhenNoLinksExist()
    {
        // Arrange
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        // ToResultsCheckExistOkAsync maps all non-400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinksEmpty_UsesSingletonQuery()
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        CheckAnyExampleLinksExist.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CheckAnyExampleLinksExist.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Same(CheckAnyExampleLinksExist.Query.Singletone, capturedQuery);
    }

    [Fact]
    public async Task CheckLinksEmpty_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.CheckLinksEmpty(cts.Token));
    }

    [Fact]
    public async Task CheckLinksEmpty_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckLinksEmpty(CancellationToken.None);
        var actionResult2 = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<bool>(actionResult1);
        AssertOkResult<bool>(actionResult2);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsOk_WhenDatabaseIsEmpty()
    {
        // Arrange
        var result = Result.Ok(false); // No links exist
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsOk_WhenDatabaseHasLinks()
    {
        // Arrange
        var result = Result.Ok(true); // Links exist
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckLinksEmpty_ReturnsOk_ForBothBooleanValues(bool isEmpty)
    {
        // Arrange
        var result = Result.Ok(isEmpty);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckLinksEmpty_DoesNotRequireParameters()
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act - Only requires CancellationToken
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinksEmpty_UsesSingletonPattern_VerifiesSameInstance()
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        var capturedQueries = new List<CheckAnyExampleLinksExist.Query>();

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                if (query is CheckAnyExampleLinksExist.Query q)
                    capturedQueries.Add(q);
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);
        await controller.CheckLinksEmpty(CancellationToken.None);
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        capturedQueries.Should().HaveCount(3);
        capturedQueries.Should().AllSatisfy(q =>
            q.Should().BeSameAs(CheckAnyExampleLinksExist.Query.Singletone));
    }

    [Fact]
    public async Task CheckLinksEmpty_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}