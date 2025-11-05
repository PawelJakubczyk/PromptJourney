using Application.UseCases.Styles.Commands;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class AddTagTests : StylesControllerTestsBase
{
    [Fact]
    public async Task AddTag_ReturnsOkWithTagName_WhenTagAddedSuccessfully()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            $"Style '{styleName}' not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsEmpty()
    {
        // Arrange
        var styleName = "TestStyle";
        var emptyTag = string.Empty;
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, emptyTag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagAlreadyExists()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "existingtag";
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            $"Tag '{tag}' already exists in style '{styleName}'");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var emptyStyleName = string.Empty;
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(emptyStyleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsWhitespace()
    {
        // Arrange
        var styleName = "TestStyle";
        var whitespaceTag = "   ";
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, whitespaceTag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceStyleName = "   ";
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(whitespaceStyleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsNull()
    {
        // Arrange
        var styleName = "TestStyle";
        string? nullTag = null;
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, nullTag!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullStyleName = null;
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(nullStyleName!, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagExceedsMaxLength()
    {
        // Arrange
        var styleName = "TestStyle";
        var tooLongTag = new string('a', 256);
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tooLongTag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongStyleName = new string('a', 256);
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(tooLongStyleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "CustomStyle";
        var tag = "modern";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        AddTagToStyle.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((cmd, ct) =>
            {
                capturedCommand = cmd as AddTagToStyle.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.StyleName.Should().Be(styleName);
        capturedCommand.Tag.Should().Be(tag);
    }

    [Fact]
    public async Task AddTag_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.AddTag(styleName, tag, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task AddTag_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("TestStyle", "modern")]
    [InlineData("CustomStyle", "abstract")]
    [InlineData("MyStyle", "vintage")]
    [InlineData("ArtStyle", "minimalist")]
    public async Task AddTag_ReturnsOk_ForVariousValidInputs(string styleName, string tag)
    {
        // Arrange
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.AddTag(styleName, tag, CancellationToken.None);
        var actionResult2 = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithValueOfType<string>();
        actionResult2.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithLowercaseTag()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "lowercasetag";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithUppercaseTag()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "UPPERCASETAG";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithMixedCaseTag()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "MixedCaseTag";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingNumbers()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "tag123";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingHyphen()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "tag-with-hyphen";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingUnderscore()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "tag_with_underscore";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during tag addition");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "newtag";
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}