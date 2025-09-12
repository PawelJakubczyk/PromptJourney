using Application.Features.VersionsMaster.Commands;
using Application.Features.VersionsMaster.Responses;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using Domain.ValueObjects;
using Moq;

namespace Unit.Test.Application.Features.VersionsMaster.Commands;

public class AddVersionCommandTests
{
    private readonly Mock<IVersionRepository> _mockVersionRepository;
    private readonly AddVersion.Handler _handler;

    public AddVersionCommandTests()
    {
        _mockVersionRepository = new Mock<IVersionRepository>();
        _handler = new AddVersion.Handler(_mockVersionRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var releaseDate = DateTime.UtcNow.AddDays(-30);
        var command = new AddVersion.Command
        (
            Version: "6.0",
            Parameter: "--v 6.0",
            ReleaseDate: releaseDate,
            Description: "Latest Midjourney version"
        );

        var expectedVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create("6.0"),
            Param.Create("--v 6.0"),
            releaseDate,
            Description.Create("Latest Midjourney version")
        ).Value;

        _mockVersionRepository
            .Setup(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()))
            .ReturnsAsync(Result.Ok(false));

        _mockVersionRepository
            .Setup(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()))
            .ReturnsAsync(Result.Ok(expectedVersion));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Should().Be("6.0");
        result.Value.Parameter.Should().Be("--v 6.0");
        result.Value.ReleaseDate.Should().Be(releaseDate);
        result.Value.Description.Should().Be("Latest Midjourney version");

        _mockVersionRepository.Verify(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()), Times.Once);
        _mockVersionRepository.Verify(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var command = new AddVersion.Command
        (
            Version: "5.1",
            Parameter: "--v 5.1"
        );

        var expectedVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create("5.1"),
            Param.Create("--v 5.1")
        ).Value;

        _mockVersionRepository
            .Setup(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()))
            .ReturnsAsync(Result.Ok(false));

        _mockVersionRepository
            .Setup(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()))
            .ReturnsAsync(Result.Ok(expectedVersion));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Should().Be("5.1");
        result.Value.Parameter.Should().Be("--v 5.1");
        result.Value.ReleaseDate.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithInvalidVersion_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddVersion.Command
        (
            Version: "invalid_version", // Invalid version format
            Parameter: "--v test"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockVersionRepository.Verify(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidParameter_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddVersion.Command
        (
            Version: "6.0",
            Parameter: "" // Invalid empty parameter
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockVersionRepository.Verify(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingVersion_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddVersion.Command
        (
            Version: "6.0",
            Parameter: "--v 6.0"
        );

        _mockVersionRepository
            .Setup(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()))
            .ReturnsAsync(Result.Ok(true)); // Version already exists

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockVersionRepository.Verify(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()), Times.Once);
        _mockVersionRepository.Verify(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()), Times.Never);
    }

    [Theory]
    [InlineData("1.0", "--v 1.0")]
    [InlineData("2.0", "--v 2.0")]
    [InlineData("5.1", "--v 5.1")]
    [InlineData("6.0", "--v 6.0")]
    [InlineData("niji 4", "--niji 4")]
    [InlineData("niji 5", "--niji 5")]
    [InlineData("niji 6", "--niji 6")]
    public async Task Handle_WithVariousValidVersions_ShouldReturnSuccess(string version, string parameter)
    {
        // Arrange
        var command = new AddVersion.Command
        (
            Version: version,
            Parameter: parameter,
            Description: $"Description for {version}"
        );

        var expectedVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(version),
            Param.Create(parameter),
            null,
            Description.Create($"Description for {version}")
        ).Value;

        _mockVersionRepository
            .Setup(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()))
            .ReturnsAsync(Result.Ok(false));

        _mockVersionRepository
            .Setup(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()))
            .ReturnsAsync(Result.Ok(expectedVersion));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Should().Be(version);
        result.Value.Parameter.Should().Be(parameter);
    }

    [Fact]
    public async Task Handle_WithFutureReleaseDate_ShouldReturnSuccess()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddMonths(6);
        var command = new AddVersion.Command
        (
            Version: "7.0",
            Parameter: "--v 7.0",
            ReleaseDate: futureDate
        );

        var expectedVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create("7.0"),
            Param.Create("--v 7.0"),
            futureDate
        ).Value;

        _mockVersionRepository
            .Setup(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()))
            .ReturnsAsync(Result.Ok(false));

        _mockVersionRepository
            .Setup(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()))
            .ReturnsAsync(Result.Ok(expectedVersion));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ReleaseDate.Should().Be(futureDate);
    }

    [Fact]
    public async Task Handle_WithRepositoryFailure_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddVersion.Command
        (
            Version: "6.0",
            Parameter: "--v 6.0"
        );

        _mockVersionRepository
            .Setup(x => x.CheckVersionExistsInVersionsAsync(It.IsAny<ModelVersion>()))
            .ReturnsAsync(Result.Ok(false));

        _mockVersionRepository
            .Setup(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()))
            .ReturnsAsync(Result.Fail<MidjourneyVersion>("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        _mockVersionRepository.Verify(x => x.AddVersionAsync(It.IsAny<MidjourneyVersion>()), Times.Once);
    }
}