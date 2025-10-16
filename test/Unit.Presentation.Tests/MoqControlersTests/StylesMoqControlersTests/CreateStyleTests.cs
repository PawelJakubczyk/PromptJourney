using Application.UseCases.Styles.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class CreateStyleTests : StylesControllerTestsBase
{
    [Fact]
    public async Task Create_ReturnsCreated_WhenStyleCreatedSuccessfully()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom",
            "A new custom style",
            ["modern", "creative"]
        );

        var response = new StyleResponse(request.Name, request.Type, request.Description, request.Tags);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertCreatedResult<StyleResponse>(actionResult, nameof(StylesController.GetByName));
    }

    [Fact]
    public async Task Create_ReturnsNoContent_WhenResultIsNull()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "NewStyle",
            "Custom"
        );

        StyleResponse? nullResponse = null;
        var result = Result.Ok(nullResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var invalidRequest = new CreateStyleRequest(
            "",
            ""
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
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenStyleAlreadyExists()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "ExistingStyle",
            "Custom"
        );

        var failureResult = CreateFailureResult<StyleResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Style already exists");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
