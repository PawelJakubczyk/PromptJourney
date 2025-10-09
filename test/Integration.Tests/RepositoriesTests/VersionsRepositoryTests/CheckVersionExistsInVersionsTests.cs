using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.VersionsRepositoryTests;

public sealed class CheckVersionExistsInVersionsTests : RepositoryTestsBase
{
    public CheckVersionExistsInVersionsTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithExistingVersion_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        var version = ModelVersion.Create(DefaultTestVersion1).Value;

        // Act
        var result = await VersionsRepository.CheckVersionExists(version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithNonExistentVersion_ShouldReturnFalse()
    {
        // Arrange
        var version = ModelVersion.Create("99.0").Value;

        // Act
        var result = await VersionsRepository.CheckVersionExists(version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithEmptyDatabase_ShouldReturnFalse()
    {
        // Arrange
        var version = ModelVersion.Create(DefaultTestVersion1).Value;

        // Act
        var result = await VersionsRepository.CheckVersionExists(version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithMultipleVersions_ShouldReturnCorrectResult()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestVersionAsync(DefaultTestVersion2);

        var existingVersion = ModelVersion.Create(DefaultTestVersion1).Value;
        var nonExistingVersion = ModelVersion.Create(DefaultTestVersion3).Value;

        // Act & Assert
        var existingResult = await VersionsRepository.CheckVersionExists(existingVersion, CancellationToken);
        AssertSuccessResult(existingResult);
        existingResult.Value.Should().BeTrue();

        var nonExistingResult = await VersionsRepository.CheckVersionExists(nonExistingVersion, CancellationToken);
        AssertSuccessResult(nonExistingResult);
        nonExistingResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithCaseVariations_ShouldReturnCorrectResult()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync("5.2");

        var exactVersion = ModelVersion.Create("5.2").Value;
        var differentVersion = ModelVersion.Create("5.3").Value;

        // Act & Assert
        var exactResult = await VersionsRepository.CheckVersionExists(exactVersion, CancellationToken);
        AssertSuccessResult(exactResult);
        exactResult.Value.Should().BeTrue();

        var differentResult = await VersionsRepository.CheckVersionExists(differentVersion, CancellationToken);
        AssertSuccessResult(differentResult);
        differentResult.Value.Should().BeFalse();
    }
}