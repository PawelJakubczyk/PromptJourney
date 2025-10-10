using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PropertiesRepositoryTests;

public class UpdateParameterTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task UpdateParameterForVersionAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        _ = await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1]);

        // Modify the property
        var updatedProperty = await CreateTestPropertyAsync(
            DefaultTestVersion1,
            DefaultTestPropertyName1,
            [DefaultTestParam1, "--updated"],
            "UpdatedDefault",
            "UpdatedMin",
            "UpdatedMax",
            "Updated description");

        // Act
        var result = await PropertiesRepository.UpdatePropertyAsync(updatedProperty, CancellationToken);

        // Assert
        AssertSuccessResult(result);
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
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var property = await CreateTestPropertyAsync(
            DefaultTestVersion1,
            "NonExistentProperty",
            [DefaultTestParam1],
            "DefaultValue",
            null,
            null,
            "Test description");

        // Act
        var result = await PropertiesRepository.UpdatePropertyAsync(property, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }
}