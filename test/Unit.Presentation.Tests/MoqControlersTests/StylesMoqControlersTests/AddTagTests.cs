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
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        var failureResult = CreateFailureResult<string, ApplicationLayer>(StatusCodes.Status404NotFound, "Style 'NonExistentStyle' not found");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("NonExistentStyle", "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsEmpty()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Tag cannot be empty");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", string.Empty, CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagAlreadyExists()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Tag 'existingtag' already exists in style 'TestStyle'");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "existingtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Style name cannot be empty");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag(string.Empty, "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsWhitespace()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Tag cannot be whitespace");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "   ", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Style name cannot be whitespace");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("   ", "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagIsNull()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Tag cannot be null");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", null!, CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Style name cannot be null");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag(null!, "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenTagExceedsMaxLength()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Tag exceeds maximum length");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", new string('a', 256), CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        var failureResult = CreateFailureResult<string, DomainLayer>(StatusCodes.Status400BadRequest, "Style name exceeds maximum length");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag(new string('a', 256), "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        var failureResult = CreateFailureResult<string, PersistenceLayer>(StatusCodes.Status500InternalServerError, "Database connection failed");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_VerifiesCommandIsCalledWithCorrectParameters()
    {
        var result = Result.Ok("modern");
        var senderMock = new Mock<ISender>();
        AddTagToStyle.Command? capturedCommand = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((cmd, ct) => { capturedCommand = cmd as AddTagToStyle.Command; })
            .ReturnsAsync(result);
        var controller = CreateController(senderMock);
        await controller.AddTag("CustomStyle", "modern", CancellationToken.None);
        capturedCommand.Should().NotBeNull();
        capturedCommand!.StyleName.Should().Be("CustomStyle");
        capturedCommand.Tag.Should().Be("modern");
    }

    [Fact]
    public async Task AddTag_HandlesCancellationToken()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendThrowsOperationCanceledForAny<string>();
        var controller = CreateController(senderMock);
        await FluentActions.Awaiting(() => controller.AddTag("TestStyle", "newtag", cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task AddTag_VerifiesSenderIsCalledOnce()
    {
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        senderMock.Verify(s => s.Send(It.IsAny<AddTagToStyle.Command>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("TestStyle", "modern")]
    [InlineData("CustomStyle", "abstract")]
    [InlineData("MyStyle", "vintage")]
    [InlineData("ArtStyle", "minimalist")]
    public async Task AddTag_ReturnsOk_ForVariousValidInputs(string styleName, string tag)
    {
        var result = Result.Ok(tag);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag(styleName, tag, CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsConsistentResults_ForSameParameters()
    {
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult1 = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        var actionResult2 = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        actionResult1.Should().BeOkResult().WithValueOfType<string>();
        actionResult2.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithLowercaseTag()
    {
        var result = Result.Ok("lowercasetag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "lowercasetag", CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithUppercaseTag()
    {
        var result = Result.Ok("UPPERCASETAG");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "UPPERCASETAG", CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithMixedCaseTag()
    {
        var result = Result.Ok("MixedCaseTag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "MixedCaseTag", CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingNumbers()
    {
        var result = Result.Ok("tag123");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "tag123", CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingHyphen()
    {
        var result = Result.Ok("tag-with-hyphen");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "tag-with-hyphen", CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsOk_WithTagContainingUnderscore()
    {
        var result = Result.Ok("tag_with_underscore");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "tag_with_underscore", CancellationToken.None);
        actionResult.Should().BeOkResult().WithValueOfType<string>();
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        var failureResult = CreateFailureResult<string, PersistenceLayer>(StatusCodes.Status400BadRequest, "Repository error during tag addition");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        var failureResult = CreateFailureResult<string, ApplicationLayer>(StatusCodes.Status400BadRequest, "Command handler failed");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddTag_RespondsQuickly_ForPerformanceTest()
    {
        var result = Result.Ok("newtag");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<AddTagToStyle.Command, string>(result);
        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;
        await controller.AddTag("TestStyle", "newtag", CancellationToken.None);
        (DateTime.UtcNow - startTime).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}