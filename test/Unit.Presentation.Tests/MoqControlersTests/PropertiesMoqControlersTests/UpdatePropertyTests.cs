using Application.Features.Properties.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Properties;

public sealed class UpdatePropertyTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task UpdateProperty_ReturnsOk_WhenPropertyUpdatedSuccessfully()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new UpdatePropertyRequest(
            version,
            propertyName,
            ["--ar", "--aspect"],
            "1:1",
            "1:4",
            "4:1",
            "Updated aspect ratio parameter"
        );

        var response = new PropertyResponse(request.Version, request.PropertyName, request.Parameters,
            request.DefaultValue, request.MinValue, request.MaxValue, request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(version, propertyName, request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        okResult!.Value.Should().BeOfType<PropertyResponse>();
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenRouteParametersDontMatchPayload()
    {
        // Arrange
        var routeVersion = "1.0";
        var routePropertyName = "aspect";
        var request = new UpdatePropertyRequest(
            "2.0", // Different from route
            "quality", // Different from route
            ["--q"]
        );

        var controller = CreateController(new Mock<ISender>());

        // Act
        var actionResult = await controller.UpdateProperty(routeVersion, routePropertyName, request, CancellationToken.None);

        // Assert
        AssertBadRequestResult(actionResult, "Route parameters must match payload values");
    }

    [Fact]
    public async Task UpdateProperty_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "nonexistent";
        var request = new UpdatePropertyRequest(
            version,
            propertyName,
            ["--ne"]
        );

        var failureResult = CreateFailureResult<PropertyResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Property not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(version, propertyName, request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateProperty_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var invalidRequest = new UpdatePropertyRequest(
            version,
            propertyName,
            [] // Empty parameters invalid
        );

        var failureResult = CreateFailureResult<PropertyResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Parameters cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateProperty(version, propertyName, invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
