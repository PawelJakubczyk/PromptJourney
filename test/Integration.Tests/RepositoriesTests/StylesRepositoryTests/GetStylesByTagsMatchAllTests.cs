using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class GetStylesByTagsMatchAllTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // Test Constants
    private const string TestTag1 = "modern";

    private const string TestTag2 = "abstract";
    private const string TestTag3 = "colorful";

    [Fact]
    public async Task GetStylesByTagsMatchAllAsync_WithMatchingTags_ShouldReturnMatchingStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1, TestTag2);
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName2, TestTag2, TestTag3);
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName3, TestTag3);

        var tags = TagsCollection.Create([TestTag2]);

        // Act
        var result = await StylesRepository.GetStylesByTags(tags.Value, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName2);
    }

    [Fact]
    public async Task GetStylesByTagsMatchAllAsync_WithMultipleTags_ShouldReturnStylesWithAnyTag()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName2, TestTag2);
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName3, TestTag3);

        var tags = TagsCollection.Create([TestTag1, TestTag3]);

        // Act
        var result = await StylesRepository.GetStylesByTags(tags.Value, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == DefaultTestStyleName3);
    }

    [Fact]
    public async Task GetStylesByTagsMatchAllAsync_WithNonExistentTags_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);

        var tags = TagsCollection.Create(["nonexistent"]);

        // Act
        var result = await StylesRepository.GetStylesByTags(tags.Value, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStylesByTagsMatchAllAsync_WithEmptyTagsList_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName1, TestTag1);

        var tags = TagsCollection.Create([]);

        // Act
        var result = await StylesRepository.GetStylesByTags(tags.Value, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStylesByTagsMatchAllAsync_WithStylesWithoutTags_ShouldNotReturnThose()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1); // Style without tags
        await CreateAndSaveTestStyleWithTagsAsync(DefaultTestStyleName2, TestTag1);

        var tags = TagsCollection.Create([TestTag1]);

        // Act
        var result = await StylesRepository.GetStylesByTags(tags.Value, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
        result.Value.First().StyleName.Value.Should().Be(DefaultTestStyleName2);
    }
}