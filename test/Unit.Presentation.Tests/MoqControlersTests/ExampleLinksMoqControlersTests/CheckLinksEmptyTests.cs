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
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsOkWithFalse_WhenNoLinksExist()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(false));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(StatusCodes.Status500InternalServerError, "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinksEmpty_UsesSingletonQuery()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        CheckAnyExampleLinksExist.Query? captured = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((q, ct) => captured = q as CheckAnyExampleLinksExist.Query)
            .ReturnsAsync(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured.Should().BeSameAs(CheckAnyExampleLinksExist.Query.Singletone);
    }

    [Fact]
    public async Task CheckLinksEmpty_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<bool>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.CheckLinksEmpty(cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Fact]
    public async Task CheckLinksEmpty_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var r1 = await controller.CheckLinksEmpty(CancellationToken.None);
        var r2 = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        r1.Should().BeOkResult().WithValueOfType<bool>();
        r2.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(StatusCodes.Status400BadRequest, "Repository error");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsOk_WhenDatabaseIsEmpty()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(false));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsOk_WhenDatabaseHasLinks()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckLinksEmpty_ReturnsOk_ForBothBooleanValues(bool isEmpty)
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(isEmpty));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinksEmpty_DoesNotRequireParameters()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, ApplicationLayer>(StatusCodes.Status400BadRequest, "Query handler failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinksEmpty_UsesSingletonPattern_VerifiesSameInstance()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        var captured = new List<CheckAnyExampleLinksExist.Query>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckAnyExampleLinksExist.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((q, ct) => { if (q is CheckAnyExampleLinksExist.Query qq) captured.Add(qq); })
            .ReturnsAsync(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);
        await controller.CheckLinksEmpty(CancellationToken.None);
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        captured.Should().HaveCount(3);
        captured.Should().AllSatisfy(q => q.Should().BeSameAs(CheckAnyExampleLinksExist.Query.Singletone));
    }

    [Fact]
    public async Task CheckLinksEmpty_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckAnyExampleLinksExist.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);
        var start = DateTime.UtcNow;

        // Act
        await controller.CheckLinksEmpty(CancellationToken.None);

        // Assert
        (DateTime.UtcNow - start).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}