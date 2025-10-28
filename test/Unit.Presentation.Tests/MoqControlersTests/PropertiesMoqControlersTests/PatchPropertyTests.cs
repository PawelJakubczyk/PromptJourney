using Application.UseCases.Properties.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class PatchPropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task PatchProperty_ReturnsOk_WhenPropertyPatchedSuccessfully()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PatchPropertyRequest
        (
            propertyName,
            version,
            "DefaultValue",
            "2:1"
        );

        var response = new PropertyQueryResponse(version, propertyName, ["--ar"], "2:1", "1:4", "4:1", "Aspect ratio");
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        //var okResult = actionResult as OkObjectResult;
        //okResult!.Value.Should().BeOfType<PropertyQueryResponse>();
    }

    [Fact]
    public async Task PatchProperty_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "nonexistent";
        var request = new PatchPropertyRequest
        (
            propertyName,
            version,
            "DefaultValue",
            "2:1"
        );

        var failureResult = CreateFailureResult<PropertyQueryResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Property not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var invalidRequest = new PatchPropertyRequest
        (
            propertyName,
            version,
            "", // Invalid characteristic
            "value"
        );

        var failureResult = CreateFailureResult<PropertyQueryResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid characteristic to update");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PatchProperty_ReturnsBadRequest_WhenCharacteristicNotSupported()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PatchPropertyRequest
        (
            propertyName,
            version,
            "UnsupportedCharacteristic",
            "value"
        );

        var failureResult = CreateFailureResult<PropertyQueryResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Characteristic not supported for patching");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.PatchProperty(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
