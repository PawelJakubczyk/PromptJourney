using Application.UseCases.Styles.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Results;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class AddTagTests : StylesControllerTestsBase
{
    [Fact]
    public async Task AddTag_ReturnsOkWithTagName_WhenTagAddedSuccessfully()
    {
        // Arrange
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("newtag");
    }

    [Fact]
    public async Task AddTag_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status404NotFound,
            "Style 'NonExistentStyle' not found");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("NonExistentStyle", "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeNotFoundResult()
            .WithMessage("Style 'NonExistentStyle' not found");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsEmpty()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be empty");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", string.Empty, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Tag cannot be empty");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagAlreadyExists()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Tag 'existingtag' already exists in style 'TestStyle'");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "existingtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Tag 'existingtag' already exists in style 'TestStyle'");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(string.Empty, "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be empty");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsWhitespace()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be whitespace");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "   ", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Tag cannot be whitespace");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("   ", "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be whitespace");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsNull()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be null");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", null!, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Tag cannot be null");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(null!, "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name cannot be null");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagExceedsMaxLength()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Tag exceeds maximum length");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", new string('a', 256), CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Tag exceeds maximum length");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(new string('a', 256), "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Style name exceeds maximum length");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Database connection failed");
    }

    [Fact]
    public async Task AddTag_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var result = Result.Ok("modern");
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
        await controller.AddTag("CustomStyle", "modern", CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.StyleName.Should().Be("CustomStyle");
        capturedCommand.Tag.Should().Be("modern");
    }

    [Fact]
    public async Task AddTag_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendThrowsOperationCanceledForAny<string>();
        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions
            .Awaiting(() => controller.AddTag("TestStyle", "newtag", cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task AddTag_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        await controller.AddTag("TestStyle", "newtag", CancellationToken.None);

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
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(tag);
    }

    [Fact]
    public async Task AddTag_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        var actionResult2 = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);

        // Assert
        actionResult1
            .Should()
            .BeOkResult()
            .WithValue("newtag");
        actionResult2
            .Should()
            .BeOkResult()
            .WithValue("newtag");
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithLowercaseTag()
    {
        // Arrange
        var result = Result.Ok("lowercasetag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "lowercasetag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("lowercasetag");
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithUppercaseTag()
    {
        // Arrange
        var result = Result.Ok("UPPERCASETAG");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "UPPERCASETAG", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("UPPERCASETAG");
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithMixedCaseTag()
    {
        // Arrange
        var result = Result.Ok("MixedCaseTag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "MixedCaseTag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("MixedCaseTag");
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingNumbers()
    {
        // Arrange
        var result = Result.Ok("tag123");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "tag123", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("tag123");
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingHyphen()
    {
        // Arrange
        var result = Result.Ok("tag-with-hyphen");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "tag-with-hyphen", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("tag-with-hyphen");
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingUnderscore()
    {
        // Arrange
        var result = Result.Ok("tag_with_underscore");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "tag_with_underscore", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithValue("tag_with_underscore");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Repository error during tag addition");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Repository error during tag addition");
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<string>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Command handler failed");
    }

    [Fact]
    public async Task AddTag_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.AddTag("TestStyle", "newtag", CancellationToken.None);

        // Assert
        (DateTime.UtcNow - startTime).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}