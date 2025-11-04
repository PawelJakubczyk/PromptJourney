using Application.UseCases.Versions.Queries;
using Application.UseCases.Versions.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

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
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
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
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var invalidVersion = "";
        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        string? nullVersion = null;
        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(nullVersion!, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var whitespaceVersion = "   ";
        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(whitespaceVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionHasInvalidFormat()
    {
        // Arrange
        var invalidVersion = "invalid.format.here";
        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(invalidVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenVersionExceedsMaxLength()
    {
        // Arrange
        var tooLongVersion = new string('1', 256);
        var failureResult = CreateFailureResult<VersionResponse, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(tooLongVersion, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithCompleteVersionData()
    {
        // Arrange
        var version = "1.0";
        var releaseDate = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var versionResponse = new VersionResponse(
            version,
            "--v 1.0",
            releaseDate,
            "Complete version with all fields");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithNullDescription()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, null);
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithNullReleaseDate()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", null, "Version without release date");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_ForNijiVersion()
    {
        // Arrange
        var version = "niji 5";
        var versionResponse = new VersionResponse(version, "--niji 5", DateTime.UtcNow, "Niji version 5");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.5")]
    [InlineData("3.0")]
    [InlineData("4.0")]
    [InlineData("5.2")]
    [InlineData("6.0")]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    public async Task GetByVersion_ReturnsOk_ForVariousVersionFormats(string version)
    {
        // Arrange
        var versionResponse = new VersionResponse(version, $"--v {version}", DateTime.UtcNow, $"Version {version}");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, "Version 1.0");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        GetVersion.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<VersionResponse>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetVersion.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.Equal(version, capturedQuery!.Version);
    }

    [Fact]
    public async Task GetByVersion_HandlesCancellationToken()
    {
        // Arrange
        var version = "1.0";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.GetByVersion(version, cts.Token));
    }

    [Fact]
    public async Task GetByVersion_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, "Version 1.0");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByVersion_ReturnsConsistentResults_ForSameVersion()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, "Version 1.0");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetByVersion(version, CancellationToken.None);
        var actionResult2 = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult1.Should().NotBeNull();
        actionResult2.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult1, -1);
        AssertOkResult<VersionResponse>(actionResult2, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithVersionContainingDash()
    {
        // Arrange
        var version = "7.0-beta";
        var versionResponse = new VersionResponse(version, "--v 7.0", DateTime.UtcNow, "Beta version");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithVersionContainingMultipleDecimals()
    {
        // Arrange
        var version = "1.2.3";
        var versionResponse = new VersionResponse(version, "--v 1.2.3", DateTime.UtcNow, "Version with multiple decimals");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithLongDescription()
    {
        // Arrange
        var version = "1.0";
        var longDescription = new string('A', 1000) + " This is a very long description for testing purposes.";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, longDescription);
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithSpecialCharactersInDescription()
    {
        // Arrange
        var version = "1.0";
        var description = "Description with spéciál characters, émojis 🎨 and symbols @#$%^&*()";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, description);
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenRepositoryFails()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<VersionResponse, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during version retrieval");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        // ToResultsOkAsync maps all non-404/400 errors to BadRequest
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByVersion_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<VersionResponse, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetByVersion_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--v 1.0", DateTime.UtcNow, "Performance test");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithFutureReleaseDate()
    {
        // Arrange
        var version = "8.0";
        var futureDate = DateTime.UtcNow.AddMonths(6);
        var versionResponse = new VersionResponse(version, "--v 8.0", futureDate, "Future version");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithPastReleaseDate()
    {
        // Arrange
        var version = "1.0";
        var pastDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var versionResponse = new VersionResponse(version, "--v 1.0", pastDate, "Old version");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithMajorVersionOnly()
    {
        // Arrange
        var version = "6";
        var versionResponse = new VersionResponse(version, "--v 6", DateTime.UtcNow, "Major version only");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithAlphaVersion()
    {
        // Arrange
        var version = "8.0-alpha";
        var versionResponse = new VersionResponse(version, "--v 8.0", DateTime.UtcNow, "Alpha version");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithDifferentParameterFormats()
    {
        // Arrange
        var version = "1.0";
        var versionResponse = new VersionResponse(version, "--version 1.0", DateTime.UtcNow, "Different parameter format");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }

    [Fact]
    public async Task GetByVersion_ReturnsOk_WithVersionZero()
    {
        // Arrange
        var version = "0.0";
        var versionResponse = new VersionResponse(version, "--v 0.0", DateTime.UtcNow, "Version zero");
        var result = Result.Ok(versionResponse);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<GetVersion.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetByVersion(version, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertOkResult<VersionResponse>(actionResult, -1);
    }
}