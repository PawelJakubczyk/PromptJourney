using Application.Features.Versions.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Versions;

public sealed class GetByVersionTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task GetByVersion_ReturnsOk_WhenVersionExists()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, "Version 1.0");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult as OkObjectResult;
        okResult!.Value.Should().BeOfType<VersionResponse>();

        var returnedVersion = okResult.Value as VersionResponse;
        returnedVersion!.Version.Should().Be(version);
    }

    [Fact]
    public async Task GetByVersion_ReturnsNotFound_WhenVersionDoesNotExist()
    {
        // Arrange
        var version = "99.0";
        var failureResult = CreateFailureResult<VersionResponse, ApplicationLayer>(
            StatusCodes.Status404NotFound,
            "Version not found");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionInvalid()
    {
        // Arrange
        var invalidVersion = "";
        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
