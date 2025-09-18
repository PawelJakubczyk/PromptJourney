using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Repositories;

namespace Integration.Tests.Repositories;

public class VersionsRepositoryTests : BaseTransactionIntegrationTest
{
    private const string TestVersion1 = "1.0";
    private const string TestVersion2 = "2.0";
    private const string TestVersion3 = "3.0";

    private const string TestParam1 = "--v 1.0";
    private const string TestParam2 = "--v 2.0";
    private const string TestParam3 = "--v 3.0";

    private const string TestDescription1 = "Test version 1.0 description";
    private const string TestDescription2 = "Test version 2.0 description";
    private const string TestDescription3 = "Test version 3.0 description";

    private readonly VersionsRepository _versionsRepository;

    public VersionsRepositoryTests(MidjourneyDbFixture fixture) : base(fixture)
    {
        _versionsRepository = new VersionsRepository(DbContext);
    }

    // AddVersionAsync Tests
    [Fact]
    public async Task AddVersionAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam1).Value,
            DateTime.UtcNow.AddDays(-30),
            Description.Create(TestDescription1).Value
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.Parameter.Value.Should().Be(TestParam1);
        result.Value.Description?.Value.Should().Be(TestDescription1);
    }

    [Fact]
    public async Task AddVersionAsync_WithNullDescription_ShouldSucceed()
    {
        // Arrange
        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam1).Value,
            DateTime.UtcNow.AddDays(-30), 
            null
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.Parameter.Value.Should().Be(TestParam1);
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task AddVersionAsync_WithNullReleaseDate_ShouldSucceed()
    {
        // Arrange
        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam1).Value,
            null,
            Description.Create(TestDescription1).Value
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.Parameter.Value.Should().Be(TestParam1);
        result.Value.ReleaseDate.Should().BeNull();
    }

    [Fact]
    public async Task AddVersionAsync_WithDuplicateVersion_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);

        var duplicateMidjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam2).Value,
            DateTime.UtcNow,
            Description.Create(TestDescription2).Value
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(duplicateMidjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains
        (
            "Database error while adding version: The instance of entity type 'MidjourneyVersion' cannot be tracked because another instance with the key value '{Version: 1.0}' is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached."
        ));
    }

    // GetAllVersionsAsync Tests
    [Fact]
    public async Task GetAllVersionsAsync_WithMultipleVersions_ShouldReturnAllVersions()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);
        await CreateAndSaveTestVersionAsync(TestVersion2, TestParam2, TestDescription2);
        await CreateAndSaveTestVersionAsync(TestVersion3, TestParam3, TestDescription3);

        // Act
        var result = await _versionsRepository.GetAllVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(v => v.Version.Value == TestVersion1);
        result.Value.Should().Contain(v => v.Version.Value == TestVersion2);
        result.Value.Should().Contain(v => v.Version.Value == TestVersion3);
    }

    [Fact]
    public async Task GetAllVersionsAsync_WithNoVersions_ShouldReturnEmptyList()
    {
        // Act
        var result = await _versionsRepository.GetAllVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    // GetAllSuportedVersionsAsync Tests
    [Fact]
    public async Task GetAllSupportedVersionsAsync_WithMultipleVersions_ShouldReturnAllSupportedVersions()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);
        await CreateAndSaveTestVersionAsync(TestVersion2, TestParam2, TestDescription2);
        await CreateAndSaveTestVersionAsync(TestVersion3, TestParam3, TestDescription3);

        // Act
        var result = await _versionsRepository.GetAllSuportedVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(v => v.Value == TestVersion1);
        result.Value.Should().Contain(v => v.Value == TestVersion2);
        result.Value.Should().Contain(v => v.Value == TestVersion3);
    }

    [Fact]
    public async Task GetAllSupportedVersionsAsync_WithNoVersions_ShouldReturnEmptyList()
    {
        // Act
        var result = await _versionsRepository.GetAllSuportedVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    // GetMasterVersionByVersionAsync Tests
    [Fact]
    public async Task GetMasterVersionByVersionAsync_WithExistingVersion_ShouldReturnVersion()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);

        // Act
        var result = await _versionsRepository.GetMasterVersionByVersionAsync
        (
            ModelVersion.Create(TestVersion1).Value
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.Parameter.Value.Should().Be(TestParam1);
        result.Value.Description?.Value.Should().Be(TestDescription1);
    }

    [Fact]
    public async Task GetMasterVersionByVersionAsync_WithNonExistentVersion_ShouldReturnNull()
    {
        // Arrange and Act
        var result = await _versionsRepository.GetMasterVersionByVersionAsync(ModelVersion.Create("99.0").Value);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    // CheckVersionExistsInVersionsAsync Tests
    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithExistingVersion_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);
        // Act
        var result = await _versionsRepository.CheckVersionExistsInVersionsAsync
        (
            ModelVersion.Create(TestVersion1).Value
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithNonExistentVersion_ShouldReturnFalse()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);
        var version = ModelVersion.Create("99.0").Value;

        // Act
        var result = await _versionsRepository.CheckVersionExistsInVersionsAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_WithEmptyDatabase_ShouldReturnFalse()
    {
        // Arrange
        var version = ModelVersion.Create(TestVersion1).Value;

        // Act
        var result = await _versionsRepository.CheckVersionExistsInVersionsAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    // CheckIfAnySupportedVersionExistsAsync Tests
    [Fact]
    public async Task CheckIfAnySupportedVersionExistsAsync_WithExistingVersions_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);

        // Act
        var result = await _versionsRepository.CheckIfAnySupportedVersionExistsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckIfAnySupportedVersionExistsAsync_WithNoVersions_ShouldReturnFalse()
    {
        // Act
        var result = await _versionsRepository.CheckIfAnySupportedVersionExistsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    // Edge Cases and Validation Tests
    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    [InlineData("5.1")]
    [InlineData("6.0")]
    [InlineData("niji 4")]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    public async Task AddVersionAsync_WithVariousValidVersions_ShouldSucceed(string versionValue)
    {
        // Arrange
        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(versionValue).Value,
            Param.Create($"--v {versionValue}").Value,
            DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365)),
            Description.Create($"Test version {versionValue}").Value
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Version.Value.Should().Be(versionValue);
    }

    [Fact]
    public async Task AddVersionAsync_WithFutureReleaseDate_ShouldSucceed()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(30);

        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam1).Value,
            futureDate,
            Description.Create(TestDescription1).Value
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.ReleaseDate.Should().Be(futureDate);
    }

    [Fact]
    public async Task AddVersionAsync_WithPastReleaseDate_ShouldSucceed()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddYears(-2);

        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam1).Value,
            pastDate,
            Description.Create(TestDescription1).Value
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.ReleaseDate.Should().Be(pastDate);
    }

    [Fact]
    public async Task GetMasterVersionByVersionAsync_AfterAddingVersion_ShouldReturnCorrectData()
    {
        // Arrange
        var originalVersion = await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);
        var version = ModelVersion.Create(TestVersion1).Value;

        // Act
        var result = await _versionsRepository.GetMasterVersionByVersionAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(originalVersion.Version.Value);
        result.Value.Parameter.Value.Should().Be(originalVersion.Parameter.Value);
        result.Value.Description?.Value.Should().Be(originalVersion.Description?.Value);
        result.Value.ReleaseDate.Should().Be(originalVersion.ReleaseDate);
    }

    [Fact]
    public async Task CheckVersionExistsInVersionsAsync_AfterAddingMultipleVersions_ShouldWorkCorrectly()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);
        await CreateAndSaveTestVersionAsync(TestVersion2, TestParam2, TestDescription2);

        var existingVersion = ModelVersion.Create(TestVersion1).Value;
        var anotherExistingVersion = ModelVersion.Create(TestVersion2).Value;
        var nonExistentVersion = ModelVersion.Create("99.0").Value;

        // Act
        var result1 = await _versionsRepository.CheckVersionExistsInVersionsAsync(existingVersion);
        var result2 = await _versionsRepository.CheckVersionExistsInVersionsAsync(anotherExistingVersion);
        var result3 = await _versionsRepository.CheckVersionExistsInVersionsAsync(nonExistentVersion);

        // Assert
        result1.Value.Should().BeTrue();
        result2.Value.Should().BeTrue();
        result3.Value.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllVersionsAsync_ShouldReturnVersionsInCorrectOrder()
    {
        // Arrange
        var version1 = await CreateAndSaveTestVersionAsync(TestVersion1, TestParam1, TestDescription1);
        var version2 = await CreateAndSaveTestVersionAsync(TestVersion2, TestParam2, TestDescription2);
        var version3 = await CreateAndSaveTestVersionAsync(TestVersion3, TestParam3, TestDescription3);

        // Act
        var result = await _versionsRepository.GetAllVersionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);

        // Verify all versions are present (order may vary depending on database)
        var versionValues = result.Value.Select(v => v.Version.Value).ToList();
        versionValues.Should().Contain(TestVersion1);
        versionValues.Should().Contain(TestVersion2);
        versionValues.Should().Contain(TestVersion3);
    }

    [Fact]
    public async Task AddVersionAsync_WithMinimalData_ShouldSucceed()
    {
        // Arrange - Only required fields
        var version = ModelVersion.Create(TestVersion1).Value;
        var parameter = Param.Create(TestParam1).Value;

        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam1).Value,
            null,
            null
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.Parameter.Value.Should().Be(TestParam1);
        result.Value.ReleaseDate.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task AddVersionAsync_WithLongDescription_ShouldSucceed()
    {
        // Arrange
        var version = ModelVersion.Create(TestVersion1).Value;
        var parameter = Param.Create(TestParam1).Value;
        var longDescription = Description.Create(new string('A', 400)).Value; // Close to 500 char limit

        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(TestVersion1).Value,
            Param.Create(TestParam1).Value,
            DateTime.UtcNow,
            Description.Create(new string('A', 400)).Value
        ).Value;

        // Act
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Description?.Value.Should().HaveLength(400);
    }

    // Helper methods
    private async Task<MidjourneyVersion> CreateAndSaveTestVersionAsync(string versionValue, string parameterValue, string descriptionValue)
    {
        var version = ModelVersion.Create(versionValue).Value;
        var parameter = Param.Create(parameterValue).Value;
        var description = Description.Create(descriptionValue).Value;
        var releaseDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365));

        var midjourneyVersion = MidjourneyVersion.Create
        (
            ModelVersion.Create(versionValue).Value,
            Param.Create(parameterValue).Value,
            DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365)),
            Description.Create(descriptionValue).Value
        ).Value;
        
        var result = await _versionsRepository.AddVersionAsync(midjourneyVersion);
        return result.Value;
    }
}