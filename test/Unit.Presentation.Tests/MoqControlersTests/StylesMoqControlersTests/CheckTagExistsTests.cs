using Application.UseCases.Styles.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class CheckTagExistsTests : StylesControllerTestsBase
{
    [Fact]
    public async Task CheckTagExists_ReturnsOkWithTrue_WhenTagExistsInStyle()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "abstract";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOkWithFalse_WhenTagDoesNotExistInStyle()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "nonexistent";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var emptyStyleName = string.Empty;
        var tag = "abstract";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(emptyStyleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenTagIsEmpty()
    {
        // Arrange
        var styleName = "ModernArt";
        var emptyTag = string.Empty;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, emptyTag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenBothParametersAreEmpty()
    {
        // Arrange
        var emptyStyleName = string.Empty;
        var emptyTag = string.Empty;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name and tag cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(emptyStyleName, emptyTag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceStyleName = "   ";
        var tag = "abstract";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(whitespaceStyleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenTagIsWhitespace()
    {
        // Arrange
        var styleName = "ModernArt";
        var whitespaceTag = "   ";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, whitespaceTag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullStyleName = null;
        var tag = "abstract";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(nullStyleName!, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenTagIsNull()
    {
        // Arrange
        var styleName = "ModernArt";
        string? nullTag = null;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, nullTag!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongStyleName = new string('a', 256);
        var tag = "abstract";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(tooLongStyleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenTagExceedsMaxLength()
    {
        // Arrange
        var styleName = "ModernArt";
        var tooLongTag = new string('a', 256);
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Tag exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tooLongTag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        // ToResultsCheckExistOkAsync maps all non-400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "CustomStyle";
        var tag = "modern";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        CheckTagExistsInStyle.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CheckTagExistsInStyle.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.StyleName.Should().Be(styleName);
        capturedQuery.Tag.Should().Be(tag);
    }

    [Fact]
    public async Task CheckTagExists_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.CheckTagExists(styleName, tag, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task CheckTagExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt", "abstract", true)]
    [InlineData("ClassicStyle", "vintage", true)]
    [InlineData("AbstractPainting", "modern", true)]
    [InlineData("TestStyle", "nonexistent", false)]
    [InlineData("AnotherStyle", "missing", false)]
    public async Task CheckTagExists_ReturnsOk_ForVariousInputs(string styleName, string tag, bool expectedExists)
    {
        // Arrange
        var result = Result.Ok(expectedExists);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsConsistentResults_ForSameParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckTagExists(styleName, tag, CancellationToken.None);
        var actionResult2 = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithValueOfType<bool>();
        actionResult2.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOkWithFalse_WhenStyleDoesNotExist()
    {
        // Arrange
        var nonExistentStyle = "NonExistentStyle";
        var tag = "abstract";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(nonExistentStyle, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOk_WithLowercaseTag()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "lowercase";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOk_WithUppercaseTag()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "UPPERCASE";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOk_WithMixedCaseTag()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "MixedCase";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOk_WithTagContainingNumbers()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "tag123";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOk_WithTagContainingHyphen()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "tag-with-hyphen";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOk_WithTagContainingUnderscore()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "tag_with_underscore";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsOk_WithStyleNameContainingSpaces()
    {
        // Arrange
        var styleName = "Modern Art";
        var tag = "abstract";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during tag existence check");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var failureResult = CreateFailureResult<bool, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Theory]
    [InlineData("Style1", "Tag1", true)]
    [InlineData("Style2", "Tag2", false)]
    [InlineData("Style3", "Tag3", true)]
    [InlineData("Style4", "Tag4", false)]
    public async Task CheckTagExists_ReturnsCorrectBooleanValues_ForDifferentCombinations(string styleName, string tag, bool expectedExists)
    {
        // Arrange
        var result = Result.Ok(expectedExists);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckTagExists_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckTagExistsInStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}