using Application.UseCases.Styles.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class UpdateStyleTests : StylesControllerTestsBase
{
    [Fact]
    public async Task Update_ReturnsOk_WhenStyleUpdatedSuccessfully()
    {
        // Arrange
        var styleName = "ExistingStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Updated description",
            ["updated", "tags"]
        );

        var response = new StyleResponse(request.Name, request.Type, request.Description, request.Tags);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(styleName, request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        okResult!.Value.Should().BeOfType<StyleResponse>();
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenRouteNameAndPayloadNameDontMatch()
    {
        // Arrange
        var routeName = "Style1";
        var request = new UpdateStyleRequest(
            "Style2", // Different from route name
            "Custom"
        );

        var controller = CreateController(new Mock<ISender>());

        // Act
        var actionResult = await controller.Update(routeName, request, CancellationToken.None);

        // Assert
        AssertBadRequestResult(actionResult, "Route name and payload name must match");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom"
        );

        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(styleName, request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var styleName = "TestStyle";
        var invalidRequest = new UpdateStyleRequest(
            styleName,
            "" // Invalid empty type
        );

        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid style data");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Update(styleName, invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
