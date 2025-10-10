using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.VersionsRepositoryTests;

public sealed class GetMasterVersionByVersionTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task GetMasterVersionByVersionAsync_WithExistingVersion_ShouldReturnVersion()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        var version = ModelVersion.Create(DefaultTestVersion1).Value;

        // Act
        var result = await VersionsRepository.GetVersionAsync(version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Version.Value.Should().Be(DefaultTestVersion1);
        result.Value.Parameter.Value.Should().Be($"--v {DefaultTestVersion1}");
        result.Value.Description!.Value.Should().Be($"Test version {DefaultTestVersion1}");
    }

    [Fact]
    public async Task GetMasterVersionByVersionAsync_WithNonExistentVersion_ShouldReturnNull()
    {
        // Arrange
        var version = ModelVersion.Create("99.0").Value;

        // Act
        var result = await VersionsRepository.GetVersionAsync(version, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetMasterVersionByVersionAsync_WithMultipleVersions_ShouldReturnCorrectVersion()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestVersionAsync(DefaultTestVersion2);
        await CreateAndSaveTestVersionAsync(DefaultTestVersion3);

        var targetVersion = ModelVersion.Create(DefaultTestVersion2).Value;

        // Act
        var result = await VersionsRepository.GetVersionAsync(targetVersion, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Version.Value.Should().Be(DefaultTestVersion2);
        result.Value.Parameter.Value.Should().Be($"--v {DefaultTestVersion2}");
        result.Value.Description!.Value.Should().Be($"Test version {DefaultTestVersion2}");
    }

    [Fact]
    public async Task GetMasterVersionByVersionAsync_WithEmptyDatabase_ShouldReturnNull()
    {
        // Arrange
        var version = ModelVersion.Create(DefaultTestVersion1).Value;

        // Act
        var result = await VersionsRepository.GetVersionAsync(version, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetMasterVersionByVersionAsync_WithSpecialVersionFormats_ShouldWork()
    {
        // Arrange
        var specialVersions = new[] { "5.1", "5.2", "niji 5", "niji 6" };

        foreach (var versionValue in specialVersions)
        {
            await CreateAndSaveTestVersionAsync(versionValue);
        }

        var targetVersion = ModelVersion.Create("niji 5").Value;

        // Act
        var result = await VersionsRepository.GetVersionAsync(targetVersion, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Version.Value.Should().Be("niji 5");
        result.Value.Parameter.Value.Should().Be("--v niji 5");
    }
}