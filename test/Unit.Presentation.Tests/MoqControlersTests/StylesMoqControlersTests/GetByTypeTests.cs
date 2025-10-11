using Application.Features.Styles.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class GetByTypeTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetByType_ReturnsOk_WhenStylesWithTypeExist()
    {
        // Arrange
        var styleType = "Custom";
        var styles = new List<StyleResponse>
        {
            new("Style1", styleType, "Description 1", ["tag1"]),
            new("Style2", styleType, "Description 2", ["tag2"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByType_ReturnsEmptyList_WhenNoStylesWithTypeExist()
    {
        // Arrange
        var styleType = "NonExistentType";
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(styleType, CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByType_ReturnsNotFound_WhenTypeIsInvalid()
    {
        // Arrange
        var invalidType = "InvalidType";
        var failureResult = CreateFailureResult<List<StyleResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Style type not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(invalidType, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByType_ReturnsBadRequest_WhenTypeParameterInvalid()
    {
        // Arrange
        var invalidType = "";
        var failureResult = CreateFailureResult<List<StyleResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Style type cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByType(invalidType, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
