using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinks;

public sealed class CheckLinkExistsTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task CheckLinkExists_ReturnsTrue_WhenLinkExists()
    {
        // Arrange
        var link = "http://example.com/image.jpg";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(link, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var value = okResult.Value;
        value.Should().NotBeNull();
        // Checking the anonymous object structure { exists = true }
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        json.Should().Contain("\"exists\":true");
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsFalse_WhenLinkDoesNotExist()
    {
        // Arrange
        var link = "http://nonexistent.com/image.jpg";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(link, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
        json.Should().Contain("\"exists\":false");
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsBadRequest_WhenLinkInvalid()
    {
        // Arrange
        var invalidLink = "invalid-link";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Invalid link format",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkExists(invalidLink, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkExists_ReturnsInternalServerError_WhenHandlerFails()
    {
        // Arrange
        var link = "http://example.com/image.jpg";
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
        var actionResult = await controller.CheckLinkExists(link, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }
}