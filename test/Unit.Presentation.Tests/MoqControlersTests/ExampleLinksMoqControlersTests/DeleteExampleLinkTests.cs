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
        var deleteResponse = DeleteResponse.Success($"Example link with ID '{CorrectId}' was successfully deleted.");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(Result.Ok(deleteResponse));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<DeleteResponse>();
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsNotFound_WhenLinkDoesNotExist()
    {
        // Arrange
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound, 
            $"Link '{CorrectId}' not found");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsInvalid()
    {
        // Arrange
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Invalid link ID format");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink("not-a-guid", CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsEmpty()
    {
        // Arrange
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Link ID cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(string.Empty, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsWhitespace()
    {
        // Arrange
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Link ID cannot be whitespace");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink("   ", CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkIdIsNull()
    {
        // Arrange
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Link ID cannot be null");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(null!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError, 
            "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var deleteResponse = DeleteResponse.Success($"Deleted link {CorrectId}");
        var senderMock = CreateSenderMock();
        DeleteExampleLink.Command? captured = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteExampleLink.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<DeleteResponse>>, CancellationToken>((cmd, ct) => { captured = cmd as DeleteExampleLink.Command; })
            .ReturnsAsync(Result.Ok(deleteResponse));
        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Id.Should().Be(CorrectId);
    }

    [Fact]
    public async Task DeleteExampleLink_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<DeleteResponse>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.DeleteExampleLink(CorrectId, cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Fact]
    public async Task DeleteExampleLink_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(
            Result.Ok(DeleteResponse.Success("Deleted")));
        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

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
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(
            Result.Ok(DeleteResponse.Success($"Deleted {linkId}")));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(linkId, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<DeleteResponse>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-guid")]
    [InlineData("12345")]
    [InlineData("invalid-format-12345")]
    [InlineData("00000000-0000-0000-0000-00000000000g")]
    public async Task DeleteExampleLink_ReturnsBadRequest_ForInvalidInputs(string invalidLinkId)
    {
        // Arrange
        var failure = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Invalid link ID");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(invalidLinkId, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_ForMalformedGuidWithHyphens()
    {
        // Arrange
        var malformedGuid = "12345678-1234-1234-1234-12345678";
        var failure = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest, 
            "Malformed GUID format");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(malformedGuid, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsConsistentResults_ForSameLinkId()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        var response = DeleteResponse.Success($"Deleted {CorrectId}");
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(Result.Ok(response));
        var controller = CreateController(senderMock);

        // Act
        var r1 = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);
        var r2 = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        r1.Should().BeOkResult().WithValueOfType<DeleteResponse>();
        r2.Should().BeOkResult().WithValueOfType<DeleteResponse>();
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failure = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest, 
            "Repository error during deletion");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_HandlesCaseInsensitiveGuids()
    {
        // Arrange
        var lowercaseGuid = "a1b2c3d4-e5f6-7890-abcd-1234567890ef";
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(
            Result.Ok(DeleteResponse.Success("Deleted")));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(lowercaseGuid, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<DeleteResponse>();
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var failure = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest, 
            "Command handler failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteExampleLink_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(
            Result.Ok(DeleteResponse.Success("Deleted")));
        var controller = CreateController(senderMock);
        var start = DateTime.UtcNow;

        // Act
        await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        (DateTime.UtcNow - start).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsDeleteResponse_WithSuccessMessage()
    {
        // Arrange
        var expectedMessage = $"Example link with ID '{CorrectId}' was successfully deleted.";
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteExampleLink.Command, DeleteResponse>(
            Result.Ok(DeleteResponse.Success(expectedMessage)));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(CorrectId, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<DeleteResponse>();
    }
}