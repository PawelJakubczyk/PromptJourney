using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Features.Styles.Responses;
using Utilities.Constants;
using FluentAssertions;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class GetByDescriptionTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetByDescription_ReturnsOk_WhenStylesWithKeywordExist()
    {
        // Arrange
        var keyword = "modern";
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Modern art style", ["abstract"]),
            new("Style2", "Custom", "Contemporary modern design", ["minimalist"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByDescription_ReturnsEmptyList_WhenNoStylesMatchKeyword()
    {
        // Arrange
        var keyword = "nonexistent";
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByDescription_ReturnsBadRequest_WhenKeywordInvalid()
    {
        // Arrange
        var invalidKeyword = "";
        var failureResult = CreateFailureResult<List<StyleResponse>>(
            StatusCodes.Status400BadRequest,
            "Keyword cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(invalidKeyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByDescription_ReturnsInternalServerError_WhenHandlerFails()
    {
        // Arrange
        var keyword = "test";
        var failureResult = CreateFailureResult<List<StyleResponse>>(
            StatusCodes.Status500InternalServerError,
            "Database error",
            typeof(PersistenceLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByDescription(keyword, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }
}