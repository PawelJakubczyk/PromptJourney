using Application.UseCases.Properties.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests;

public sealed class GetAllByVersionTests : PropertiesControllerTestsBase
{
    [Fact]
    public async Task GetAllByVersion_ReturnsOk_WhenPropertiesExist()
    {
        // Arrange
        var version = "1.0";
        var properties = new List<PropertyQueryResponse>
        {
            new("1.0", "aspect", ["--ar", "--aspect"], "16:9", "1:1", "32:1", "Aspect ratio parameter"),
            new("1.0", "quality", ["--q", "--quality"], "1", "0.25", "2", "Quality parameter")
        };

        var result = Result.Ok(properties);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        AssertOkResult<PropertyQueryResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsEmptyList_WhenNoPropertiesExist()
    {
        // Arrange
        var version = "1.0";
        var emptyList = new List<PropertyQueryResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        AssertOkResult<PropertyQueryResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsBadRequest_WhenVersionInvalid()
    {
        // Arrange
        var invalidVersion = "";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetAllByVersion_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var version = "99.0";
        var failureResult = CreateFailureResult<List<PropertyQueryResponse>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAllPropertiesByVersion(version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }
}
