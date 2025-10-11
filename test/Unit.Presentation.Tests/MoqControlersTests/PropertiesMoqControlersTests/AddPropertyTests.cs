using Application.Features.Properties.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Properties;

public sealed class AddPropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task AddProperty_ReturnsCreated_WhenPropertyAddedSuccessfully()
    {
        // Arrange
        var version = "1.0";
        var request = new AddPropertyRequest(
            "stylize",
            ["--s", "--stylize"],
            "100",
            "0",
            "1000",
            "Stylization parameter"
        );

        var response = new PropertyResponse(version, request.PropertyName, request.Parameters,
            request.DefaultValue, request.MinValue, request.MaxValue, request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(version, request, CancellationToken.None);

        // Assert
        AssertCreatedResult<PropertyResponse>(actionResult, nameof(PropertiesController.CheckPropertyExists));
    }

    [Fact]
    public async Task AddProperty_ReturnsNoContent_WhenResultIsNull()
    {
        // Arrange
        var version = "1.0";
        var request = new AddPropertyRequest(
            "stylize",
            ["--s"]
        );

        PropertyResponse? nullResponse = null;
        var result = Result.Ok(nullResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(version, request, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task AddProperty_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var version = "1.0";
        var invalidRequest = new AddPropertyRequest(
            "",
            []
        );

        var failureResult = CreateFailureResult<PropertyResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid property data");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(version, invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddProperty_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var version = "99.0";
        var request = new AddPropertyRequest(
            "stylize",
            ["--s"]
        );

        var failureResult = CreateFailureResult<PropertyResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(version, request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }
}
