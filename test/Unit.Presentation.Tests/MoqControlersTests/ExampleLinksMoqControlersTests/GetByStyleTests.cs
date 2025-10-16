using Application.UseCases.ExampleLinks.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinks;

public sealed class GetByStyleTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task GetByStyle_ReturnsOk_WhenStyleExists()
    {
        // Arrange
        var styleName = "ModernArt";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com", styleName, "1.0"),
            new("http://example2.com", styleName, "2.0")
        };

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        AssertOkResult<ExampleLinkResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByStyle_ReturnsEmptyList_WhenStyleHasNoLinks()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var emptyList = new List<ExampleLinkResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        AssertOkResult<ExampleLinkResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByStyle_ReturnsBadRequest_WhenStyleNameInvalid()
    {
        // Arrange
        var invalidStyleName = "";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(invalidStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyle_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyle(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }
}
