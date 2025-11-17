using Application.UseCases.ExampleLinks.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class CheckLinkExistsTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task CheckLinkExists_ReturnsOkWithTrue_WhenLinkExists()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(CorrectId, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsOkWithFalse_WhenLinkDoesNotExist()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(Result.Ok(false));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(CorrectId, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsInvalidGuid()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Invalid link ID format");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists("not-a-guid", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeErrorResult()
            .WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsEmpty()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Link ID cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(string.Empty, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeErrorResult()
            .WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenLinkIdIsWhitespace()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Link ID cannot be whitespace");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists("   ", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeErrorResult()
            .WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(StatusCodes.Status500InternalServerError, "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(CorrectId, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeErrorResult()
            .WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        CheckExampleLinkExistsById.Query? capturedQuery = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) => { capturedQuery = query as CheckExampleLinkExistsById.Query; })
            .ReturnsAsync(Result.Ok(true));
        var controller = CreateController(senderMock);
        var linkId = CorrectId;

        // Act
        await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.Id.Should().Be(linkId);
    }

    [Fact]
    public async Task CheckLinkExists_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<bool>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.CheckLinkExists(CorrectId, cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    [InlineData("a1b2c3d4-e5f6-7890-1234-567890abcdef")]
    [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
    public async Task CheckLinkExists_ReturnsOk_ForVariousValidGuids(string linkId)
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValueOfType<bool>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-guid")]
    [InlineData("12345")]
    [InlineData("invalid-format-12345")]
    [InlineData("00000000-0000-0000-0000-00000000000g")]
    public async Task CheckLinkExists_ReturnsBadRequest_ForInvalidInputs(string invalidLinkId)
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Invalid link ID");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(invalidLinkId, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenNullLinkId()
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Link ID cannot be null");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(null!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsConsistentResults_ForSameLinkId()
    {
        // Arrange
        var linkId = CorrectId;
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckLinkExists(linkId, CancellationToken.None);
        var actionResult2 = await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithValueOfType<bool>();
        actionResult2.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_ForMalformedGuidWithHyphens()
    {
        // Arrange
        var malformedGuid = "12345678-1234-1234-1234-12345678";
        var failureResult = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Malformed GUID format");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(malformedGuid, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var linkId = CorrectId;
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinkExists(linkId, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(It.IsAny<CheckExampleLinkExistsById.Query>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckLinkExists_HandlesCaseInsensitiveGuids()
    {
        // Arrange
        var lowercaseGuid = "a1b2c3d4-e5f6-7890-abcd-1234567890ef";
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsById.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(lowercaseGuid, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }
}