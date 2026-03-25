using Application.UseCases.Properties.Commands;
using Application.UseCases.Properties.Responses;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Results;

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

        var response = new PropertyResponse(request.PropertyName, request.Version, request.Parameters, request.DefaultValue, request.MinValue, request.MaxValue, request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(PropertiesController.CheckPropertyExists));
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

        PropertyResponse? nullResponse = null;
        var result = Result.Ok<PropertyResponse?>(nullResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddProperty.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddProperty(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeNoContentResult();
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

        var failureResult = CreateFailureResult<PropertyResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Invalid property data");
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

        var failureResult = CreateFailureResult<PropertyResponse>(
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
        actionResult
            .Should()
            .BeNotFoundResult()
            .WithMessage("Version not found");
    }
}
