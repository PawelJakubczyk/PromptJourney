using Application.UseCases.ExampleLinks.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinks;

public sealed class GetByStyleAndVersionTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task GetByStyleAndVersion_ReturnsOk_WhenStyleAndVersionExist()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "1.0";
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com", styleName, version),
            new("http://example2.com", styleName, version)
        };

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        AssertOkResult<ExampleLinkResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsEmptyList_WhenNoMatchingLinks()
    {
        // Arrange
        var styleName = "ModernArt";
        var version = "99.0";
        var emptyList = new List<ExampleLinkResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        AssertOkResult<ExampleLinkResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsBadRequest_WhenParametersInvalid()
    {
        // Arrange
        var invalidStyleName = "";
        var invalidVersion = "";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid parameters");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(invalidStyleName, invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByStyleAndVersion_ReturnsNotFound_WhenStyleOrVersionNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var version = "1.0";
        var failureResult = CreateFailureResult<List<ExampleLinkResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style or version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByStyleAndVersion(styleName, version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }
}
