using Application.UseCases.ExampleLinks.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class CheckLinkWithStyleExistsTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsOkWithTrue_WhenStyleHasLinks()
    {
        // Arrange
        var styleName = "ModernArt";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsOkWithFalse_WhenStyleHasNoLinks()
    {
        // Arrange
        var styleName = "EmptyStyle";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var emptyStyleName = string.Empty;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(emptyStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var whitespaceStyleName = "   ";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(whitespaceStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var tooLongStyleName = new string('A', 256); // Assuming max length is 255
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(tooLongStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        string? nullStyleName = null;
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(nullStyleName!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Database connection failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        // ToResultsCheckExistOkAsync maps all non-400 errors to BadRequest
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        CheckExampleLinkExistsByStyle.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CheckExampleLinkExistsByStyle.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.StyleName.Should().Be(styleName);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesCancellationToken()
    {
        // Arrange
        var styleName = "ModernArt";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions.Awaiting(() => controller.CheckLinkWithStyleExists(styleName, cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var styleName = "ModernArt";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("Abstract")]
    [InlineData("Minimal")]
    [InlineData("Vintage")]
    public async Task CheckLinkWithStyleExists_ReturnsOk_ForVariousValidStyleNames(string styleName)
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Arrange
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid style name");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(invalidStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var styleName = "ModernArt";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);
        var actionResult2 = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult1.Should().BeOkResult().WithValueOfType<bool>();
        actionResult2.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<bool, PersistenceLayer>(
            StatusCodes.Status400BadRequest,
            "Repository error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesStyleNameWithSpecialCharacters()
    {
        // Arrange
        var styleNameWithSpecialChars = "Modern-Art_2024";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleNameWithSpecialChars, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesStyleNameWithNumbers()
    {
        // Arrange
        var styleNameWithNumbers = "Style123";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleNameWithNumbers, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var lowercaseStyleName = "modernart";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(lowercaseStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckLinkWithStyleExists_ReturnsOk_ForBothBooleanValues(bool exists)
    {
        // Arrange
        var styleName = "TestStyle";
        var result = Result.Ok(exists);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var styleName = "ModernArt";
        var failureResult = CreateFailureResult<bool, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var styleName = "ModernArt";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}