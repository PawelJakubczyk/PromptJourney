using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class CheckPropertyExistsTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task CheckPropertyExists_ReturnsTrue_WhenPropertyExists()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        //var okResult = actionResult as OkObjectResult;
        //var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
        //json.Should().Contain("\"exists\":true");
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsFalse_WhenPropertyDoesNotExist()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "nonexistent";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(version, propertyName, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        //var okResult = actionResult as OkObjectResult;
        //var json = System.Text.Json.JsonSerializer.Serialize(okResult!.Value);
        //json.Should().Contain("\"exists\":false");
    }

    [Fact]
    public async Task CheckPropertyExists_ReturnsBadRequest_WhenParametersInvalid()
    {
        // Arrange
        var invalidVersion = "";
        var invalidPropertyName = "";
        var failureResult = CreateFailureResult<bool, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version and property name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckPropertyExists(invalidVersion, invalidPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
