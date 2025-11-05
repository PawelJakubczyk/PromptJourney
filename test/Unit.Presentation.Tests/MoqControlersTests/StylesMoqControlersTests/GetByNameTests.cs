using Application.UseCases.Styles.Queries;
using Application.UseCases.Styles.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class GetByNameTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetByName_ReturnsOk_WhenStyleExists()
    {
        // Arrange
        var styleName = "ModernArt";
        var styleResponse = new StyleResponse(styleName, "Custom", "Modern art style", ["abstract", "contemporary"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByName_ReturnsBadRequest_WhenNameInvalid()
    {
        // Arrange
        var invalidName = "";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(invalidName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByName_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string? nullName = null;
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(nullName!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByName_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        // Arrange
        var whitespaceName = "   ";
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(whitespaceName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByName_ReturnsBadRequest_WhenNameExceedsMaxLength()
    {
        // Arrange
        var tooLongName = new string('a', 256);
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(tooLongName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithStyleHavingNoTags()
    {
        // Arrange
        var styleName = "MinimalistArt";
        var styleResponse = new StyleResponse(styleName, "Abstract", "Minimalist style", null);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithStyleHavingNoDescription()
    {
        // Arrange
        var styleName = "AbstractArt";
        var styleResponse = new StyleResponse(styleName, "Abstract", null, ["modern"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "ClassicArt";
        var styleResponse = new StyleResponse(styleName, "Traditional", "Classic art style", ["traditional"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        GetStyleByName.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<StyleResponse>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetStyleByName.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.StyleName.Should().Be(styleName);
    }

    [Fact]
    public async Task GetByName_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "ModernArt";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.GetByName(styleName, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetByName_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "VintageArt";
        var styleResponse = new StyleResponse(styleName, "Retro", "Vintage style", ["retro", "old"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("AbstractDesign")]
    [InlineData("MinimalistArt")]
    [InlineData("VintagePattern")]
    public async Task GetByName_ReturnsOk_ForVariousStyleNames(string styleName)
    {
        // Arrange
        var styleResponse = new StyleResponse(styleName, "Custom", "Test description", ["tag1"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var styleName = "ConsistentStyle";
        var styleResponse = new StyleResponse(styleName, "Custom", "Consistent description", ["tag1"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByName(styleName, CancellationToken.None);
        var actionResult2 = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithValueOfType<StyleResponse>();
        actionResult2.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithStyleHavingMultipleTags()
    {
        // Arrange
        var styleName = "MultiTagStyle";
        var styleResponse = new StyleResponse(
            styleName,
            "Contemporary",
            "Style with multiple tags",
            ["modern", "abstract", "colorful", "dynamic"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithStyleNameContainingNumbers()
    {
        // Arrange
        var styleName = "Style2024";
        var styleResponse = new StyleResponse(styleName, "Modern", "2024 trending style", ["trend"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithStyleNameContainingHyphen()
    {
        // Arrange
        var styleName = "Modern-Art";
        var styleResponse = new StyleResponse(styleName, "Contemporary", "Modern art style", ["modern"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithStyleNameContainingUnderscore()
    {
        // Arrange
        var styleName = "Modern_Art";
        var styleResponse = new StyleResponse(styleName, "Contemporary", "Modern art style", ["modern"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "ErrorStyle";
        var failureResult = CreateFailureResult<StyleResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error occurred");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByName_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var styleName = "HandlerFailStyle";
        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithLongStyleName()
    {
        // Arrange
        var styleName = new string('a', 100);
        var styleResponse = new StyleResponse(styleName, "Custom", "Long name style", ["tag1"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "PerformanceTestStyle";
        var styleResponse = new StyleResponse(styleName, "Custom", "Performance test", ["test"]);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithEmptyTagsList()
    {
        // Arrange
        var styleName = "EmptyTagsStyle";
        var styleResponse = new StyleResponse(styleName, "Custom", "Style with empty tags", []);
        var result = Result.Ok(styleResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByName(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
    }

    [Fact]
    public async Task GetByName_ReturnsOk_WithDifferentStyleTypes()
    {
        // Arrange
        var styleName = "TestStyle";
        var styleTypes = new[] { "Abstract", "Realistic", "Minimalist", "Traditional", "Contemporary" };

        foreach (var styleType in styleTypes)
        {
            var styleResponse = new StyleResponse(styleName, styleType, "Test description", ["tag1"]);
            var result = Result.Ok(styleResponse);
            var senderMock = new Mock<ISender>();
            senderMock
                .Setup(s => s.Send(It.IsAny<GetStyleByName.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            var controller = CreateController(senderMock);

            // Act
            var actionResult = await controller.GetByName(styleName, CancellationToken.None);

            // Assert
            actionResult.Should().BeOkResult().WithValueOfType<StyleResponse>();
        }
    }
}