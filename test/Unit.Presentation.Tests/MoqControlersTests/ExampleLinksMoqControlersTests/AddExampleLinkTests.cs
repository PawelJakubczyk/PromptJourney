using Application.Features.ExampleLinks.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinks;

public sealed class AddExampleLinkTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task AddExampleLink_ReturnsCreated_WhenLinkAddedSuccessfully()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt",
            "1.0"
        );

        var response = new ExampleLinkResponse(request.Link, request.Style, request.Version);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertCreatedResult<ExampleLinkResponse>(actionResult, nameof(ExampleLinksController.CheckLinkExists));
    }

    [Fact]
    public async Task AddExampleLink_ReturnsNoContent_WhenResultIsNull()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "ModernArt",
            "1.0"
        );

        ExampleLinkResponse? nullResponse = null;
        var result = Result.Ok(nullResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var invalidRequest = new AddExampleLinkRequest(
            "invalid-link",
            "",
            ""
        );

        var failureResult = CreateFailureResult<ExampleLinkResponse, DomainLayer>(StatusCodes.Status400BadRequest, "Invalid request data");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsNotFound_WhenStyleOrVersionNotExist()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "http://example.com/image.jpg",
            "NonExistentStyle",
            "99.0"
        );

        var failureResult = CreateFailureResult<ExampleLinkResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style or version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.AddExampleLink(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }
}
