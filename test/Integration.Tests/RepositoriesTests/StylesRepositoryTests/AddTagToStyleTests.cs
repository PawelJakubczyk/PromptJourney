using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class AddTagToStyleTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // Test Constants
    private const string TestTag1 = "modern";

    private const string TestTag2 = "abstract";

    [Fact]
    public async Task AddTagToStyleAsync_WithExistingStyle_ShouldReturnSuccess()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tagResult = Tag.Create(TestTag1);

        // Act
        var result = await StylesRepository.AddTagToStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Tags.Should().NotBeNull();
        result.Value.Tags.Should().Contain(t => t.Value == TestTag1);
    }

    [Fact]
    public async Task AddTagToStyleAsync_WithMultipleTags_ShouldReturnSuccess()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result1 = await StylesRepository.AddTagToStyleAsync(styleName, Tag.Create(TestTag1), CancellationToken);
        var result2 = await StylesRepository.AddTagToStyleAsync(styleName, Tag.Create(TestTag2), CancellationToken);

        // Assert
        AssertSuccessResult(result2);
        result2.Value.Tags.Should().HaveCount(2);
        result2.Value.Tags.Should().Contain(t => t.Value == TestTag1);
        result2.Value.Tags.Should().Contain(t => t.Value == TestTag2);
    }

    [Fact]
    public async Task AddTagToStyleAsync_WithDuplicateTag_ShouldReturnFailure()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tagResult = Tag.Create(TestTag1);

        // Act
        var result = await StylesRepository.AddTagToStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task AddTagToStyleAsync_WithNonExistentStyle_ShouldReturnFailure()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var tagResult = Tag.Create(TestTag1);

        // Act
        var result = await StylesRepository.AddTagToStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task AddTagToStyleAsync_WithInvalidTag_ShouldReturnFailure()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var invalidTagResult = Tag.Create(""); // Invalid empty tag

        // Act
        var result = await StylesRepository.AddTagToStyleAsync(styleName, invalidTagResult, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }
}