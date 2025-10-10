using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class UpdateStyleDescriptionTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task UpdateStyleDescriptionAsync_WithExistingStyle_ShouldReturnSuccess()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var newDescription = Description.Create("Updated description for style").Value;

        // Act
        var result = await StylesRepository.UpdateStyleDescriptionAsync(styleName, newDescription, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be(DefaultTestStyleName1);
        result.Value.Description!.Value.Should().Be("Updated description for style");
    }

    [Fact]
    public async Task UpdateStyleDescriptionAsync_WithNullDescription_ShouldReturnSuccess()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var nullDescription = Description.Create(null).Value;

        // Act
        var result = await StylesRepository.UpdateStyleDescriptionAsync(styleName, nullDescription, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Description?.Value.Should().BeNull();
    }

    [Fact]
    public async Task UpdateStyleDescriptionAsync_WithNonExistentStyle_ShouldReturnFailure()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var description = Description.Create("Test description").Value;

        // Act
        var result = await StylesRepository.UpdateStyleDescriptionAsync(styleName, description, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task UpdateStyleDescriptionAsync_WithLongDescription_ShouldReturnSuccess()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var longDescription = Description.Create("This is a very detailed description of the style that explains all its characteristics, usage scenarios, and artistic elements in great detail.").Value;

        // Act
        var result = await StylesRepository.UpdateStyleDescriptionAsync(styleName, longDescription, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Description!.Value.Should().Contain("very detailed description");
    }

    [Fact]
    public async Task UpdateStyleDescriptionAsync_MultipleUpdates_ShouldReturnLatest()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        var firstDescription = Description.Create("First description").Value;
        var secondDescription = Description.Create("Second description").Value;

        // Act
        await StylesRepository.UpdateStyleDescriptionAsync(styleName, firstDescription, CancellationToken);
        var result = await StylesRepository.UpdateStyleDescriptionAsync(styleName, secondDescription, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Description!.Value.Should().Be("Second description");
    }
}