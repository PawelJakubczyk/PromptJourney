using Application.UseCases.Versions.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Results;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

public sealed class CheckVersionExistsTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task CheckExists_ReturnsOkWithTrue_WhenVersionExists()
    {
        // Arrange
        var version = "1.0";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Fact]
    public async Task CheckExists_ReturnsOkWithFalse_WhenVersionDoesNotExist()
    {
        // Arrange
        var version = "99.0";
        var result = Result.Ok(false);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(false);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var invalidVersion = "";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(invalidVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version cannot be empty");
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        string? nullVersion = null;
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Version cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(nullVersion!, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version cannot be null");
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var whitespaceVersion = "   ";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(whitespaceVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version cannot be whitespace");
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenVersionHasInvalidFormat()
    {
        // Arrange
        var invalidVersion = "invalid.version.format";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Invalid version format");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(invalidVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Invalid version format");
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenVersionExceedsMaxLength()
    {
        // Arrange
        var tooLongVersion = new string('1', 256);
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Version exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(tooLongVersion, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version exceeds maximum length");
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenRepositoryFails()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Database error");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Database error");
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.5")]
    [InlineData("3.0")]
    [InlineData("4.0")]
    [InlineData("5.2")]
    [InlineData("6.0")]
    public async Task CheckExists_ReturnsOk_ForVariousVersionFormats(string version)
    {
        // Arrange
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Fact]
    public async Task CheckExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var version = "1.0";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        CheckVersionExists.Query? capturedQuery = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as CheckVersionExists.Query;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckExists(version, CancellationToken.None);

        // Assert
        capturedQuery.Should().NotBeNull();
        capturedQuery!.Version.Should().Be(version);
    }

    [Fact]
    public async Task CheckExists_HandlesCancellationToken()
    {
        // Arrange
        var version = "1.0";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions
            .Awaiting(() => controller.CheckExists(version, cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task CheckExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var version = "1.0";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.CheckExists(version, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckExists_ReturnsConsistentResults_ForSameVersion()
    {
        // Arrange
        var version = "1.0";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.CheckExists(version, CancellationToken.None);
        var actionResult2 = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult1
            .Should()
            .NotBeNull();
        actionResult2
            .Should()
            .NotBeNull();
        actionResult1
            .Should()
            .BeOkResult()
            .WithValue(true);
        actionResult2
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithVersionContainingMultipleDecimals()
    {
        // Arrange
        var version = "1.2.3";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithBetaVersion()
    {
        // Arrange
        var version = "7.0-beta";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var version = "1.0";
        var failureResult = CreateFailureResult<bool>(
            StatusCodes.Status400BadRequest,
            "Query handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Query handler failed");
    }

    [Fact]
    public async Task CheckExists_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.CheckExists(version, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithMajorVersionOnly()
    {
        // Arrange
        var version = "6";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithDecimalMinorVersion()
    {
        // Arrange
        var version = "1.5";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Theory]
    [InlineData("1.0", true)]
    [InlineData("2.0", false)]
    [InlineData("3.5", true)]
    [InlineData("4.0", false)]
    [InlineData("5.2", true)]
    public async Task CheckExists_ReturnsCorrectResult_ForVariousExistenceStates(string version, bool exists)
    {
        // Arrange
        var result = Result.Ok(exists);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();

        actionResult
            .Should()
            .BeOkResult()
            .WithValue(exists);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_WithVersionContainingDash()
    {
        // Arrange
        var version = "6.0-niji";
        var result = Result.Ok(true);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckExists(version, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeOkResult()
            .WithValue(true);
    }

    [Fact]
    public async Task CheckExists_ReturnsOk_ForMultipleSequentialChecks()
    {
        // Arrange
        var versions = new[] { "1.0", "2.0", "3.0", "4.0", "5.0" };
        var senderMock = new Mock<ISender>();
        var controller = CreateController(senderMock);

        foreach (var version in versions)
        {
            var result = Result.Ok(true);
            senderMock
                .Setup(s => s.Send(It.IsAny<CheckVersionExists.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await controller.CheckExists(version, CancellationToken.None);

            // Assert
            actionResult
                .Should()
                .NotBeNull();
            actionResult
                .Should()
                .BeOkResult()
                .WithValue(true);
        }
    }
}