using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PropertiesRepositoryTests;

public class DeleteParameterTests : RepositoryTestsBase
{
    public DeleteParameterTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // DeleteParameterInVersionAsync Tests
    [Fact]
    public async Task DeleteParameterInVersionAsync_WithExistingParameter_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1]);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var propertyName = PropertyName.Create(DefaultTestPropertyName1).Value;

        // Act
        var result = await PropertiesRepository.DeleteParameterInVersionAsync(version, propertyName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.PropertyName.Value.Should().Be(DefaultTestPropertyName1);

        // Verify it's been deleted
        var checkResult = await PropertiesRepository.CheckParameterExistsInVersionAsync(version, propertyName, CancellationToken);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteParameterInVersionAsync_WithNonExistentParameter_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var propertyName = PropertyName.Create("NonExistent").Value;

        // Act
        var result = await PropertiesRepository.DeleteParameterInVersionAsync(version, propertyName, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task DeleteParameterInVersionAsync_AfterDeletion_ShouldNotExistInDatabase()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName1, [DefaultTestParam1]);
        await CreateAndSaveTestPropertyAsync(DefaultTestVersion1, DefaultTestPropertyName2, [DefaultTestParam2]);

        var version = ModelVersion.Create(DefaultTestVersion1).Value;
        var propertyName = PropertyName.Create(DefaultTestPropertyName1).Value;

        // Act
        var deleteResult = await PropertiesRepository.DeleteParameterInVersionAsync(version, propertyName, CancellationToken);

        // Assert
        AssertSuccessResult(deleteResult);

        // Verify it doesn't exist anymore
        var allProperties = await PropertiesRepository.GetAllParametersByVersionAsync(version, CancellationToken);
        allProperties.Value.Should().HaveCount(1);
        allProperties.Value.Should().NotContain(p => p.PropertyName.Value == DefaultTestPropertyName1);
        allProperties.Value.Should().Contain(p => p.PropertyName.Value == DefaultTestPropertyName2);
    }
}