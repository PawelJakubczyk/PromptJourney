using Application.UseCases.Properties.Commands;
using Application.UseCases.Properties.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class AddPropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task AddProperty_ReturnsCreated_WhenPropertyAddedSuccessfully()
    {
        // Arrange
        var version = "1.0";
        var request = new PropertyRequest(
            version,
            "stylize",
            ["--s", "--stylize"],
            "100",
            "0",
            "1000",
            "Stylization parameter"
        );

        var response = new PropertyCommandResponse(request.PropertyName, request.Version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeCreatedResult().WithActionName(nameof(PropertiesController.CheckPropertyExists));
    }

    [Fact]
    public async Task AddProperty_ReturnsNoContent_WhenResultIsNull()
    {
        // Arrange
        var version = "1.0";
        var request = new PropertyRequest(
            version,
            "stylize",
            ["--s"]
        );

        PropertyCommandResponse? nullResponse = null;
        var result = Result.Ok<PropertyCommandResponse?>(nullResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeNoContentResult();
    }

    [Fact]
    public async Task AddProperty_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var version = "1.0";
        var invalidRequest = new PropertyRequest(
            version,
            string.Empty,
            []
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid property data");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(invalidRequest, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddProperty_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var version = "99.0";
        var request = new PropertyRequest(
            version,
            "stylize",
            ["--s"]
        );

        var failureResult = CreateFailureResult<PropertyCommandResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(request, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status404NotFound);
    }
}
