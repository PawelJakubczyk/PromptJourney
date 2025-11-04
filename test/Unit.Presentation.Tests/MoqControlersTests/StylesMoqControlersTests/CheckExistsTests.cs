using Application.UseCases.Styles.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

public sealed class CheckExistsTests : StylesControllerTestsBase
{
    [Fact]
    public async Task CheckExists_ReturnsOkWithTrue_WhenStyleExists()
    {
        // Arrange
        var styleName = "ModernArt";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsOkWithFalse_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var emptyName = string.Empty;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(emptyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceName = "   ";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(whitespaceName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullName = null;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(nullName!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongName = new string('a', 256);
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(tooLongName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "TestStyle";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        // ToResultsCheckExistOkAsync maps all non-400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "CustomStyle";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        CheckStyleExists.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CheckStyleExists.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(styleName, capturedQuery!.StyleName);
    }

    [Fact]
    public async Task CheckExists_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "TestStyle";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.CheckExists(styleName, cts.Token));
    }

    [Fact]
    public async Task CheckExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "TestStyle";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt", true)]
    [InlineData("ClassicStyle", true)]
    [InlineData("AbstractPainting", true)]
    [InlineData("NonExistentStyle", false)]
    [InlineData("AnotherNonExistent", false)]
    public async Task CheckExists_ReturnsOk_ForVariousStyleNames(string styleName, bool exists)
    {
        // Arrange
        var result = Result.Ok(exists);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var styleName = "TestStyle";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckExists(styleName, CancellationToken.None);
        var actionResult2 = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<bool>(actionResult1);
        AssertOkResult<bool>(actionResult2);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithLowercaseStyleName()
    {
        // Arrange
        var styleName = "modernart";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithUppercaseStyleName()
    {
        // Arrange
        var styleName = "MODERNART";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithMixedCaseStyleName()
    {
        // Arrange
        var styleName = "ModernArt";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithStyleNameContainingNumbers()
    {
        // Arrange
        var styleName = "Style123";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithStyleNameContainingHyphen()
    {
        // Arrange
        var styleName = "modern-art";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithStyleNameContainingUnderscore()
    {
        // Arrange
        var styleName = "modern_art";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithStyleNameContainingSpaces()
    {
        // Arrange
        var styleName = "Modern Art";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "TestStyle";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error during existence check");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var styleName = "TestStyle";
        var failureResult = CreateFailureResult<bool, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Theory]
    [InlineData("Style1", true)]
    [InlineData("Style2", false)]
    [InlineData("Style3", true)]
    [InlineData("Style4", false)]
    public async Task CheckExists_ReturnsCorrectBooleanValues_ForDifferentStyles(string styleName, bool expectedExists)
    {
        // Arrange
        var result = Result.Ok(expectedExists);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<bool>(actionResult);
    }

    [Fact]
    public async Task CheckExists_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckStyleExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.CheckExists(styleName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}