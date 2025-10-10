using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PropertiesRepositoryTests;

public class CheckParameterExistsTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task CheckParameterExistsInVersionAsync_WithExistingParameter_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1]);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var propertyName = PropertyName.Create(DefaultTestPropertyName1).Value;

        // Act
        var result = await PropertiesRepository.CheckPropertyExistsInVersionAsync(version, propertyName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckParameterExistsInVersionAsync_WithNonExistentParameter_ShouldReturnFalse()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var propertyName = PropertyName.Create("NonExistentProperty").Value;

        // Act
        var result = await PropertiesRepository.CheckPropertyExistsInVersionAsync(version, propertyName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckParameterExistsInVersionAsync_WithNonExistentVersion_ShouldReturnFalse()
    {
        // Arrange
        var version = ModelVersion.Create("99.0").Value;
        var propertyName = PropertyName.Create(DefaultTestPropertyName1).Value;

        // Act
        var result = await PropertiesRepository.CheckPropertyExistsInVersionAsync(version, propertyName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckParameterExistsInVersionAsync_WithMultipleProperties_ShouldWorkCorrectly()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1]);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName2, [DefaultTestParam2]);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var existingProperty = PropertyName.Create(DefaultTestPropertyName1).Value;
        var anotherExistingProperty = PropertyName.Create(DefaultTestPropertyName2).Value;
        var nonExistentProperty = PropertyName.Create("NonExistent").Value;

        // Act
        var result1 = await PropertiesRepository.CheckPropertyExistsInVersionAsync(version, existingProperty, CancellationToken);
        var result2 = await PropertiesRepository.CheckPropertyExistsInVersionAsync(version, anotherExistingProperty, CancellationToken);
        var result3 = await PropertiesRepository.CheckPropertyExistsInVersionAsync(version, nonExistentProperty, CancellationToken);

        // Assert
        result1.Value.Should().BeTrue();
        result2.Value.Should().BeTrue();
        result3.Value.Should().BeFalse();
    }
}