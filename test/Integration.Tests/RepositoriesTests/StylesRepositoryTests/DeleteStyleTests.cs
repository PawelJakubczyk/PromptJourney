using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class DeleteStyleTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task DeleteStyleAsync_WithExistingStyle_ShouldReturnSuccess()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await StylesRepository.DeleteStyleAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be(DefaultTestStyleName1);

        // Verify deletion
        var checkResult = await StylesRepository.CheckStyleExistsAsync(styleName, CancellationToken);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteStyleAsync_WithNonExistentStyle_ShouldReturnFailure()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await StylesRepository.DeleteStyleAsync(styleName, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task DeleteStyleAsync_WithStyleHavingTags_ShouldDeleteStyleAndTags()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, "modern", "abstract");
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await StylesRepository.DeleteStyleAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);

        // Verify style is deleted
        var checkResult = await StylesRepository.CheckStyleExistsAsync(styleName, CancellationToken);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteStyleAsync_WithMultipleStyles_ShouldDeleteOnlySpecified()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName2);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName3);

        var styleName = StyleName.Create(DefaultTestStyleName2).Value;

        // Act
        var result = await StylesRepository.DeleteStyleAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);

        // Verify only specified style is deleted
        var deletedCheck = await StylesRepository.CheckStyleExistsAsync(styleName, CancellationToken);
        deletedCheck.Value.Should().BeFalse();

        var stillExists1 = await StylesRepository.CheckStyleExistsAsync(StyleName.Create(DefaultTestStyleName1).Value, CancellationToken);
        stillExists1.Value.Should().BeTrue();

        var stillExists3 = await StylesRepository.CheckStyleExistsAsync(StyleName.Create(DefaultTestStyleName3).Value, CancellationToken);
        stillExists3.Value.Should().BeTrue();
    }
}