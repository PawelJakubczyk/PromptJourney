using Application.UseCases.Versions.Commands;
using Application.UseCases.Versions.Queries;
using Application.UseCases.Versions.Responses;
using Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Controllers;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Errors;
using Utilities.Results;

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
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            "New version 7.0"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(request.ReleaseDate!), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionIsEmpty()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
            StatusCodes.Status400BadRequest,
            "Version cannot be empty"
        );
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version cannot be empty");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterIsEmpty()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Parameter cannot be empty");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionIsNull()
    {
        // Arrange
        var request = new CreateVersionRequest(
            null!,
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version cannot be null");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterIsNull()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            null!,
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Parameter cannot be null");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionIsWhitespace()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "   ",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version cannot be whitespace");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterIsWhitespace()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "   ",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Parameter cannot be whitespace");
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenVersionAlreadyExists()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "1.0",
            "--v 1.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
            StatusCodes.Status409Conflict,
            "Version already exists");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeConflictResult()
            .WithMessage("Version already exists");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenVersionExceedsMaxLength()
    {
        // Arrange
        var tooLongVersion = new string('1', 256);
        var request = new CreateVersionRequest(
            tooLongVersion,
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Version exceeds maximum length");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenParameterExceedsMaxLength()
    {
        // Arrange
        var tooLongParameter = "--" + new string('v', Param.MaxLength);
        var request = new CreateVersionRequest(
            "7.0",
            tooLongParameter,
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Parameter exceeds maximum length");
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithMinimalRequest()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var response = new VersionResponse(request.Version, request.Parameter, null, null);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithCompleteRequest()
    {
        // Arrange
        var releaseDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            releaseDate,
            "Complete version with all details"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(releaseDate), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNullDescription()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            null
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(request.ReleaseDate!), null);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
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

        var response = new VersionResponse(request.Version, request.Parameter, null, request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNijiVersion()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "niji 7",
            "--niji 7",
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            "Niji version 7"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(request.ReleaseDate!), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
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
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            $"Test version {version}"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(request.ReleaseDate!), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_VerifiesCommandIsCalledWithCorrectParameters()
    {
        // Arrange
        var releaseDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            releaseDate,
            "Test description"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(releaseDate), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        AddVersion.Command? capturedCommand = null;

        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<VersionResponse>>, CancellationToken>((command, ct) =>
            {
                capturedCommand = command as AddVersion.Command;
            })
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        await controller.Create(request, CancellationToken.None);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Version.Should().Be(request.Version);
        capturedCommand.Parameter.Should().Be(request.Parameter);
        capturedCommand.ReleaseDate.Should().Be(releaseDate);
        capturedCommand.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task Create_HandlesCancellationToken()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions
            .Awaiting(() => controller.Create(request, cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Create_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var response = new VersionResponse(request.Version, request.Parameter, null, null);
        var result = Result.Ok(response);
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
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            longDescription
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(request.ReleaseDate!), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithSpecialCharactersInDescription()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            "Description with spéciál characters, émojis 🎨 and symbols @#$%^&*()"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(request.ReleaseDate!), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
            StatusCodes.Status400BadRequest,
            "Repository error during version creation");

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Repository error during version creation");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCommandHandlerFails()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd")
        );

        var failureResult = CreateFailureResult<VersionResponse>(
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
        actionResult
            .Should()
            .BeBadRequestResult()
            .WithMessage("Command handler failed");
    }

    [Fact]
    public async Task Create_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var request = new CreateVersionRequest(
            "7.0",
            "--v 7.0",
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            "Performance test version"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(request.ReleaseDate!), request.Description);
        var result = Result.Ok(response);
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
        var futureDate = DateTime.UtcNow.AddMonths(3).ToString("yyyy-MM-dd");
        var request = new CreateVersionRequest(
            "8.0",
            "--v 8.0",
            futureDate,
            "Future version"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(futureDate), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithPastReleaseDate()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddYears(-1).ToString("yyyy-MM-dd");
        var request = new CreateVersionRequest(
            "6.5",
            "--v 6.5",
            pastDate,
            "Past version"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(pastDate), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithVersionContainingDash()
    {
        // Arrange
        var releaseDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd");
        var request = new CreateVersionRequest(
            "7.0-beta",
            "--v 7.0",
            releaseDate,
            "Beta version"
        );

        var response = new VersionResponse(request.Version, request.Parameter, DateTime.Parse(releaseDate), request.Description);
        var result = Result.Ok(response);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<AddVersion.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .NotBeNull();
        actionResult
            .Should()
            .BeCreatedResult()
            .WithActionName(nameof(VersionsController.GetByVersion));
    }
}