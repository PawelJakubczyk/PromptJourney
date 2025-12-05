using Application.UseCases.Styles.Commands;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class UpdateDescriptionTests : StylesControllerTestsBase
{
    [Fact]
    public async Task UpdateDescription_ReturnsOk_WhenDescriptionUpdatedSuccessfully()
    {
        // Arrange
        var styleName = "TestStyle";
        var updatedDescription = "Updated description";
        var request = new UpdateDescriptionRequest(updatedDescription);
        var result = Result.Ok(request.Description);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(updatedDescription);
    }

    [Fact]
    public async Task UpdateDescription_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style not found");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeNotFoundResult()
            .WithMessage("Style not found");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var invalidStyleName = "";
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(invalidStyleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be empty");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullStyleName = null;
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(nullStyleName!, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be null");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceStyleName = "   ";
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(whitespaceStyleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be whitespace");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongStyleName = new string('a', 256);
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(tooLongStyleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name exceeds maximum length");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsOk_WhenDescriptionSetToNull()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest(null!);
        var result = Result.Ok<string>(null!);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithNullValue();
    }

    [Fact]
    public async Task UpdateDescription_ReturnsOk_WhenDescriptionSetToEmpty()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("");
        var result = Result.Ok("");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsOk_WithLongDescription()
    {
        // Arrange
        var styleName = "TestStyle";
        var longDescription = new string('A', 1000) + " This is a very long description for testing purposes.";
        var request = new UpdateDescriptionRequest(longDescription);
        var result = Result.Ok(longDescription);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(longDescription);
    }

    [Fact]
    public async Task UpdateDescription_ReturnsOk_WithSpecialCharacters()
    {
        // Arrange
        var styleName = "TestStyle";
        var newDescription = "New description with special characters: !@#$%^&*()_+[]{}|;:',.<>?/~`";
        var request = new UpdateDescriptionRequest(newDescription);
        var result = Result.Ok(newDescription);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(newDescription);
    }

    [Fact]
    public async Task UpdateDescription_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var description = "New description";
        var request = new UpdateDescriptionRequest(description);
        var result = Result.Ok(description);
        var senderMock = new Mock<ISender>();
        UpdateDescriptionInStyle.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateDescriptionInStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((command, ct) =>
            {
                capturedCommand = command as UpdateDescriptionInStyle.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.StyleName.Should().Be(styleName);
        capturedCommand.NewDescription.Should().Be(description);
    }

    [Fact]
    public async Task UpdateDescription_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("New description");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock.SetupSendThrowsOperationCanceledForAny<string>();

        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.UpdateDescription(styleName, request, cts.Token);

        // Assert
        await action
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task UpdateDescription_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("New description");
        var result = Result.Ok(request.Description);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        senderMock.Verify(
            sender => sender.Send(It.IsAny<UpdateDescriptionInStyle.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernStyle", "This is a modern style description")]
    [InlineData("ClassicArt", "Classic art with timeless beauty")]
    [InlineData("MinimalistDesign", "Less is more in design")]
    [InlineData("Contemporary", "Contemporary artistic expression")]
    [InlineData("Traditional", "Traditional craftsmanship")]
    public async Task UpdateDescription_ReturnsOk_ForVariousStyleNameAndDescriptionCombinations(string styleName, string description)
    {
        // Arrange
        var request = new UpdateDescriptionRequest(description);
        var result = Result.Ok(description);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(description);
    }

    [Fact]
    public async Task UpdateDescription_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("Consistent description");
        var result = Result.Ok(request.Description);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.UpdateDescription(styleName, request, CancellationToken.None);
        var actionResult2 = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult1
            .Should()
            .BeOkResult()
            .WithValue("Consistent description");
        
        actionResult2
            .Should()
            .BeOkResult()
            .WithValue("Consistent description");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during description update");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Repository error during description update");
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Command handler failed");
    }

    [Fact]
    public async Task UpdateDescription_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("Performance test description");
        var result = Result.Ok(request.Description);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration
            .Should()
            .BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateDescription_ReturnsOk_WithMultipleSequentialUpdates()
    {
        // Arrange
        var styleName = "TestStyle";
        var lastDescription = "Final description";
        var descriptions = new[] { "Orginal Description", "Second Description", "Third Description", lastDescription };
        var senderMock = new Mock<ISender>();
        var controller = CreateController(senderMock);

        foreach (var description in descriptions)
        {
            var request = new UpdateDescriptionRequest(description);
            var result = Result.Ok(description);
            senderMock.SetupSendReturnsForRequest<UpdateDescriptionInStyle.Command, string>(result);

            // Act
            var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

            // Assert
            actionResult
                .Should()
                .BeOkResult()
                .WithValue(description);
        }
    }
}