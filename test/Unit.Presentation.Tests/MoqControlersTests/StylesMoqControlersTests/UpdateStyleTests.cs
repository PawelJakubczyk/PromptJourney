using Application.UseCases.Styles.Commands;
using Application.UseCases.Styles.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class UpdateStyleTests : StylesControllerTestsBase
{
    [Fact]
    public async Task Update_ReturnsOk_WhenStyleUpdatedSuccessfully()
    {
        // Arrange
        var styleName = "ExistingStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Updated description",
            ["updated", "tags"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleTypeIsEmpty()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            ""
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            null!,
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleTypeIsNull()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "TestStyle",
            null!
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "   ",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleTypeIsWhitespace()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "TestStyle",
            "   "
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongName = new string('a', 256);
        var request = new UpdateStyleRequest(
            tooLongName,
            "Custom"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStyleTypeExceedsMaxLength()
    {
        // Arrange
        var tooLongType = new string('a', 256);
        var request = new UpdateStyleRequest(
            "TestStyle",
            tooLongType
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenTagIsEmpty()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "TestStyle",
            "Custom",
            "Description",
            ["valid", ""]
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenTagIsWhitespace()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "TestStyle",
            "Custom",
            "Description",
            ["valid", "   "]
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithNullDescription()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            null,
            ["tag1"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithNullTags()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Description",
            null
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithEmptyTagsList()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Description",
            []
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithMultipleTags()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Description",
            ["tag1", "tag2", "tag3", "tag4"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsLongDescription()
    {
        // Arrange
        var styleName = "TestStyle";
        var longDescription = new string('A', 1000) + " This is a very long description for testing purposes.";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            longDescription,
            ["tag1"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithSpecialCharactersInDescription()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Description with spéciál characters, émojis 🎨 and symbols @#$%^&*()",
            ["tag1"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Test description",
            ["tag1", "tag2"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        UpdateStyle.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((command, ct) =>
            {
                capturedCommand = command as UpdateStyle.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Update(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(styleName, capturedCommand!.StyleName);
        Assert.Equal(request.Type, capturedCommand.Type);
        Assert.Equal(request.Description, capturedCommand.Description);
        Assert.Equal(request.Tags, capturedCommand.Tags);
    }

    [Fact]
    public async Task Update_HandlesCancellationToken()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "TestStyle",
            "Custom"
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.Update(request, cts.Token));
    }

    [Fact]
    public async Task Update_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom"
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Update(request, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernStyle", "Custom")]
    [InlineData("ClassicArt", "Traditional")]
    [InlineData("MinimalistDesign", "Abstract")]
    [InlineData("Contemporary", "Realistic")]
    [InlineData("VintagePattern", "Minimalist")]
    public async Task Update_ReturnsOk_ForVariousStyleNameAndTypeCombinations(string styleName, string styleType)
    {
        // Arrange
        var request = new UpdateStyleRequest(
            styleName,
            styleType,
            "Test description",
            ["tag1"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Consistent description",
            ["tag1"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.Update(request, CancellationToken.None);
        var actionResult2 = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult1);
        AssertOkResult<string>(actionResult2);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithStyleNameContainingSpecialCharacters()
    {
        // Arrange
        var styleName = "Modern-Art_2024";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Test description",
            ["tag1"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithTagsContainingSpecialCharacters()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Test description",
            ["modern-art", "2024-trend", "style_v2"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "TestStyle",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during style update");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var request = new UpdateStyleRequest(
            "TestStyle",
            "Custom"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithDuplicateTags()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Description",
            ["tag1", "tag1", "tag2"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Performance test description",
            ["tag1"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.Update(request, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Update_ReturnsOk_WithMinimalRequest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom"
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithCompleteRequest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Complete style description with all details",
            ["tag1", "tag2", "tag3", "tag4", "tag5"]
        );

        var result = Result.Ok(styleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithLongStyleName()
    {
        // Arrange
        var longStyleName = new string('a', 100);
        var request = new UpdateStyleRequest(
            longStyleName,
            "Custom",
            "Description",
            ["tag1"]
        );

        var result = Result.Ok(longStyleName);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<UpdateStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult);
    }
}