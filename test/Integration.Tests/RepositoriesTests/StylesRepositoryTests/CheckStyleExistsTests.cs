using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class CheckStyleExistsTests : RepositoryTestsBase
{
    public CheckStyleExistsTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CheckStyleExistsAsync_WithExistingStyle_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await StylesRepository.CheckStyleExistsAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckStyleExistsAsync_WithNonExistentStyle_ShouldReturnFalse()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await StylesRepository.CheckStyleExistsAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckStyleExistsAsync_WithEmptyDatabase_ShouldReturnFalse()
    {
        // Arrange
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await StylesRepository.CheckStyleExistsAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckStyleExistsAsync_WithMultipleStyles_ShouldReturnCorrectResult()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName2);

        var existingStyleName = StyleName.Create(DefaultTestStyleName1).Value;
        var nonExistingStyleName = StyleName.Create(DefaultTestStyleName3).Value;

        // Act & Assert
        var existingResult = await StylesRepository.CheckStyleExistsAsync(existingStyleName, CancellationToken);
        AssertSuccessResult(existingResult);
        existingResult.Value.Should().BeTrue();

        var nonExistingResult = await StylesRepository.CheckStyleExistsAsync(nonExistingStyleName, CancellationToken);
        AssertSuccessResult(nonExistingResult);
        nonExistingResult.Value.Should().BeFalse();
    }
}