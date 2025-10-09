using Application.Features.Properties.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Properties;

public sealed class DeletePropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task DeleteProperty_ReturnsNoContent_WhenPropertyDeletedSuccessfully()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var response = new PropertyResponse(version, propertyName, ["--ar"], "16:9", "1:4", "4:1", "Aspect ratio");
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "nonexistent";
        var failureResult = CreateFailureResult<PropertyResponse>(
            StatusCodes.Status404NotFound,
            "Property not found",
            typeof(ApplicationLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsBadRequest_WhenParametersInvalid()
    {
        // Arrange
        var invalidVersion = "";
        var invalidPropertyName = "";
        var failureResult = CreateFailureResult<PropertyResponse>(
            StatusCodes.Status400BadRequest,
            "Version and property name cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(invalidVersion, invalidPropertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProperty_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var version = "99.0";
        var propertyName = "aspect";
        var failureResult = CreateFailureResult<PropertyResponse>(
            StatusCodes.Status404NotFound,
            "Version not found",
            typeof(ApplicationLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteProperty(version, propertyName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }
}