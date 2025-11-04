using Application.UseCases.Common.Responses;
using Application.UseCases.Styles.Commands;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class DeleteStyleTests : StylesControllerTestsBase
{
    [Fact]
    public async Task Delete_ReturnsOkWithDeleteResponse_WhenStyleDeletedSuccessfully()
    {
        // Arrange
        var styleName = "StyleToDelete";
        var deleteResponse = DeleteResponse.Success($"Style '{styleName}' was successfully deleted.");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{styleName}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var emptyName = string.Empty;
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(emptyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceName = "   ";
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(whitespaceName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullName = null;
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(nullName!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongName = new string('a', 256);
        var failureResult = CreateFailureResult<DeleteResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(tooLongName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "TestStyle";
        var failureResult = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Delete_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "CustomStyle";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        DeleteStyle.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<DeleteResponse>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as DeleteStyle.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Delete(styleName, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(styleName, capturedCommand!.StyleName);
    }

    [Fact]
    public async Task Delete_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "TestStyle";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.Delete(styleName, cts.Token));
    }

    [Fact]
    public async Task Delete_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "TestStyle";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Delete(styleName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("AbstractPainting")]
    [InlineData("MinimalDesign")]
    public async Task Delete_ReturnsOk_ForVariousValidStyleNames(string styleName)
    {
        // Arrange
        var deleteResponse = DeleteResponse.Success($"Style '{styleName}' deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var styleName = "TestStyle";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.Delete(styleName, CancellationToken.None);
        var actionResult2 = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult1);
        AssertOkResult<DeleteResponse>(actionResult2);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WithLowercaseStyleName()
    {
        // Arrange
        var styleName = "modernart";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WithUppercaseStyleName()
    {
        // Arrange
        var styleName = "MODERNART";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WithMixedCaseStyleName()
    {
        // Arrange
        var styleName = "ModernArt";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WithStyleNameContainingNumbers()
    {
        // Arrange
        var styleName = "Style123";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WithStyleNameContainingHyphen()
    {
        // Arrange
        var styleName = "modern-art";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WithStyleNameContainingUnderscore()
    {
        // Arrange
        var styleName = "modern_art";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WithStyleNameContainingSpaces()
    {
        // Arrange
        var styleName = "Modern Art";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "TestStyle";
        var failureResult = CreateFailureResult<DeleteResponse, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during deletion");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var styleName = "TestStyle";
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsDeleteResponse_WithSuccessMessage()
    {
        // Arrange
        var styleName = "TestStyle";
        var expectedMessage = $"Style '{styleName}' was successfully deleted.";
        var deleteResponse = DeleteResponse.Success(expectedMessage);
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<DeleteResponse>(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenStyleDeletedPreviously()
    {
        // Arrange
        var styleName = "DeletedStyle";
        var failureResult = CreateFailureResult<DeleteResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{styleName}' not found - may have been deleted already");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Delete_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var deleteResponse = DeleteResponse.Success("Deleted");
        var result = Result.Ok(deleteResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.Delete(styleName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}