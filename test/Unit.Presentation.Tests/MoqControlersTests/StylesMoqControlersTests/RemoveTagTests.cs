using Application.UseCases.Styles.Commands;
using Application.UseCases.Styles.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class RemoveTagTests : StylesControllerTestsBase
{
    [Fact]
    public async Task RemoveTag_ReturnsOk_WhenTagRemovedSuccessfully()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "tagtoremove";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var tag = "anytag";
        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task RemoveTag_ReturnsNotFound_WhenTagDoesNotExistInStyle()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "nonexistenttag";
        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Tag not found in style");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var styleName = "";
        var tag = "validtag";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenTagIsEmpty()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenBothParametersAreEmpty()
    {
        // Arrange
        var styleName = "";
        var tag = "";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name and tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? styleName = null;
        var tag = "validtag";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName!, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenTagIsNull()
    {
        // Arrange
        var styleName = "TestStyle";
        string? tag = null;
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var styleName = "   ";
        var tag = "validtag";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenTagIsWhitespace()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "   ";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var styleName = new string('a', 256);
        var tag = "validtag";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenTagExceedsMaxLength()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = new string('a', 256);
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithLastTagRemoved()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "lasttag";
        var response = new StyleResponse(styleName, "Custom", "Description", null);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithMultipleTagsRemaining()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "removedtag";
        var response = new StyleResponse(styleName, "Custom", "Description", ["tag1", "tag2", "tag3"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        DeleteTagFromStyle.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<StyleResponse>>, CancellationToken>((command, ct) =>
            {
                capturedCommand = command as DeleteTagFromStyle.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(styleName, capturedCommand!.StyleName);
        Assert.Equal(tag, capturedCommand.Tag);
    }

    [Fact]
    public async Task RemoveTag_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.RemoveTag(styleName, tag, cts.Token));
    }

    [Fact]
    public async Task RemoveTag_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernStyle", "abstract")]
    [InlineData("ClassicArt", "vintage")]
    [InlineData("MinimalistDesign", "simple")]
    [InlineData("Contemporary", "modern")]
    [InlineData("Traditional", "classic")]
    public async Task RemoveTag_ReturnsOk_ForVariousStyleNameAndTagCombinations(string styleName, string tag)
    {
        // Arrange
        var response = new StyleResponse(styleName, "Custom", "Test description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.RemoveTag(styleName, tag, CancellationToken.None);
        var actionResult2 = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        actionResult1.Should().BeOkResult().WithValueOfType<StyleResponse>();
        actionResult2.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithTagContainingHyphen()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "modern-art";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithTagContainingUnderscore()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "modern_art";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithTagContainingNumbers()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "style2024";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithStyleNameContainingSpecialCharacters()
    {
        // Arrange
        var styleName = "Modern-Art_2024";
        var tag = "testtag";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var failureResult = CreateFailureResult<StyleResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during tag removal");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithEmptyTagsListAfterRemoval()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "lasttag";
        var response = new StyleResponse(styleName, "Custom", "Description", []);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithCaseSensitiveTag()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "ModernArt";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithLongStyleName()
    {
        // Arrange
        var styleName = new string('a', 100);
        var tag = "testtag";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task RemoveTag_ReturnsOk_WithLongTag()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = new string('a', 100);
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<DeleteTagFromStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }
}