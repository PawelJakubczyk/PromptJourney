using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PromptHistoryRepositoryTests;

public sealed class GetHistoryRecordsByPromptKeywordTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // GetHistoryRecordsByPromptKeywordAsync Tests
    [Fact]
    public async Task GetHistoryRecordsByPromptKeywordAsync_WithMatchingKeyword_ShouldReturnMatchingRecords()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();

        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt2, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt3, version, [style]);

        var keyword = Keyword.Create("landscape").Value;

        // Act
        var result = await PromptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
        result.Value[0].Prompt.Value.Should().Contain("landscape");
    }

    [Fact]
    public async Task GetHistoryRecordsByPromptKeywordAsync_WithNonMatchingKeyword_ShouldReturnEmptyList()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, version, [style]);

        var keyword = Keyword.Create("nonexistent").Value;

        // Act
        var result = await PromptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }
}