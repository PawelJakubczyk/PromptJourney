using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PropertiesRepositoryTests;

public sealed class PatchParameterTests : RepositoryTestsBase
{
    public PatchParameterTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // PatchParameterForVersionAsync Tests

    [Fact]
    public async Task PatchParameterForVersionAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1], "OriginalDefault");

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var propertyName = PropertyName.Create(DefaultTestPropertyName1).Value;

        // Act
        var result = await PropertiesRepository.PatchPropertyAsync(
            version,
            propertyName,
            "DefaultValue",
            "NewDefault",
            CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.DefaultValue?.Value.Should().Be("NewDefault");
    }

    [Fact]
    public async Task PatchParameterForVersionAsync_WithNonExistentParameter_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var propertyName = PropertyName.Create("NonExistent").Value;

        // Act
        var result = await PropertiesRepository.PatchPropertyAsync(
            version,
            propertyName,
            "DefaultValue",
            "NewValue",
            CancellationToken);

        // Assert
        AssertFailureResult(result);
    }
}