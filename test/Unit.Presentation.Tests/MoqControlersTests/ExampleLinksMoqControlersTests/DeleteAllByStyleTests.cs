using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utilities.Constants;
using FluentAssertions;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinks;

public sealed class DeleteAllByStyleTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task DeleteAllByStyle_ReturnsNoContent_WhenLinksDeletedSuccessfully()
    {
        // Arrange
        var styleName = "ModernArt";
        var deletedCount = 5;
        var result = Result.Ok(deletedCount);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsNoContent_WhenNoLinksToDelete()
    {
        // Arrange
        var styleName = "EmptyStyle";
        var deletedCount = 0;
        var result = Result.Ok(deletedCount);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsNotFound_WhenStyleDoesNotExist()
    {
        // Arrange
        var styleName = "NonExistentStyle";
        var failureResult = CreateFailureResult<int>(
            StatusCodes.Status404NotFound,
            "Style not found",
            typeof(ApplicationLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(styleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteAllByStyle_ReturnsBadRequest_WhenStyleNameInvalid()
    {
        // Arrange
        var invalidStyleName = "";
        var failureResult = CreateFailureResult<int>(
            StatusCodes.Status400BadRequest,
            "Style name cannot be empty",
            typeof(DomainLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.DeleteAllByStyle(invalidStyleName, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}