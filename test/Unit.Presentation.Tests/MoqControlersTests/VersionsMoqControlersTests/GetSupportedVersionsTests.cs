using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

public sealed class GetSupportedVersionsTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task GetSupported_ReturnsOk_WhenSupportedVersionsExist()
    {
        // Arrange
        var supportedVersions = new List<string> { "1.0", "2.0", "5.1", "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult, 4);
    }

    [Fact]
    public async Task GetSupported_ReturnsEmptyList_WhenNoSupportedVersionsExist()
    {
        // Arrange
        var emptyList = new List<string>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        AssertOkResult<string>(actionResult, 0);
    }

    [Fact]
    public async Task GetSupported_ReturnsNotFound_WhenNoVersionsConfigured()
    {
        // Arrange
        var failureResult = CreateFailureResult<List<string>, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "No supported versions found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }
}
