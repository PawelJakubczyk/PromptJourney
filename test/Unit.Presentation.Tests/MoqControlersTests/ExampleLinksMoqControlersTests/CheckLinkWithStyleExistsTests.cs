using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utilities.Constants;
using FluentAssertions;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinks;

public sealed class CheckLinkWithStyleExistsTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsTrue_WhenStyleHasLinks()
    {
        // Arrange
        var styleName = "ModernArt";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
        json.Should().Contain("\"exists\":true");
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsFalse_WhenStyleHasNoLinks()
    {
        // Arrange
        var styleName = "EmptyStyle";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
        json.Should().Contain("\"exists\":false");
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameInvalid()
    {
        // Arrange
        var invalidStyleName = "";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(invalidStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}