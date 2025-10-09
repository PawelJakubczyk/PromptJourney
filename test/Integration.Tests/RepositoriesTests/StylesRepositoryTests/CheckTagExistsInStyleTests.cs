using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class CheckTagExistsInStyleTests : RepositoryTestsBase
{
    public CheckTagExistsInStyleTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // Test Constants
    private const string TestTag1 = "modern";

    private const string TestTag2 = "abstract";

    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithExistingTag_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1, TestTag2);

        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await StylesRepository.CheckTagExistsInStyleAsync(styleName, tag, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithNonExistentTag_ShouldReturnFalse()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);

        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tag = Tag.Create("nonexistent").Value;

        // Act
        var result = await StylesRepository.CheckTagExistsInStyleAsync(styleName, tag, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithNonExistentStyle_ShouldReturnFalse()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await StylesRepository.CheckTagExistsInStyleAsync(styleName, tag, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithStyleWithoutTags_ShouldReturnFalse()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1); // Style without tags

        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await StylesRepository.CheckTagExistsInStyleAsync(styleName, tag, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithMultipleStylesAndTags_ShouldReturnCorrectResult()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName2, TestTag2);

        var styleName1 = StyleName.Create(DefaultTestStyleName1).Value;
        var styleName2 = StyleName.Create(DefaultTestStyleName2).Value;
        var tag1 = Tag.Create(TestTag1).Value;
        var tag2 = Tag.Create(TestTag2).Value;

        // Act & Assert
        var result1 = await StylesRepository.CheckTagExistsInStyleAsync(styleName1, tag1, CancellationToken);
        AssertSuccessResult(result1);
        result1.Value.Should().BeTrue();

        var result2 = await StylesRepository.CheckTagExistsInStyleAsync(styleName1, tag2, CancellationToken);
        AssertSuccessResult(result2);
        result2.Value.Should().BeFalse();

        var result3 = await StylesRepository.CheckTagExistsInStyleAsync(styleName2, tag2, CancellationToken);
        AssertSuccessResult(result3);
        result3.Value.Should().BeTrue();
    }
}