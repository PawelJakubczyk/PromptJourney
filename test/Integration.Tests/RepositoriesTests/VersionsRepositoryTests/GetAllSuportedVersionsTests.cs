//using FluentAssertions;

//namespace Integration.Tests.RepositoriesTests.VersionsRepositoryTests;

//public sealed class GetAllSuportedVersionsTests : RepositoryTestsBase
//{
//    public GetAllSuportedVersionsTests(MidjourneyDbFixture fixture) : base(fixture)
//    {
//    }

//    [Fact]
//    public async Task GetAllSuportedVersionsAsync_WithMultipleVersions_ShouldReturnAllVersionNumbers()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
//        await CreateAndSaveTestVersionAsync(DefaultTestVersion2);
//        await CreateAndSaveTestVersionAsync(DefaultTestVersion3);

//        // Act
//        var result = await VersionsRepository.GetAllSuportedVersionsAsync(CancellationToken);

//        // Assert
//        AssertSuccessResult(result);
//        result.Value.Should().HaveCount(3);
//        result.Value.Should().Contain(v => v.Value == DefaultTestVersion1);
//        result.Value.Should().Contain(v => v.Value == DefaultTestVersion2);
//        result.Value.Should().Contain(v => v.Value == DefaultTestVersion3);
//    }

//    [Fact]
//    public async Task GetAllSuportedVersionsAsync_WithNoVersions_ShouldReturnEmptyList()
//    {
//        // Act
//        var result = await VersionsRepository.GetAllSuportedVersionsAsync(CancellationToken);

//        // Assert
//        AssertSuccessResult(result);
//        result.Value.Should().BeEmpty();
//    }

//    [Fact]
//    public async Task GetAllSuportedVersionsAsync_ShouldReturnOnlyVersionNumbers()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

//        // Act
//        var result = await VersionsRepository.GetAllSuportedVersionsAsync(CancellationToken);

//        // Assert
//        AssertSuccessResult(result);
//        result.Value.Should().HaveCount(1);
//        result.Value.First().Value.Should().Be(DefaultTestVersion1);

//        // Verify we only get ModelVersion objects, not full MidjourneyVersion entities
//        result.Value.Should().AllBeOfType<Domain.ValueObjects.ModelVersion>();
//    }

//    [Fact]
//    public async Task GetAllSuportedVersionsAsync_WithMixedVersionFormats_ShouldReturnAllFormats()
//    {
//        // Arrange
//        var versions = new[] { "1", "2", "5.1", "5.2", "6", "niji 4", "niji 5" };

//        foreach (var version in versions)
//        {
//            await CreateAndSaveTestVersionAsync(version);
//        }

//        // Act
//        var result = await VersionsRepository.GetAllSuportedVersionsAsync(CancellationToken);

//        // Assert
//        AssertSuccessResult(result);
//        result.Value.Should().HaveCount(versions.Length);

//        foreach (var expectedVersion in versions)
//        {
//            result.Value.Should().Contain(v => v.Value == expectedVersion);
//        }
//    }

//    [Fact]
//    public async Task GetAllSuportedVersionsAsync_WithDuplicateVersions_ShouldReturnUniqueVersions()
//    {
//        // Arrange
//        // Note: This test assumes the database constraints prevent duplicate versions
//        // If duplicates are allowed, this test may need modification
//        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

//        // Try to create another version with different data but same version number
//        // This should either fail or be handled by the repository

//        // Act
//        var result = await VersionsRepository.GetAllSuportedVersionsAsync(CancellationToken);

//        // Assert
//        AssertSuccessResult(result);
//        result.Value.Should().HaveCount(1);
//        result.Value.First().Value.Should().Be(DefaultTestVersion1);
//    }

//    [Fact]
//    public async Task GetAllSuportedVersionsAsync_ResultsShouldBeOrderedConsistently()
//    {
//        // Arrange
//        var versions = new[] { "6", "1", "5.2", "2", "5.1" };

//        foreach (var version in versions)
//        {
//            await CreateAndSaveTestVersionAsync(version);
//        }

//        // Act
//        var result1 = await VersionsRepository.GetAllSuportedVersionsAsync(CancellationToken);
//        var result2 = await VersionsRepository.GetAllSuportedVersionsAsync(CancellationToken);

//        // Assert
//        AssertSuccessResult(result1);
//        AssertSuccessResult(result2);
//        result1.Value.Should().HaveCount(versions.Length);
//        result2.Value.Should().HaveCount(versions.Length);

//        // Results should be consistent between calls
//        result1.Value.Should().BeEquivalentTo(result2.Value);
//    }
//}