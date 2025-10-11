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

public sealed class UpdateDescriptionTests : StylesControllerTestsBase
{
    [Fact]
    public async Task UpdateDescription_ReturnsOk_WhenDescriptionUpdatedSuccessfully()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("Updated description");
        var response = new StyleResponse(styleName, "Custom", request.Description, ["tag1"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        okResult!.Value.Should().BeOfType<StyleResponse>();

        var returnedStyle = okResult.Value as StyleResponse;
        returnedStyle!.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task UpdateDescription_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WhenStyleNameInvalid()
    {
        // Arrange
        var invalidStyleName = "";
        var request = new UpdateDescriptionRequest("New description");
        var failureResult = CreateFailureResult<StyleResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(invalidStyleName, request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateDescription_ReturnsOk_WhenDescriptionSetToNull()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest(null!); // Setting description to null
        var response = new StyleResponse(styleName, "Custom", null, ["tag1"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.UpdateDescription(styleName, request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        var returnedStyle = okResult!.Value as StyleResponse;
        returnedStyle!.Description.Should().BeNull();
    }
}
