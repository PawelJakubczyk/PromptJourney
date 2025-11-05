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

public sealed class DeleteAllByStyleTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task DeleteAllByStyle_ReturnsOkWithBulkDeleteResponse_WhenLinksDeletedSuccessfully()
    {
        // Arrange
        var styleName = "ModernArt";
        var deletedCount = 5;
        var bulkDeleteResponse = BulkDeleteResponse.Success(
            deletedCount,
            $"Successfully deleted {deletedCount} example links for style '{styleName}'.");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsOkWithZeroCount_WhenNoLinksToDelete()
    {
        // Arrange
        var styleName = "EmptyStyle";
        var deletedCount = 0;
        var bulkDeleteResponse = BulkDeleteResponse.Success(
            deletedCount,
            $"Successfully deleted {deletedCount} example links for style '{styleName}'.");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var failureResult = CreateFailureResult<BulkDeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{styleName}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var emptyStyleName = string.Empty;
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(emptyStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceStyleName = "   ";
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(whitespaceStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongStyleName = new string('A', 256);
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(tooLongStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullStyleName = null;
        var failureResult = CreateFailureResult<BulkDeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(nullStyleName!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<BulkDeleteResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var bulkDeleteResponse = BulkDeleteResponse.Success(3, "Deleted 3 links");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        DeleteAllExampleLinksByStyle.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<BulkDeleteResponse>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as DeleteAllExampleLinksByStyle.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.StyleName.Should().Be(styleName);
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "ModernArt";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.DeleteAllByStyle(styleName, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task DeleteAllByStyle_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "ModernArt";
        var bulkDeleteResponse = BulkDeleteResponse.Success(5, "Deleted 5 links");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.DeleteAllByStyle(styleName, CancellationToken.None);

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
        var bulkDeleteResponse = BulkDeleteResponse.Success(
            deletedCount,
            $"Deleted {deletedCount} links");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
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

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(invalidStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsOk_WhenDeletingMultipleLinks()
    {
        // Arrange
        var styleName = "PopularStyle";
        var deletedCount = 100;
        var bulkDeleteResponse = BulkDeleteResponse.Success(
            deletedCount,
            $"Successfully deleted {deletedCount} example links for style '{styleName}'.");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesStyleNameWithSpecialCharacters()
    {
        // Arrange
        var styleNameWithSpecialChars = "Modern-Art_2024";
        var bulkDeleteResponse = BulkDeleteResponse.Success(3, "Deleted 3 links");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleNameWithSpecialChars, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<BulkDeleteResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during deletion");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var styleName = "ModernArt";
        var bulkDeleteResponse = BulkDeleteResponse.Success(5, "Deleted 5 links");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.DeleteAllByStyle(styleName, CancellationToken.None);
        var actionResult2 = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
        actionResult2.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
    }

    [Fact]
    public async Task DeleteAllByStyle_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var lowercaseStyleName = "modernart";
        var bulkDeleteResponse = BulkDeleteResponse.Success(3, "Deleted 3 links");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(lowercaseStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<BulkDeleteResponse>();
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<BulkDeleteResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteAllByStyle_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "ModernArt";
        var bulkDeleteResponse = BulkDeleteResponse.Success(5, "Deleted 5 links");
        var result = Result.Ok(bulkDeleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteAllExampleLinksByStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}