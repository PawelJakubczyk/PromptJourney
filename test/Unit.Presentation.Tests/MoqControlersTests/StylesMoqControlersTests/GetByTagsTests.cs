using Application.Features.Styles.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class GetByTagsTests : StylesControllerTestsBase
{
    [Fact]
    public async Task GetByTags_ReturnsOk_WhenStylesWithTagsExist()
    {
        // Arrange
        var tags = new List<string> { "abstract", "modern" };
        var styles = new List<StyleResponse>
        {
            new("Style1", "Custom", "Description 1", ["abstract", "contemporary"]),
            new("Style2", "Custom", "Description 2", ["modern", "minimalist"])
        };

        var result = Result.Ok(styles);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetByTags_ReturnsEmptyList_WhenNoStylesWithTagsExist()
    {
        // Arrange
        var tags = new List<string> { "nonexistent" };
        var emptyList = new List<StyleResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(tags, CancellationToken.None);

        // Assert
        AssertOkResult<StyleResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetByTags_ReturnsNotFound_WhenTagsAreInvalid()
    {
        // Arrange
        var invalidTags = new List<string> { "" };
        var failureResult = CreateFailureResult<List<StyleResponse>>(
            StatusCodes.Status404NotFound,
            "No styles found with specified tags",
            typeof(ApplicationLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(invalidTags, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByTags_ReturnsBadRequest_WhenTagsListIsEmpty()
    {
        // Arrange
        var emptyTags = new List<string>();
        var failureResult = CreateFailureResult<List<StyleResponse>>(
            StatusCodes.Status400BadRequest,
            "Tags list cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByTags(emptyTags, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}