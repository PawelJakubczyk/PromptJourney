using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Features.Styles.Responses;
using Utilities.Constants;
using FluentAssertions;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class RemoveTagTests : StylesControllerTestsBase
{
    [Fact]
    public async Task RemoveTag_ReturnsNoContent_WhenTagRemovedSuccessfully()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "tagtoremove";
        var response = new StyleResponse(styleName, "Custom", "Description", ["remaining"]);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task RemoveTag_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var tag = "anytag";
        var failureResult = CreateFailureResult<StyleResponse>(
            StatusCodes.Status404NotFound,
            "Style not found",
            typeof(ApplicationLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenTagDoesNotExistInStyle()
    {
        // Arrange
        var styleName = "TestStyle";
        var tag = "nonexistenttag";
        var failureResult = CreateFailureResult<StyleResponse>(
            StatusCodes.Status400BadRequest,
            "Tag not found in style",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task RemoveTag_ReturnsBadRequest_WhenParametersInvalid()
    {
        // Arrange
        var styleName = "";
        var tag = "";
        var failureResult = CreateFailureResult<StyleResponse>(
            StatusCodes.Status400BadRequest,
            "Style name and tag cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.RemoveTag(styleName, tag, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}