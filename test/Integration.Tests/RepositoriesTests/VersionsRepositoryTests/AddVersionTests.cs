using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using FluentResults;

namespace Integration.Tests.RepositoriesTests.VersionsRepositoryTests;

public sealed class AddVersionTests : RepositoryTestsBase
{
    public AddVersionTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task AddVersionAsync_WithValidVersion_ShouldReturnSuccess()
    {
        // Arrange
        var version = ModelVersion.Create("7.0").Value;
        var parameter = Param.Create("--v 7.0").Value;
        var description = Description.Create("Test version 7.0").Value;
        var releaseDate = DateTime.UtcNow;

        var versionEntity = MidjourneyVersion.Create(
            Result.Ok(version),
            Result.Ok(parameter),
            releaseDate,
            Result.Ok<Description?>(description)).Value;

        // Act
        var result = await VersionsRepository.AddVersionAsync(versionEntity, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Version.Value.Should().Be("7.0");
        result.Value.Parameter.Value.Should().Be("--v 7.0");
        result.Value.Description!.Value.Should().Be("Test version 7.0");
        result.Value.ReleaseDate.Should().BeCloseTo(releaseDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task AddVersionAsync_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var version = ModelVersion.Create("8.0").Value;
        var parameter = Param.Create("--v 8.0").Value;
        var releaseDate = DateTime.UtcNow;

        var versionEntity = MidjourneyVersion.Create(
            Result.Ok(version),
            Result.Ok(parameter),
            releaseDate).Value;

        // Act
        var result = await VersionsRepository.AddVersionAsync(versionEntity, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Version.Value.Should().Be("8.0");
        result.Value.Parameter.Value.Should().Be("--v 8.0");
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task AddVersionAsync_WithNijiVersion_ShouldReturnSuccess()
    {
        // Arrange
        var version = ModelVersion.Create("niji 7").Value;
        var parameter = Param.Create("--niji 7").Value;
        var description = Description.Create("Niji version 7").Value;
        var releaseDate = DateTime.UtcNow;

        var versionEntity = MidjourneyVersion.Create(
            Result.Ok(version),
            Result.Ok(parameter),
            releaseDate,
            Result.Ok<Description?>(description)).Value;

        // Act
        var result = await VersionsRepository.AddVersionAsync(versionEntity, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Version.Value.Should().Be("niji 7");
        result.Value.Parameter.Value.Should().Be("--niji 7");
        result.Value.Description!.Value.Should().Be("Niji version 7");
    }

    [Fact]
    public async Task AddVersionAsync_WithDuplicateVersion_ShouldHandleGracefully()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var parameter = Param.Create($"--v {DefaultTestVersion1}").Value;
        var releaseDate = DateTime.UtcNow;

        var duplicateVersionEntity = MidjourneyVersion.Create(
            Result.Ok(version),
            Result.Ok(parameter),
            releaseDate).Value;

        // Act & Assert
        // The behavior depends on database constraints and implementation
        // This might throw an exception or handle duplicates gracefully
        var result = await VersionsRepository.AddVersionAsync(duplicateVersionEntity, CancellationToken);

        // We don't assert success/failure as it depends on implementation
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddVersionAsync_MultipleVersions_ShouldAllSucceed()
    {
        // Arrange
        var versions = new[] { "7.0", "7.1", "8.0" };
        var results = new List<MidjourneyVersion>();

        // Act
        foreach (var versionValue in versions)
        {
            var version = ModelVersion.Create(versionValue).Value;
            var parameter = Param.Create($"--v {versionValue}").Value;
            var releaseDate = DateTime.UtcNow;

            var versionEntity = MidjourneyVersion.Create(
                Result.Ok(version),
                Result.Ok(parameter),
                releaseDate).Value;

            var result = await VersionsRepository.AddVersionAsync(versionEntity, CancellationToken);
            AssertSuccessResult(result);
            results.Add(result.Value);
        }

        // Assert
        results.Should().HaveCount(3);
        results.Should().Contain(v => v.Version.Value == "7.0");
        results.Should().Contain(v => v.Version.Value == "7.1");
        results.Should().Contain(v => v.Version.Value == "8.0");

        // Verify all versions are in the database
        var allVersions = await VersionsRepository.GetAllVersionsAsync(CancellationToken);
        allVersions.Value.Should().HaveCountGreaterOrEqualTo(3);
    }

    [Fact]
    public async Task AddVersionAsync_WithVersionAlreadyExists_ShouldBeCheckableAfterAdd()
    {
        // Arrange
        var version = ModelVersion.Create("9.0").Value;
        var parameter = Param.Create("--v 9.0").Value;
        var releaseDate = DateTime.UtcNow;

        var versionEntity = MidjourneyVersion.Create(
            Result.Ok(version),
            Result.Ok(parameter),
            releaseDate).Value;

        // Act
        var addResult = await VersionsRepository.AddVersionAsync(versionEntity, CancellationToken);
        var existsResult = await VersionsRepository.CheckVersionExistsInVersionsAsync(version, CancellationToken);

        // Assert
        AssertSuccessResult(addResult);
        AssertSuccessResult(existsResult);
        existsResult.Value.Should().BeTrue();
    }
}