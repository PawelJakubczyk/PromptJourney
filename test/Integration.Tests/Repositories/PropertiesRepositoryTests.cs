using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyVersions;
using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Repositories;

namespace Integration.Tests.Repositories;

public class PropertiesRepositoryTests : BaseTransactionIntegrationTest
{
    private const string TestVersion1 = "1";
    private const string TestVersion2 = "2";
    private const string TestVersion3 = "5.1";
    private const string TestVersion4 = "niji 4";

    private const string TestPropertyName1 = "aspect";
    private const string TestPropertyName2 = "quality";
    private const string TestPropertyName3 = "stylize";

    private const string TestParam1 = "--ar";
    private const string TestParam2 = "--q";
    private const string TestParam3 = "--s";

    private const string TestDefaultValue1 = "1:1";
    private const string TestDefaultValue2 = "1";
    private const string TestDefaultValue3 = "100";

    private const string TestMinValue1 = "1:3";
    private const string TestMinValue2 = "0.25";
    private const string TestMinValue3 = "0";

    private const string TestMaxValue1 = "3:1";
    private const string TestMaxValue2 = "2";
    private const string TestMaxValue3 = "1000";

    private const string TestDescription1 = "Aspect ratio parameter";
    private const string TestDescription2 = "Quality parameter";
    private const string TestDescription3 = "Stylize parameter";

    private readonly PropertiesRepository _propertiesRepository;
    private readonly VersionsRepository _versionsRepository;

    public PropertiesRepositoryTests(MidjourneyDbFixture fixture) : base(fixture)
    {
        _propertiesRepository = new PropertiesRepository(DbContext);
        _versionsRepository = new VersionsRepository(DbContext);
    }

    // AddParameterToVersionAsync Tests
    [Fact]
    public async Task AddParameterToVersionAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        var property = await CreateTestPropertyAsync(
            TestVersion1, 
            TestPropertyName1, 
            [TestParam1], 
            TestDefaultValue1, 
            TestMinValue1, 
            TestMaxValue1, 
            TestDescription1);

        // Act
        var result = await _propertiesRepository.AddParameterToVersionAsync(property);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyName.Value.Should().Be(TestPropertyName1);
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.DefaultValue?.Value.Should().Be(TestDefaultValue1);
        result.Value.MinValue?.Value.Should().Be(TestMinValue1);
        result.Value.MaxValue?.Value.Should().Be(TestMaxValue1);
        result.Value.Description?.Value.Should().Be(TestDescription1);
    }

    [Fact]
    public async Task AddParameterToVersionAsync_WithMinimalData_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        var property = await CreateTestPropertyAsync(
            TestVersion1, 
            TestPropertyName1, 
            [TestParam1], 
            null, 
            null, 
            null, 
            null);

        // Act
        var result = await _propertiesRepository.AddParameterToVersionAsync(property);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyName.Value.Should().Be(TestPropertyName1);
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.DefaultValue.Should().BeNull();
        result.Value.MinValue.Should().BeNull();
        result.Value.MaxValue.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task AddParameterToVersionAsync_WithDuplicateProperty_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

        var duplicateProperty = await CreateTestPropertyAsync(
            TestVersion1, 
            TestPropertyName1, 
            [TestParam1], 
            TestDefaultValue1, 
            TestMinValue1, 
            TestMaxValue1, 
            TestDescription1);

        // Act
        var result = await _propertiesRepository.AddParameterToVersionAsync(duplicateProperty);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains($"Parameter '{TestPropertyName1}' already exists"));
    }

    [Fact]
    public async Task AddParameterToVersionAsync_WithNonExistentVersion_ShouldFail()
    {
        // Arrange
        var property = await CreateTestPropertyAsync(
            "99.0", 
            TestPropertyName1, 
            [TestParam1], 
            TestDefaultValue1, 
            TestMinValue1, 
            TestMaxValue1, 
            TestDescription1);

        // Act
        var result = await _propertiesRepository.AddParameterToVersionAsync(property);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Version master '99.0' not found"));
    }

    [Fact]
    public async Task AddParameterToVersionAsync_WithMultipleParameters_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        var property = await CreateTestPropertyAsync(
            TestVersion1, 
            TestPropertyName1, 
            [TestParam1, "--aspect", "--a"], 
            TestDefaultValue1, 
            TestMinValue1, 
            TestMaxValue1, 
            TestDescription1);

        // Act
        var result = await _propertiesRepository.AddParameterToVersionAsync(property);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Parameters.Should().HaveCount(3);
        result.Value.Parameters.Should().Contain(p => p.Value == TestParam1);
        result.Value.Parameters.Should().Contain(p => p.Value == "--aspect");
        result.Value.Parameters.Should().Contain(p => p.Value == "--a");
    }

    // GetAllParametersByVersionAsync Tests
    [Fact]
    public async Task GetAllParametersByVersionAsync_WithExistingVersion_ShouldReturnParameters()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName2, [TestParam2]);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName3, [TestParam3]);

        var version = ModelVersion.Create(TestVersion1).Value;

        // Act
        var result = await _propertiesRepository.GetAllParametersByVersionAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName1);
        result.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName2);
        result.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName3);
    }

    [Fact]
    public async Task GetAllParametersByVersionAsync_WithNonExistentVersion_ShouldReturnEmptyList()
    {
        // Arrange
        var version = ModelVersion.Create("99.0").Value;

        // Act
        var result = await _propertiesRepository.GetAllParametersByVersionAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllParametersByVersionAsync_WithEmptyVersion_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        var version = ModelVersion.Create(TestVersion1).Value;

        // Act
        var result = await _propertiesRepository.GetAllParametersByVersionAsync(version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    // CheckParameterExistsInVersionAsync Tests
    [Fact]
    public async Task CheckParameterExistsInVersionAsync_WithExistingParameter_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;

        // Act
        var result = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckParameterExistsInVersionAsync_WithNonExistentParameter_ShouldReturnFalse()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create("NonExistentProperty").Value;

        // Act
        var result = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckParameterExistsInVersionAsync_WithNonExistentVersion_ShouldReturnFalse()
    {
        // Arrange
        var version = ModelVersion.Create("99.0").Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;

        // Act
        var result = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    // UpdateParameterForVersionAsync Tests
    [Fact]
    public async Task UpdateParameterForVersionAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

        var updatedProperty = await CreateTestPropertyAsync(
            TestVersion1, 
            TestPropertyName1, 
            [TestParam1, "--updated"], 
            "UpdatedDefault", 
            "UpdatedMin", 
            "UpdatedMax", 
            "Updated description");

        // Act
        var result = await _propertiesRepository.UpdateParameterForVersionAsync(updatedProperty);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.DefaultValue?.Value.Should().Be("UpdatedDefault");
        result.Value.MinValue?.Value.Should().Be("UpdatedMin");
        result.Value.MaxValue?.Value.Should().Be("UpdatedMax");
        result.Value.Description?.Value.Should().Be("Updated description");
        result.Value.Parameters.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateParameterForVersionAsync_WithNonExistentParameter_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        var property = await CreateTestPropertyAsync(
            TestVersion1, 
            "NonExistentProperty", 
            [TestParam1], 
            TestDefaultValue1, 
            TestMinValue1, 
            TestMaxValue1, 
            TestDescription1);

        // Act
        var result = await _propertiesRepository.UpdateParameterForVersionAsync(property);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Parameter 'NonExistentProperty' not found"));
    }

    // PatchParameterForVersionAsync Tests
    [Fact]
    public async Task PatchParameterForVersionAsync_UpdateDefaultValue_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;

        // Act
        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "defaultvalue", "NewDefault");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.DefaultValue?.Value.Should().Be("NewDefault");
    }

    [Fact]
    public async Task PatchParameterForVersionAsync_UpdateDescription_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;

        // Act
        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "description", "New description");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description?.Value.Should().Be("New description");
    }

    [Theory]
    [InlineData("defaultvalue", "NewDefault")]
    [InlineData("minvalue", "NewMin")]
    [InlineData("maxvalue", "NewMax")]
    [InlineData("description", "New description")]
    [InlineData("parameters", "--new,--param")]
    public async Task PatchParameterForVersionAsync_WithVariousProperties_ShouldSucceed(string propertyToUpdate, string newValue)
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;

        // Act
        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, propertyToUpdate, newValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task PatchParameterForVersionAsync_WithNonExistentParameter_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create("NonExistentProperty").Value;

        // Act
        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "defaultvalue", "NewValue");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Parameter 'NonExistentProperty' not found"));
    }

    // DeleteParameterInVersionAsync Tests
    [Fact]
    public async Task DeleteParameterInVersionAsync_WithExistingParameter_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;

        // Act
        var result = await _propertiesRepository.DeleteParameterInVersionAsync(version, propertyName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyName.Value.Should().Be(TestPropertyName1);

        // Verify deletion
        var checkResult = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteParameterInVersionAsync_WithNonExistentParameter_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create("NonExistentProperty").Value;

        // Act
        var result = await _propertiesRepository.DeleteParameterInVersionAsync(version, propertyName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Parameter 'NonExistentProperty' not found"));
    }

    // Edge Cases and Integration Tests
    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("5.1")]
    [InlineData("6")]
    [InlineData("niji 4")]
    [InlineData("niji 5")]
    public async Task AddParameterToVersionAsync_WithVariousVersions_ShouldSucceed(string versionValue)
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(versionValue);

        var property = await CreateTestPropertyAsync(
            versionValue,
            TestPropertyName1, 
            [TestParam1], 
            TestDefaultValue1, 
            TestMinValue1, 
            TestMaxValue1, 
            TestDescription1);

        // Act
        var result = await _propertiesRepository.AddParameterToVersionAsync(property);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Version.Value.Should().Be(versionValue);
    }

    [Fact]
    public async Task GetAllParametersByVersionAsync_WithMultipleVersions_ShouldReturnCorrectParameters()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestVersionAsync(TestVersion2);

        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName2, [TestParam2]);
        await CreateAndSaveTestPropertyAsync(TestVersion2, TestPropertyName3, [TestParam3]);

        var version1 = ModelVersion.Create(TestVersion1).Value;
        var version2 = ModelVersion.Create(TestVersion2).Value;

        // Act
        var result1 = await _propertiesRepository.GetAllParametersByVersionAsync(version1);
        var result2 = await _propertiesRepository.GetAllParametersByVersionAsync(version2);

        // Assert
        result1.Value.Should().HaveCount(2);
        result1.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName1);
        result1.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName2);

        result2.Value.Should().HaveCount(1);
        result2.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName3);
    }

    [Fact]
    public async Task PatchParameterForVersionAsync_SetToNull_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1], TestDefaultValue1, TestMinValue1, TestMaxValue1, TestDescription1);

        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;

        // Act
        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "description", null);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task CRUD_Operations_Integration_ShouldWorkCorrectly()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);

        // Create
        var property = await CreateTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1], TestDefaultValue1, TestMinValue1, TestMaxValue1, TestDescription1);
        var addResult = await _propertiesRepository.AddParameterToVersionAsync(property);
        addResult.IsSuccess.Should().BeTrue();

        // Read
        var version = ModelVersion.Create(TestVersion1).Value;
        var propertyName = PropertyName.Create(TestPropertyName1).Value;
        var existsResult = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName);
        existsResult.Value.Should().BeTrue();

        // Update via Patch
        var patchResult = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "description", "Updated description");
        patchResult.IsSuccess.Should().BeTrue();
        patchResult.Value.Description?.Value.Should().Be("Updated description");

        // Delete
        var deleteResult = await _propertiesRepository.DeleteParameterInVersionAsync(version, propertyName);
        deleteResult.IsSuccess.Should().BeTrue();

        // Verify deletion
        var checkAfterDelete = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName);
        checkAfterDelete.Value.Should().BeFalse();
    }

    // Helper methods
    private async Task<MidjourneyVersion> CreateAndSaveTestVersionAsync(string versionValue)
    {
        var version = ModelVersion.Create(versionValue).Value;
        var parameter = Param.Create($"--v {versionValue}").Value;
        var description = Description.Create($"Test version {versionValue}").Value;

        var versionEntity = MidjourneyVersion.Create(version, parameter, DateTime.UtcNow, description).Value;
        var result = await _versionsRepository.AddVersionAsync(versionEntity);

        return result.Value;
    }

    private async Task<MidjourneyPropertiesBase> CreateTestPropertyAsync(
        string version, 
        string propertyName, 
        string[] parameters, 
        string? defaultValue = null, 
        string? minValue = null, 
        string? maxValue = null, 
        string? description = null)
    {
        var propertyNameVo = PropertyName.Create(propertyName).Value;
        var versionVo = ModelVersion.Create(version).Value;
        var parametersVo = parameters.Select(p => Param.Create(p)).ToList();
        var defaultValueVo = defaultValue != null ? DefaultValue.Create(defaultValue).Value : null;
        var minValueVo = minValue != null ? MinValue.Create(minValue).Value : null;
        var maxValueVo = maxValue != null ? MaxValue.Create(maxValue).Value : null;
        var descriptionVo = description != null ? Description.Create(description).Value : null;

        var property = MidjourneyPropertiesBase.Create(
            propertyNameVo,
            versionVo,
            parametersVo,
            defaultValueVo,
            minValueVo,
            maxValueVo,
            descriptionVo).Value;

        return property;
    }

    private async Task<MidjourneyPropertiesBase> CreateAndSaveTestPropertyAsync(
        string version, 
        string propertyName, 
        string[] parameters, 
        string? defaultValue = null, 
        string? minValue = null, 
        string? maxValue = null, 
        string? description = null)
    {
        var property = await CreateTestPropertyAsync(version, propertyName, parameters, defaultValue, minValue, maxValue, description);
        var result = await _propertiesRepository.AddParameterToVersionAsync(property);
        return result.Value;
    }
}