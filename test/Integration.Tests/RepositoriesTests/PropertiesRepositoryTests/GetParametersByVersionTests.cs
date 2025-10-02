using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PropertiesRepositoryTests;

public class GetParametersByVersionTests : RepositoryTestsBase
{

    public GetParametersByVersionTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllParametersByVersionAsync_WithExistingVersion_ShouldReturnParameters()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1]);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName2, [DefaultTestParam2]);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName3, [DefaultTestParam3]);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;

        // Act
        var result = await PropertiesRepository.GetAllParametersByVersionAsync(version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(p => p.PropertyName.Value == DefaultTestPropertyName1);
        result.Value.Should().Contain(p => p.PropertyName.Value == DefaultTestPropertyName2);
        result.Value.Should().Contain(p => p.PropertyName.Value == DefaultTestPropertyName3);
    }

    [Fact]
    public async Task GetAllParametersByVersionAsync_WithNonExistentVersion_ShouldReturnEmptyList()
    {
        // Arrange
        var version = ModelVersion.Create("99.0").Value;

        // Act
        var result = await PropertiesRepository.GetAllParametersByVersionAsync(version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllParametersByVersionAsync_WithMultipleVersions_ShouldReturnCorrectParameters()
    {
        // Arrange
        var versions = await CreateAndSaveMultipleVersionsAsync(DefaultTestVersion1, DefaultTestVersion2);

        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1]);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName2, [DefaultTestParam2]);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion2, DefaultTestPropertyName3, [DefaultTestParam3]);

        var version1 = ModelVersion.Create(DefaultTestVersion1).Value;
        var version2 = ModelVersion.Create(DefaultTestVersion2).Value;

        // Act
        var result1 = await PropertiesRepository.GetAllParametersByVersionAsync(version1, CancellationToken);
        var result2 = await PropertiesRepository.GetAllParametersByVersionAsync(version2, CancellationToken);

        // Assert
        result1.Value.Should().HaveCount(2);
        result1.Value.Should().Contain(p => p.PropertyName.Value == DefaultTestPropertyName1);
        result1.Value.Should().Contain(p => p.PropertyName.Value == DefaultTestPropertyName2);

        result2.Value.Should().HaveCount(1);
        result2.Value.Should().Contain(p => p.PropertyName.Value == DefaultTestPropertyName3);
    }
}