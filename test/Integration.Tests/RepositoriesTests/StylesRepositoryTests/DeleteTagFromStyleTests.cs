using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class DeleteTagFromStyleTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // Test Constants
    private const string TestTag1 = "modern";

    private const string TestTag2 = "abstract";

    [Fact]
    public async Task DeleteTagFromStyleAsync_WithExistingTag_ShouldReturnSuccess()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1, TestTag2);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tagResult = Tag.Create(TestTag1);

        // Act
        var result = await StylesRepository.DeleteTagFromStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Tags.Should().NotContain(t => t.Value == TestTag1);
        result.Value.Tags.Should().Contain(t => t.Value == TestTag2);
    }

    [Fact]
    public async Task DeleteTagFromStyleAsync_WithNonExistentTag_ShouldReturnFailure()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tagResult = Tag.Create("nonexistent");

        // Act
        var result = await StylesRepository.DeleteTagFromStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task DeleteTagFromStyleAsync_WithNonExistentStyle_ShouldReturnFailure()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var tagResult = Tag.Create(TestTag1);

        // Act
        var result = await StylesRepository.DeleteTagFromStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task DeleteTagFromStyleAsync_WithStyleWithoutTags_ShouldReturnFailure()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1); // Style without tags
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tagResult = Tag.Create(TestTag1);

        // Act
        var result = await StylesRepository.DeleteTagFromStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task DeleteTagFromStyleAsync_WithLastTag_ShouldLeaveEmptyTagsList()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var tagResult = Tag.Create(TestTag1);

        // Act
        var result = await StylesRepository.DeleteTagFromStyleAsync(styleName, tagResult, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Tags.Should().BeNullOrEmpty();
    }
}