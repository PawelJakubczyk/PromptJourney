using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utilities.Constants;
using FluentAssertions;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class CheckTagExistsTests : StylesControllerTestsBase
{
    [Fact]
    public async Task CheckTagExists_ReturnsTrue_WhenTagExistsInStyle()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "abstract";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
        json.Should().Contain("\"exists\":true");
    }

    [Fact]
    public async Task CheckTagExists_ReturnsFalse_WhenTagDoesNotExistInStyle()
    {
        // Arrange
        var styleName = "ModernArt";
        var tag = "nonexistent";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
        json.Should().Contain("\"exists\":false");
    }

    [Fact]
    public async Task CheckTagExists_ReturnsBadRequest_WhenParametersInvalid()
    {
        // Arrange
        var invalidStyleName = "";
        var invalidTag = "";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Style name and tag cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(invalidStyleName, invalidTag, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckTagExists_ReturnsInternalServerError_WhenHandlerFails()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "testtag";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status500InternalServerError,
            "Database error",
            typeof(PersistenceLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckTagExists(styleName, tag, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }
}