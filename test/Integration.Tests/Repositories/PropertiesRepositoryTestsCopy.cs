//using Domain.Entities;
//using Domain.ValueObjects;
//using FluentAssertions;
//using FluentResults;
//using Persistence.Repositories;
//using System.Threading;

//namespace Integration.Tests.Repositories;

//public class PropertiesRepositoryTestsCopy : BaseTransactionIntegrationTest
//{
//    private Result<ModelVersion> TestVersion1 = ModelVersion.Create("1");
//    private Result<ModelVersion> TestVersion2 = ModelVersion.Create("2");
//    private Result<ModelVersion> TestVersion3 = ModelVersion.Create("5.1");
//    private Result<ModelVersion> TestVersion4 = ModelVersion.Create("niji 4");

//    private PropertyName TestPropertyName1 = PropertyName.Create("aspect").Value;
//    private string TestPropertyName2 = "quality";
//    private string TestPropertyName3 = "stylize";

//    private List<Result<Param>> TestParam1 = [Param.Create("--ar"), Param.Create("--ar2")];
//    private string TestParam2 = "--q";
//    private string TestParam3 = "--s";

//    private DefaultValue TestDefaultValue1 = DefaultValue.Create("1:1").Value;
//    private string TestDefaultValue2 = "1";
//    private string TestDefaultValue3 = "100";

//    private MinValue TestMinValue1 = MinValue.Create("1:3").Value;
//    private string TestMinValue2 = "0.25";
//    private string TestMinValue3 = "0";

//    private MaxValue TestMaxValue1 = MaxValue.Create("3:1").Value;
//    private string TestMaxValue2 = "2";
//    private string TestMaxValue3 = "1000";

//    private Description TestDescription1 = Description.Create("Aspect ratio parameter").Value;
//    private string TestDescription2 = "Quality parameter";
//    private string TestDescription3 = "Stylize parameter";

//    private readonly PropertiesRepository _propertiesRepository;
//    private readonly VersionsRepository _versionsRepository;

//    private readonly CancellationToken _cancellationToken;

//    public PropertiesRepositoryTestsCopy(MidjourneyDbFixture fixture) : base(fixture)
//    {
//        _propertiesRepository = new PropertiesRepository(DbContext);
//        _versionsRepository = new VersionsRepository(DbContext);
//    }

//    // AddParameterToVersionAsync Tests
//    [Fact]
//    public async Task AddParameterToVersionAsync_WithValidData_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1.Value);

//        var property = await CreateTestPropertyAsync(
//            TestVersion1, 
//            TestPropertyName1, 
//            TestParam1, 
//            TestDefaultValue1, 
//            TestMinValue1, 
//            TestMaxValue1, 
//            TestDescription1);

//        // Act
//        var result = await _propertiesRepository.AddParameterToVersionAsync(property, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.PropertyName.Value.Should().Be(TestPropertyName1.Value);
//        result.Value.Version.Value.Should().Be(TestVersion1.Value);
//        result.Value.DefaultValue?.Value.Should().Be(TestDefaultValue1.Value);
//        result.Value.MinValue?.Value.Should().Be(TestMinValue1.Value);
//        result.Value.MaxValue?.Value.Should().Be(TestMaxValue1.Value);
//        result.Value.Description?.Value.Should().Be(TestDescription1.Value);
//    }

//    [Fact]
//    public async Task AddParameterToVersionAsync_WithMinimalData_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);

//        var property = await CreateTestPropertyAsync(
//            TestVersion1, 
//            TestPropertyName1, 
//            [TestParam1], 
//            null, 
//            null, 
//            null, 
//            null);

//        // Act
//        var result = await _propertiesRepository.AddParameterToVersionAsync(property, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.PropertyName.Value.Should().Be(TestPropertyName1);
//        result.Value.Version.Value.Should().Be(TestVersion1);
//        result.Value.DefaultValue.Should().BeNull();
//        result.Value.MinValue.Should().BeNull();
//        result.Value.MaxValue.Should().BeNull();
//        result.Value.Description.Should().BeNull();
//    }

//    [Fact]
//    public async Task AddParameterToVersionAsync_WithDuplicateProperty_ShouldFail()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

//        var duplicateProperty = await CreateTestPropertyAsync(
//            TestVersion1, 
//            TestPropertyName1, 
//            [TestParam1], 
//            TestDefaultValue1, 
//            TestMinValue1, 
//            TestMaxValue1, 
//            TestDescription1);

//        // Act
//        var result = await _propertiesRepository.AddParameterToVersionAsync(duplicateProperty, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeFalse();
//        result.Errors.Should().NotBeEmpty();
//        result.Errors.Should().Contain(e => e.Message.Contains($"Parameter '{TestPropertyName1}' already exists"));
//    }

//    [Fact]
//    public async Task AddParameterToVersionAsync_WithNonExistentVersion_ShouldFail()
//    {
//        // Arrange
//        var property = await CreateTestPropertyAsync(
//            "99.0", 
//            TestPropertyName1, 
//            [TestParam1], 
//            TestDefaultValue1, 
//            TestMinValue1, 
//            TestMaxValue1, 
//            TestDescription1);

//        // Act
//        var result = await _propertiesRepository.AddParameterToVersionAsync(property, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeFalse();
//        result.Errors.Should().NotBeEmpty();
//        result.Errors.Should().Contain(e => e.Message.Contains("Version master '99.0' not found"));
//    }

//    [Fact]
//    public async Task AddParameterToVersionAsync_WithMultipleParameters_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);

//        var property = await CreateTestPropertyAsync(
//            TestVersion1, 
//            TestPropertyName1, 
//            [TestParam1, "--aspect", "--a"], 
//            TestDefaultValue1, 
//            TestMinValue1, 
//            TestMaxValue1, 
//            TestDescription1);

//        // Act
//        var result = await _propertiesRepository.AddParameterToVersionAsync(property, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Parameters.Should().HaveCount(3);
//        result.Value.Parameters.Should().Contain(p => p.Value == TestParam1);
//        result.Value.Parameters.Should().Contain(p => p.Value == "--aspect");
//        result.Value.Parameters.Should().Contain(p => p.Value == "--a");
//    }

//    // GetAllParametersByVersionAsync Tests
//    [Fact]
//    public async Task GetAllParametersByVersionAsync_WithExistingVersion_ShouldReturnParameters()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName2, [TestParam2]);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName3, [TestParam3]);

//        var version = ModelVersion.Create(TestVersion1).Value;

//        // Act
//        var result = await _propertiesRepository.GetAllParametersByVersionAsync(version, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.Should().HaveCount(3);
//        result.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName1);
//        result.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName2);
//        result.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName3);
//    }

//    [Fact]
//    public async Task GetAllParametersByVersionAsync_WithNonExistentVersion_ShouldReturnEmptyList()
//    {
//        // Arrange
//        var version = ModelVersion.Create("99.0").Value;

//        // Act
//        var result = await _propertiesRepository.GetAllParametersByVersionAsync(version, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.Should().BeEmpty();
//    }

//    [Fact]
//    public async Task GetAllParametersByVersionAsync_WithEmptyVersion_ShouldReturnEmptyList()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        var version = ModelVersion.Create(TestVersion1).Value;

//        // Act
//        var result = await _propertiesRepository.GetAllParametersByVersionAsync(version, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.Should().BeEmpty();
//    }

//    // CheckParameterExistsInVersionAsync Tests
//    [Fact]
//    public async Task CheckParameterExistsInVersionAsync_WithExistingParameter_ShouldReturnTrue()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;

//        // Act
//        var result = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().BeTrue();
//    }

//    [Fact]
//    public async Task CheckParameterExistsInVersionAsync_WithNonExistentParameter_ShouldReturnFalse()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create("NonExistentProperty").Value;

//        // Act
//        var result = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().BeFalse();
//    }

//    [Fact]
//    public async Task CheckParameterExistsInVersionAsync_WithNonExistentVersion_ShouldReturnFalse()
//    {
//        // Arrange
//        var version = ModelVersion.Create("99.0").Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;

//        // Act
//        var result = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().BeFalse();
//    }

//    // UpdateParameterForVersionAsync Tests
//    [Fact]
//    public async Task UpdateParameterForVersionAsync_WithValidData_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

//        var updatedProperty = await CreateTestPropertyAsync(
//            TestVersion1, 
//            TestPropertyName1, 
//            [TestParam1, "--updated"], 
//            "UpdatedDefault", 
//            "UpdatedMin", 
//            "UpdatedMax", 
//            "Updated description");

//        // Act
//        var result = await _propertiesRepository.UpdateParameterForVersionAsync(updatedProperty, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.DefaultValue?.Value.Should().Be("UpdatedDefault");
//        result.Value.MinValue?.Value.Should().Be("UpdatedMin");
//        result.Value.MaxValue?.Value.Should().Be("UpdatedMax");
//        result.Value.Description?.Value.Should().Be("Updated description");
//        result.Value.Parameters.Should().HaveCount(2);
//    }

//    [Fact]
//    public async Task UpdateParameterForVersionAsync_WithNonExistentParameter_ShouldFail()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);

//        var property = await CreateTestPropertyAsync(
//            TestVersion1, 
//            "NonExistentProperty", 
//            [TestParam1], 
//            TestDefaultValue1, 
//            TestMinValue1, 
//            TestMaxValue1, 
//            TestDescription1);

//        // Act
//        var result = await _propertiesRepository.UpdateParameterForVersionAsync(property, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeFalse();
//        result.Errors.Should().NotBeEmpty();
//        result.Errors.Should().Contain(e => e.Message.Contains("Parameter 'NonExistentProperty' not found"));
//    }

//    // PatchParameterForVersionAsync Tests
//    [Fact]
//    public async Task PatchParameterForVersionAsync_UpdateDefaultValue_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;

//        // Act
//        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "defaultvalue", "NewDefault", _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.DefaultValue?.Value.Should().Be("NewDefault");
//    }

//    [Fact]
//    public async Task PatchParameterForVersionAsync_UpdateDescription_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;

//        // Act
//        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "description", "New description", _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.Description?.Value.Should().Be("New description");
//    }

//    [Theory]
//    [InlineData("defaultvalue", "NewDefault")]
//    [InlineData("minvalue", "NewMin")]
//    [InlineData("maxvalue", "NewMax")]
//    [InlineData("description", "New description")]
//    [InlineData("parameters", "--new,--param")]
//    public async Task PatchParameterForVersionAsync_WithVariousProperties_ShouldSucceed(string propertyToUpdate, string newValue)
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;

//        // Act
//        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, propertyToUpdate, newValue, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//    }

//    [Fact]
//    public async Task PatchParameterForVersionAsync_WithNonExistentParameter_ShouldFail()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create("NonExistentProperty").Value;

//        // Act
//        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "defaultvalue", "NewValue", _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeFalse();
//        result.Errors.Should().NotBeEmpty();
//        result.Errors.Should().Contain(e => e.Message.Contains("Parameter 'NonExistentProperty' not found"));
//    }

//    // DeleteParameterInVersionAsync Tests
//    [Fact]
//    public async Task DeleteParameterInVersionAsync_WithExistingParameter_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;

//        // Act
//        var result = await _propertiesRepository.DeleteParameterInVersionAsync(version, propertyName, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.PropertyName.Value.Should().Be(TestPropertyName1);

//        // Verify deletion
//        var checkResult = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName, _cancellationToken);
//        checkResult.Value.Should().BeFalse();
//    }

//    [Fact]
//    public async Task DeleteParameterInVersionAsync_WithNonExistentParameter_ShouldFail()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create("NonExistentProperty").Value;

//        // Act
//        var result = await _propertiesRepository.DeleteParameterInVersionAsync(version, propertyName, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeFalse();
//        result.Errors.Should().NotBeEmpty();
//        result.Errors.Should().Contain(e => e.Message.Contains("Parameter 'NonExistentProperty' not found"));
//    }

//    // Edge Cases and Integration Tests
//    [Theory]
//    [InlineData("1")]
//    [InlineData("2")]
//    [InlineData("5.1")]
//    [InlineData("6")]
//    [InlineData("niji 4")]
//    [InlineData("niji 5")]
//    public async Task AddParameterToVersionAsync_WithVariousVersions_ShouldSucceed(string versionValue)
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(versionValue);

//        var property = await CreateTestPropertyAsync(
//            versionValue,
//            TestPropertyName1, 
//            [TestParam1], 
//            TestDefaultValue1, 
//            TestMinValue1, 
//            TestMaxValue1, 
//            TestDescription1);

//        // Act
//        var result = await _propertiesRepository.AddParameterToVersionAsync(property, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Version.Value.Should().Be(versionValue);
//    }

//    [Fact]
//    public async Task GetAllParametersByVersionAsync_WithMultipleVersions_ShouldReturnCorrectParameters()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestVersionAsync(TestVersion2);

//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1]);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName2, [TestParam2]);
//        await CreateAndSaveTestPropertyAsync(TestVersion2, TestPropertyName3, [TestParam3]);

//        var version1 = ModelVersion.Create(TestVersion1).Value;
//        var version2 = ModelVersion.Create(TestVersion2).Value;

//        // Act
//        var result1 = await _propertiesRepository.GetAllParametersByVersionAsync(version1, _cancellationToken);
//        var result2 = await _propertiesRepository.GetAllParametersByVersionAsync(version2, _cancellationToken);

//        // Assert
//        result1.Value.Should().HaveCount(2);
//        result1.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName1);
//        result1.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName2);

//        result2.Value.Should().HaveCount(1);
//        result2.Value.Should().Contain(p => p.PropertyName.Value == TestPropertyName3);
//    }

//    [Fact]
//    public async Task PatchParameterForVersionAsync_SetToNull_ShouldSucceed()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);
//        await CreateAndSaveTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1], TestDefaultValue1, TestMinValue1, TestMaxValue1, TestDescription1);

//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;

//        // Act
//        var result = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "description", null, _cancellationToken);

//        // Assert
//        result.Should().NotBeNull();
//        result.IsSuccess.Should().BeTrue();
//        result.Value.Should().NotBeNull();
//        result.Value.Description.Should().BeNull();
//    }

//    [Fact]
//    public async Task CRUD_Operations_Integration_ShouldWorkCorrectly()
//    {
//        // Arrange
//        await CreateAndSaveTestVersionAsync(TestVersion1);

//        // Create
//        var property = await CreateTestPropertyAsync(TestVersion1, TestPropertyName1, [TestParam1], TestDefaultValue1, TestMinValue1, TestMaxValue1, TestDescription1);
//        var addResult = await _propertiesRepository.AddParameterToVersionAsync(property, _cancellationToken);
//        addResult.IsSuccess.Should().BeTrue();

//        // Read
//        var version = ModelVersion.Create(TestVersion1).Value;
//        var propertyName = PropertyName.Create(TestPropertyName1).Value;
//        var existsResult = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName, _cancellationToken);
//        existsResult.Value.Should().BeTrue();

//        // Update via Patch
//        var patchResult = await _propertiesRepository.PatchParameterForVersionAsync(version, propertyName, "description", "Updated description", _cancellationToken);
//        patchResult.IsSuccess.Should().BeTrue();
//        patchResult.Value.Description?.Value.Should().Be("Updated description");

//        // Delete
//        var deleteResult = await _propertiesRepository.DeleteParameterInVersionAsync(version, propertyName, _cancellationToken);
//        deleteResult.IsSuccess.Should().BeTrue();

//        // Verify deletion
//        var checkAfterDelete = await _propertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName, _cancellationToken);
//        checkAfterDelete.Value.Should().BeFalse();
//    }

//    // Helper methods
//    private async Task<MidjourneyVersion> CreateAndSaveTestVersionAsync(string versionValue)
//    {
//        var version = ModelVersion.Create(versionValue).Value;
//        var parameter = Param.Create($"--v {versionValue}").Value;
//        var description = Description.Create($"Test version {versionValue}").Value;

//        var versionEntity = MidjourneyVersion.Create(version, parameter, DateTime.UtcNow, description).Value;
//        var result = await _versionsRepository.AddVersionAsync(versionEntity, _cancellationToken);

//        return result.Value;
//    }

//    private async Task<MidjourneyPropertiesBase> CreateTestPropertyAsync(
//        ModelVersion version, 
//        PropertyName propertyName, 
//        List<Result<Param>> parameters, 
//        DefaultValue? defaultValue = null, 
//        MinValue? minValue = null, 
//        MaxValue? maxValue = null, 
//        Description? description = null)
//    {


//        var property = MidjourneyPropertiesBase.Create(
//            propertyName,
//            version,
//            parameters,
//            defaultValue,
//            minValue,
//            maxValue,
//            description).Value;

//        return property;
//    }

//    private async Task<MidjourneyPropertiesBase> CreateAndSaveTestPropertyAsync(
//        ModelVersion version, 
//        PropertyName propertyName,
//        List<Result<Param>> parameters, 
//        DefaultValue? defaultValue = null, 
//        MinValue? minValue = null, 
//        MaxValue? maxValue = null, 
//        Description? description = null)
//    {
//        var property = await CreateTestPropertyAsync(version, propertyName, parameters, defaultValue, minValue, maxValue, description);
//        var result = await _propertiesRepository.AddParameterToVersionAsync(property, _cancellationToken);
//        return result.Value;
//    }
//}