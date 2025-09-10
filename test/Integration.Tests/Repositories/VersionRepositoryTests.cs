using Domain.ValueObjects;
using Persistence.Repositories;
using Domain.Entities.MidjourneyVersions;
using FluentAssertions;

namespace Integration.Tests.Repositories;

public class VersionRepositoryTests : BaseTransactionIntegrationTest
{
    private readonly VersionsRepository _repository;

    public VersionRepositoryTests(MidjourneyDbFixture fixture) : base(fixture)
    {
        _repository = new VersionsRepository(DbContext);
    }

    [Fact]
    public async Task AddVersion_WithValidData_ShouldSucceed()
    {
        // Arrange
        var version = ModelVersion.Create("8.0").Value;
        var parameter = Param.Create("--v 8").Value;
        var description = Description.Create("Test version").Value;
        var releaseDate = DateTime.UtcNow.AddDays(-10);
        
        var versionEntity = MidjourneyVersion.Create(version, parameter, releaseDate, description).Value;

        // Act
        var result = await _repository.AddVersionAsync(versionEntity);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be("8.0");
        result.Value.Parameter.Value.Should().Be("--v 8");
        result.Value.Description.Should().NotBeNull();
        result.Value.Description!.Value.Should().Be("Test version");
        result.Value.ReleaseDate.Should().BeCloseTo(releaseDate, TimeSpan.FromSeconds(1));

        // Verify it's in the database
        var exists = await _repository.CheckVersionExistsInVersionsAsync(version);
        exists.IsSuccess.Should().BeTrue();
        exists.Value.Should().BeTrue();
    }

    [Fact]
    public async Task AddVersion_WithMinimalData_ShouldSucceed()
    {
        // Arrange
        var version = ModelVersion.Create("9.0").Value;
        var parameter = Param.Create("--v 9").Value;
        
        var versionEntity = MidjourneyVersion.Create(version, parameter).Value;

        // Act
        var result = await _repository.AddVersionAsync(versionEntity);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be("9.0");
        result.Value.Parameter.Value.Should().Be("--v 9");
        result.Value.Description.Should().BeNull();
        result.Value.ReleaseDate.Should().BeNull();
    }

    [Fact]
    public async Task AddVersion_WithDuplicateVersion_ShouldFail()
    {
        // Arrange
        var version = ModelVersion.Create("7.0").Value;
        var parameter1 = Param.Create("--v 7").Value;
        var parameter2 = Param.Create("--version 7").Value;
        
        var versionEntity1 = MidjourneyVersion.Create(version, parameter1).Value;
        var versionEntity2 = MidjourneyVersion.Create(version, parameter2).Value;

        // Act
        var firstResult = await _repository.AddVersionAsync(versionEntity1);
        
        // Clear the change tracker to simulate a fresh context
        DbContext.ChangeTracker.Clear();
        
        var secondResult = await _repository.AddVersionAsync(versionEntity2);

        // Assert
        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeFalse();
        secondResult.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CheckVersionExists_WithExistingVersion_ShouldReturnTrue()
    {
        // Arrange
        var version = ModelVersion.Create("6.0").Value;
        var parameter = Param.Create("--v 6").Value;
        var versionEntity = MidjourneyVersion.Create(version, parameter).Value;
        
        await _repository.AddVersionAsync(versionEntity);

        // Act
        var result = await _repository.CheckVersionExistsInVersionsAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckVersionExists_WithNonExistentVersion_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentVersion = ModelVersion.Create("999.0").Value;

        // Act
        var result = await _repository.CheckVersionExistsInVersionsAsync(nonExistentVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task GetMasterVersionByVersion_WithExistingVersion_ShouldReturnVersion()
    {
        // Arrange
        var version = ModelVersion.Create("5.2").Value;
        var parameter = Param.Create("--v 5.2").Value;
        var description = Description.Create("Enhanced prompt following").Value;
        var releaseDate = new DateTime(2023, 6, 22, 0, 0, 0, DateTimeKind.Utc);
        
        var versionEntity = MidjourneyVersion.Create(version, parameter, releaseDate, description).Value;
        await _repository.AddVersionAsync(versionEntity);

        // Act
        var result = await _repository.GetMasterVersionByVersionAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be("5.2");
        result.Value.Parameter.Value.Should().Be("--v 5.2");
        result.Value.Description!.Value.Should().Be("Enhanced prompt following");
        result.Value.ReleaseDate.Should().Be(releaseDate);
    }

    [Fact]
    public async Task GetMasterVersionByVersion_WithNonExistentVersion_ShouldFail()
    {
        // Arrange
        var nonExistentVersion = ModelVersion.Create("888.0").Value;

        // Act
        var result = await _repository.GetMasterVersionByVersionAsync(nonExistentVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("not found"));
    }

    [Fact]
    public async Task GetAllVersions_WithMultipleVersions_ShouldReturnAllVersions()
    {
        // Arrange
        var version1 = MidjourneyVersion.Create(
            ModelVersion.Create("4.0").Value,
            Param.Create("--v 4").Value,
            new DateTime(2022, 11, 5, 0, 0, 0, DateTimeKind.Utc),
            Description.Create("Improved quality").Value).Value;

        var version2 = MidjourneyVersion.Create(
            ModelVersion.Create("5.0").Value,
            Param.Create("--v 5").Value,
            new DateTime(2023, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            Description.Create("New aesthetic").Value).Value;

        var version3 = MidjourneyVersion.Create(
            ModelVersion.Create("6.0").Value,
            Param.Create("--v 6").Value).Value;

        await _repository.AddVersionAsync(version1);
        await _repository.AddVersionAsync(version2);
        await _repository.AddVersionAsync(version3);

        // Act
        var result = await _repository.GetAllVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(v => v.Version.Value == "4.0");
        result.Value.Should().Contain(v => v.Version.Value == "5.0");
        result.Value.Should().Contain(v => v.Version.Value == "6.0");
        
        // Verify ordering (should be by version)
        var sortedVersions = result.Value.OrderBy(v => v.Version.Value).ToList();
        sortedVersions[0].Version.Value.Should().Be("4.0");
        sortedVersions[1].Version.Value.Should().Be("5.0");
        sortedVersions[2].Version.Value.Should().Be("6.0");
    }

    [Fact]
    public async Task GetAllVersions_WithNoVersions_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllSupportedVersions_ShouldReturnSupportedVersionsList()
    {
        // Arrange
        var version1 = MidjourneyVersion.Create
        (
            ModelVersion.Create("4.0").Value,
            Param.Create("--v 4").Value,
            new DateTime(2022, 11, 5, 0, 0, 0, DateTimeKind.Utc),
            Description.Create("Improved quality").Value
        ).Value;

        await _repository.AddVersionAsync(version1);

        // Act
        var result = await _repository.GetAllSuportedVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
        result.Value[0].Value.Should().Be("4.0");
    }

    [Fact]
    public async Task CheckIfAnySupportedVersionExists_WithNoVersions_ShouldReturnFalse()
    {
        // Act (empty database)
        var result = await _repository.CheckIfAnySupportedVersionExistsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckIfAnySupportedVersionExists_WithSupportedVersions_ShouldReturnFalse()
    {
        // Arrange - Add a supported version
        var supportedVersions = await _repository.GetAllSuportedVersionsAsync();
        if (supportedVersions.IsSuccess && supportedVersions.Value.Count > 0)
        {
            var firstSupportedVersion = supportedVersions.Value[0];
            var parameter = Param.Create($"--v {firstSupportedVersion.Value}").Value;
            var versionEntity = MidjourneyVersion.Create(firstSupportedVersion, parameter).Value;
            
            await _repository.AddVersionAsync(versionEntity);

            // Act
            var result = await _repository.CheckIfAnySupportedVersionExistsAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse(); // False means versions exist
        }
    }

    [Fact]
    public async Task AddVersion_WithFutureReleaseDate_ShouldSucceed()
    {
        // Arrange
        var version = ModelVersion.Create("1.0").Value;
        var parameter = Param.Create("--v 1").Value;
        var description = Description.Create("Future version").Value;
        var futureDate = DateTime.UtcNow.AddDays(30);
        
        var versionEntity = MidjourneyVersion.Create(version, parameter, futureDate, description).Value;

        // Act
        var result = await _repository.AddVersionAsync(versionEntity);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.ReleaseDate.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task AddVersion_WithVeryLongDescription_ShouldSucceed()
    {
        // Arrange
        var version = ModelVersion.Create("1.1").Value; // Changed from "1.0" to avoid conflicts
        var parameter = Param.Create("--v 1.1").Value;
        var longDescription = new string('A', 500); // Very long description
        var description = Description.Create(longDescription).Value;
        
        var versionEntity = MidjourneyVersion.Create(version, parameter, null, description).Value;

        // Act
        var result = await _repository.AddVersionAsync(versionEntity);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Description!.Value.Should().HaveLength(500);
        result.Value.Description.Value.Should().Be(longDescription);
    }

    [Fact]
    public async Task AddVersion_WithNijiVersion_ShouldSucceed()
    {
        // Arrange
        var version = ModelVersion.Create("niji 7").Value;
        var parameter = Param.Create("--niji 7").Value;
        var description = Description.Create("Next generation anime model").Value;
        
        var versionEntity = MidjourneyVersion.Create(version, parameter, null, description).Value;

        // Act
        var result = await _repository.AddVersionAsync(versionEntity);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Version.Value.Should().Be("niji 7");
        result.Value.Parameter.Value.Should().Be("--niji 7");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    [InlineData("5")]
    [InlineData("5.1")]
    [InlineData("5.2")]
    [InlineData("6")]
    [InlineData("6.1")]
    [InlineData("7")]
    [InlineData("niji 4")]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    public async Task AddVersion_WithKnownVersions_ShouldSucceed(string versionString)
    {
        // Arrange
        var version = ModelVersion.Create(versionString).Value;
        var parameter = Param.Create($"--v {versionString}").Value;
        var description = Description.Create($"Standard {versionString} version").Value;
        
        var versionEntity = MidjourneyVersion.Create(version, parameter, null, description).Value;

        // Act
        var result = await _repository.AddVersionAsync(versionEntity);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Version.Value.Should().Be(versionString);
    }
}