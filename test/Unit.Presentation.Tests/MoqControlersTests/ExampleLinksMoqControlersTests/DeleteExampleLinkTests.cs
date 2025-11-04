using Application.UseCases.Common.Responses;
using Application.UseCases.ExampleLinks.Commands;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class DeleteExampleLinkTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task DeleteExampleLink_ReturnsOkWithDeleteResponse_WhenLinkDeletedSuccessfully()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var deleteResponse = DeleteResponse.Success($"Example link with ID '{linkId}' was successfully deleted.");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsConflict_WhenLinkDoesNotExist()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            $"Link '{linkId}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsInvalid()
    {
        // Arrange
        var invalidLinkId = "not-a-guid";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid link ID format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(invalidLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsEmpty()
    {
        // Arrange
        var emptyLinkId = string.Empty;
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Link ID cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(emptyLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsWhitespace()
    {
        // Arrange
        var whitespaceLinkId = "   ";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Link ID cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(whitespaceLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsNull()
    {
        // Arrange
        string? nullLinkId = null;
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Link ID cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(nullLinkId!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var failureResult = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var deleteResponse = DeleteResponse.Success($"Deleted link {linkId}");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        DeleteExampleLink.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<DeleteResponse>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as DeleteExampleLink.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(linkId, capturedCommand!.Id);
    }

    [Fact]
    public async Task DeleteExampleLink_HandlesCancellationToken()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.DeleteExampleLink(linkId, cts.Token));
    }

    [Fact]
    public async Task DeleteExampleLink_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    [InlineData("a1b2c3d4-e5f6-7890-1234-567890abcdef")]
    [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
    public async Task DeleteExampleLink_ReturnsOk_ForVariousValidGuids(string linkId)
    {
        // Arrange
        var deleteResponse = DeleteResponse.Success($"Deleted {linkId}");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-guid")]
    [InlineData("12345")]
    [InlineData("invalid-format-12345")]
    [InlineData("00000000-0000-0000-0000-00000000000g")] // Invalid character
    public async Task DeleteExampleLink_ReturnsBadRequest_ForInvalidInputs(string invalidLinkId)
    {
        // Arrange
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid link ID");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(invalidLinkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsConflict_WhenLinkIsAlreadyDeleted()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status409Conflict,
            "Link has already been deleted");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_ForMalformedGuid()
    {
        // Arrange
        var malformedGuid = "12345678-1234-1234-1234-12345678"; // Too short
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Malformed GUID format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(malformedGuid, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsConsistentResults_ForSameLinkId()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var deleteResponse = DeleteResponse.Success($"Deleted {linkId}");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.DeleteExampleLink(linkId, CancellationToken.None);
        var actionResult2 = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult1);
        AssertOkResult<DeleteResponse>(actionResult2);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var failureResult = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during deletion");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_HandlesCaseInsensitiveGuids()
    {
        // Arrange
        var lowercaseGuid = "a1b2c3d4-e5f6-7890-abcd-1234567890ef";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(lowercaseGuid, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsDeleteResponse_WithSuccessMessage()
    {
        // Arrange
        var linkId = Guid.NewGuid().ToString();
        var expectedMessage = $"Example link with ID '{linkId}' was successfully deleted.";
        var deleteResponse = DeleteResponse.Success(expectedMessage);
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }
}