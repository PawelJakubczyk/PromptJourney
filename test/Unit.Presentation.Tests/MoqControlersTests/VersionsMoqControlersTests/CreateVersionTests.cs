using Application.UseCases.Versions.Commands;
using Application.UseCases.Versions.Responses;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

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

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "",
            "--v 7.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterIsEmpty()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            ""
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Parameter cannot be empty");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        var request = new CreateVersionRequest(
            null!,
            "--v 7.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterIsNull()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            null!
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Parameter cannot be null");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "   ",
            "--v 7.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterIsWhitespace()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "   "
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Parameter cannot be whitespace");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

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

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Version already exists");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionExceedsMaxLength()
    {
        // Arrange
        var tooLongVersion = new string('1', 256);
        var request = new CreateVersionRequest(
            tooLongVersion,
            "--v 7.0"
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Version exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterExceedsMaxLength()
    {
        // Arrange
        var tooLongParameter = new string('v', 256);
        var request = new CreateVersionRequest(
            "7.0",
            tooLongParameter
        );

        var failureResult = CreateFailureResult<string, DomainLayer>(
            StatusCodes.Status400BadRequest,
            "Parameter exceeds maximum length");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithMinimalRequest()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithCompleteRequest()
    {
        // Arrange
        var releaseDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            releaseDate,
            "Complete version with all details"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNullDescription()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow,
            null
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNullReleaseDate()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            null,
            "Version without release date"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNijiVersion()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "niji 7",
            "--niji 7",
            DateTime.UtcNow,
            "Niji version 7"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Theory]
    [InlineData("1.0", "--v 1.0")]
    [InlineData("2.5", "--v 2.5")]
    [InlineData("5.2", "--v 5.2")]
    [InlineData("6.0", "--v 6.0")]
    [InlineData("niji 5", "--niji 5")]
    [InlineData("niji 6", "--niji 6")]
    public async Task Create_ReturnsCreated_ForVariousVersionFormats(string version, string parameter)
    {
        // Arrange
        var request = new CreateVersionRequest(
            version,
            parameter,
            DateTime.UtcNow,
            $"Test version {version}"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var releaseDate = DateTime.UtcNow;
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            releaseDate,
            "Test description"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        AddVersion.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<string>>, CancellationToken>((command, ct) =>
            {
                capturedCommand = command as AddVersion.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Create(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(request.Version, capturedCommand!.Version);
        Assert.Equal(request.Parameter, capturedCommand.Parameter);
        Assert.Equal(releaseDate, capturedCommand.ReleaseDate);
        Assert.Equal(request.Description, capturedCommand.Description);
    }

    [Fact]
    public async Task Create_HandlesCancellationToken()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0"
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            controller.Create(request, cts.Token));
    }

    [Fact]
    public async Task Create_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Create(request, CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithLongDescription()
    {
        // Arrange
        var longDescription = new string('A', 1000) + " This is a very long description for testing purposes.";
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow,
            longDescription
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithSpecialCharactersInDescription()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow,
            "Description with spéciál characters, émojis 🎨 and symbols @#$%^&*()"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0"
        );

        var failureResult = CreateFailureResult<string, PersistenceLayer>(
            StatusCodes.Status500InternalServerError,
            "Repository error during version creation");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0"
        );

        var failureResult = CreateFailureResult<string, ApplicationLayer>(
            StatusCodes.Status400BadRequest,
            "Command handler failed");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        AssertErrorResult(actionResult, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Create_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow,
            "Performance test version"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;

        // Act
        await controller.Create(request, CancellationToken.None);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithFutureReleaseDate()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddMonths(3);
        var request = new CreateVersionRequest(
            "8.0",
            "--v 8.0",
            futureDate,
            "Future version"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithPastReleaseDate()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddYears(-1);
        var request = new CreateVersionRequest(
            "6.5",
            "--v 6.5",
            pastDate,
            "Past version"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithVersionContainingDash()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0-beta",
            "--v 7.0",
            DateTime.UtcNow,
            "Beta version"
        );

        var result = Result.Ok(request.Version);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult.Should().NotBeNull();
        AssertCreatedResult<string>(actionResult, nameof(VersionsController.GetByVersion));
    }
}