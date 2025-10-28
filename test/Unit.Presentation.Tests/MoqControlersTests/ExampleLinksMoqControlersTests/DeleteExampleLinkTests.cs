using Application.UseCases.ExampleLinks.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class DeleteExampleLinkTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task DeleteExampleLink_ReturnsNoContent_WhenLinkDeletedSuccessfully()
    {
        // Arrange
        var link = "http://example.com/image.jpg";
        var response = new ExampleLinkResponse(link, "Style", "1.0");
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(link, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsNotFound_WhenLinkDoesNotExist()
    {
        // Arrange
        var link = "http://nonexistent.com/image.jpg";
        var failureResult = CreateFailureResult<ExampleLinkResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Link not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(link, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteExampleLink_ReturnsBadRequest_WhenLinkInvalid()
    {
        // Arrange
        var invalidLink = "invalid-link";
        var failureResult = CreateFailureResult<ExampleLinkResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid link format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteExampleLink(invalidLink, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
