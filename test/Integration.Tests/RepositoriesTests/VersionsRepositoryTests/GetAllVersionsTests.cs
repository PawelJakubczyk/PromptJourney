using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.VersionsRepositoryTests;

public sealed class GetAllVersionsTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task GetAllVersionsAsync_WithMultipleVersions_ShouldReturnAllVersions()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestVersionAsync(DefaultTestVersion2);
        await CreateAndSaveTestVersionAsync(DefaultTestVersion3);

        // Act
        var result = await VersionsRepository.GetAllVersionsAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(v => v.Version.Value == DefaultTestVersion1);
        result.Value.Should().Contain(v => v.Version.Value == DefaultTestVersion2);
        result.Value.Should().Contain(v => v.Version.Value == DefaultTestVersion3);
    }

    [Fact]
    public async Task GetAllVersionsAsync_WithNoVersions_ShouldReturnEmptyList()
    {
        // Act
        var result = await VersionsRepository.GetAllVersionsAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllVersionsAsync_WithSingleVersion_ShouldReturnOneVersion()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        // Act
        var result = await VersionsRepository.GetAllVersionsAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
        result.Value.First().Version.Value.Should().Be(DefaultTestVersion1);
        result.Value.First().Parameter.Value.Should().Be($"--v {DefaultTestVersion1}");
        result.Value.First().Description!.Value.Should().Be($"Test version {DefaultTestVersion1}");
    }

    [Fact]
    public async Task GetAllVersionsAsync_WithMixedVersionTypes_ShouldReturnAllTypes()
    {
        // Arrange
        var versions = new[] { "1", "2", "3", "4", "5", "5.1", "5.2", "6", "niji 4", "niji 5", "niji 6" };

        foreach (var version in versions)
        {
            await CreateAndSaveTestVersionAsync(version);
        }

        // Act
        var result = await VersionsRepository.GetAllVersionsAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(versions.Length);

        foreach (var expectedVersion in versions)
        {
            result.Value.Should().Contain(v => v.Version.Value == expectedVersion);
        }
    }

    [Fact]
    public async Task GetAllVersionsAsync_ShouldReturnVersionsWithAllProperties()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        // Act
        var result = await VersionsRepository.GetAllVersionsAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);

        var version = result.Value.First();
        version.Version.Should().NotBeNull();
        version.Parameter.Should().NotBeNull();
        version.ReleaseDate.Should().NotBeNull();
        version.Description.Should().NotBeNull();
        version.Version.Value.Should().Be(DefaultTestVersion1);
    }
}