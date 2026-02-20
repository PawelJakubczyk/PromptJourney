using Application.UseCases.Common.Responses;
using Application.UseCases.ExampleLinks.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;
using Utilities.Results;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class DeleteAllByStyleTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task DeleteAllByStyle_ReturnsOkWithBulkDeleteResponse_WhenLinksDeletedSuccessfully()
    {
        // Arrange
        var deletedCount = 5;
        var bulkDeleteResponse = BulkDeleteResponse.Success(
            deletedCount,
            $"Successfully deleted {deletedCount} example links for style '{CorrectStyleName}'.");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(Result.Ok(bulkDeleteResponse));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(bulkDeleteResponse);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsOkWithZeroCount_WhenNoLinksToDelete()
    {
        // Arrange
        var deletedCount = 0;
        var bulkDeleteResponse = BulkDeleteResponse.Success(
            deletedCount,
            $"Successfully deleted {deletedCount} example links for style 'EmptyStyle'.");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(Result.Ok(bulkDeleteResponse));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle("EmptyStyle", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(bulkDeleteResponse);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{NonExistStyleName}' not found");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(NonExistStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeNotFoundResult()
            .WithMessage($"Style '{NonExistStyleName}' not found");
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(string.Empty, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be empty");
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle("   ", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be whitespace");
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            ErrorMessageStyleNameTooLong);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(IncorrectStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage(ErrorMessageStyleNameTooLong);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(null!, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be null");
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Database connection failed");
    }

    [Fact]
    public async Task DeleteAllByStyle_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        DeleteAllExampleLinksByStyle.Command? capturedCommand = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<BulkDeleteResponse>>, CancellationToken>((cmd, ct) => { capturedCommand = cmd as DeleteAllExampleLinksByStyle.Command; })
            .ReturnsAsync(Result.Ok(BulkDeleteResponse.Success(3, "Deleted 3 links")));
        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.StyleName.Should().Be(CorrectStyleName);
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<BulkDeleteResponse>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.DeleteAllByStyle(CorrectStyleName, cts.Token);

        // Assert
        await action
            .Should()
            .ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Fact]
    public async Task DeleteAllByStyle_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(
            Result.Ok(BulkDeleteResponse.Success(5, "Deleted 5 links")));
        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt", 10)]
    [InlineData("ClassicStyle", 5)]
    [InlineData("AbstractArt", 1)]
    [InlineData("MinimalStyle", 0)]
    public async Task DeleteAllByStyle_ReturnsOk_ForVariousDeletionCounts(string styleName, int deletedCount)
    {
        // Arrange
        var bulkDeleteResponse = BulkDeleteResponse.Success(deletedCount, $"Deleted {deletedCount} links");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(Result.Ok(bulkDeleteResponse));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(bulkDeleteResponse);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task DeleteAllByStyle_ReturnsBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid style name");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(invalidStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Invalid style name");
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsOk_WhenDeletingMultipleLinks()
    {
        // Arrange
        var response = BulkDeleteResponse.Success(100, $"Successfully deleted 100 example links for style 'PopularStyle'.");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(Result.Ok(response));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle("PopularStyle", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(response);
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesStyleNameWithSpecialCharacters()
    {
        // Arrange
        var styleName = "Modern-Art_2024";
        var bulkDeleteResponse = BulkDeleteResponse.Success(3, $"Successfully deleted 3 example links for style '{styleName}'.");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(
            Result.Ok(bulkDeleteResponse));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(bulkDeleteResponse);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during deletion");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Repository error during deletion");
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        var bulkDeleteResponse = BulkDeleteResponse.Success(5, "Deleted 5 links");
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(Result.Ok(bulkDeleteResponse));
        var controller = CreateController(senderMock);

        // Act
        var r1 = await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);
        var r2 = await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        r1
            .Should()
            .BeOkResult()
            .WithValue(bulkDeleteResponse);

        r2
            .Should()
            .BeOkResult()
            .WithValue(bulkDeleteResponse);
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var styleName = "modernart";
        var bulkDeleteResponse = BulkDeleteResponse.Success(3, $"Successfully deleted 3 example links for style '{styleName}'.");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(
            Result.Ok(bulkDeleteResponse));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(bulkDeleteResponse);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<BulkDeleteResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Command handler failed");
    }

    [Fact]
    public async Task DeleteAllByStyle_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<DeleteAllExampleLinksByStyle.Command, BulkDeleteResponse>(
            Result.Ok(BulkDeleteResponse.Success(5, "Deleted 5 links")));
        var controller = CreateController(senderMock);
        var start = DateTime.UtcNow;

        // Act
        await controller.DeleteAllByStyle(CorrectStyleName, CancellationToken.None);

        // Assert
        (DateTime.UtcNow - start).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}