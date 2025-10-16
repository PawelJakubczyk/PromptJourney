using Application.UseCases.Versions.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.Versions;

public sealed class CreateVersionTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task Create_ReturnsCreated_WhenVersionCreatedSuccessfully()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow,
            "New version 7.0"
        );

        var response = new VersionResponse(request.Version, request.Parameter, request.ReleaseDate, request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertCreatedResult<VersionResponse>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsNoContent_WhenResultIsNull()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0"
        );

        VersionResponse? nullResponse = null;
        var result = Result.Ok(nullResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertNoContentResult(actionResult);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRequestInvalid()
    {
        // Arrange
        var invalidRequest = new CreateVersionRequest(
            "",
            ""
        );

        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version data");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(invalidRequest, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionAlreadyExists()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "1.0",
            "--v 1.0"
        );

        var failureResult = CreateFailureResult<VersionResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Version already exists");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }
}
