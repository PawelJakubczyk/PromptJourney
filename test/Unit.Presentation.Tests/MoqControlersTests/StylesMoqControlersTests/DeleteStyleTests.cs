using Application.Features.Styles.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Styles;

public sealed class DeleteStyleTests : StylesControllerTestsBase
{
    [Fact]
    public async Task Delete_ReturnsNoContent_WhenStyleDeletedSuccessfully()
    {
        // Arrange
        var styleName = "StyleToDelete";
        var response = new StyleResponse(styleName, "Custom", "Description", null);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
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
        var actionResult = await controller.Delete(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenNameInvalid()
    {
        // Arrange
        var invalidName = "";
        var failureResult = CreateFailureResult<StyleResponse>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Delete(invalidName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}