using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class GetStylesByDescriptionKeywordTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{
    [Fact]
    public async Task GetStylesByDescriptionKeywordAsync_WithMatchingKeyword_ShouldReturnMatchingStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync("Style1", DefaultTestStyleType);
        await CreateAndSaveTestStyleAsync("Style2", DefaultTestStyleType);
        await CreateAndSaveTestStyleAsync("Style3", DefaultTestStyleType);

        var keyword = Keyword.Create("Test").Value; // Will match "Test style Style1" etc.

        // Act
        var result = await StylesRepository.GetStylesByDescriptionKeywordAsync(keyword, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().AllSatisfy(s => s.Description!.Value.Should().Contain("Test"));
    }

    [Fact]
    public async Task GetStylesByDescriptionKeywordAsync_WithNonMatchingKeyword_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1, DefaultTestStyleType);

        var keyword = Keyword.Create("NonExistent").Value;

        // Act
        var result = await StylesRepository.GetStylesByDescriptionKeywordAsync(keyword, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStylesByDescriptionKeywordAsync_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        var keyword = Keyword.Create("Test").Value;

        // Act
        var result = await StylesRepository.GetStylesByDescriptionKeywordAsync(keyword, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStylesByDescriptionKeywordAsync_WithPartialMatch_ShouldReturnMatching()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync("AbstractArt", DefaultTestStyleType);
        await CreateAndSaveTestStyleAsync("RealisticPortrait", DefaultTestStyleType);

        var keyword = Keyword.Create("Abstract").Value;

        // Act
        var result = await StylesRepository.GetStylesByDescriptionKeywordAsync(keyword, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
        result.Value.First().StyleName.Value.Should().Be("AbstractArt");
    }
}