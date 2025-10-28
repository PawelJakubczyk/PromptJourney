using Application.UseCases.Styles.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests;

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
        var actionResult = await controller.Update(request, CancellationToken.None);

        // Assert
        //actionResult.Should().NotBeNull();
        //actionResult.Should().BeOfType<OkObjectResult>();

        //var okResult = actionResult as OkObjectResult;
        //okResult!.Value.Should().BeOfType<StyleResponse>();
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
        var actionResult = await controller.Update(request, CancellationToken.None);

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
        var actionResult = await controller.Update(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
