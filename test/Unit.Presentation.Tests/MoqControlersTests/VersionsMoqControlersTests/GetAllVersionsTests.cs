using Application.Features.Versions.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Versions;

public sealed class GetAllVersionsTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOk_WhenVersionsExist()
    {
        // Arrange
        var versions = new List<VersionResponse>
        {
            new("1.0", "--v 1.0", DateTime.UtcNow, "Version 1.0"),
            new("2.0", "--v 2.0", DateTime.UtcNow, "Version 2.0")
        };

        var result = Result.Ok(versions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<VersionResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoVersionsExist()
    {
        // Arrange
        var emptyList = new List<VersionResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<VersionResponse>(actionResult, 0);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_WhenHandlerFails()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<VersionResponse>>(
            StatusCodes.Status500InternalServerError,
            "Database error",
            typeof(PersistenceLayer));

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status500InternalServerError);
    }
}