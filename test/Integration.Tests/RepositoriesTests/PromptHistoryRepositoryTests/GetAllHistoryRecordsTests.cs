using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PromptHistoryRepositoryTests;

public class GetAllHistoryRecordsTests : RepositoryTestsBase
{
    public GetAllHistoryRecordsTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // GetAllHistoryRecordsAsync Tests
    [Fact]
    public async Task GetAllHistoryRecordsAsync_WithMultipleRecords_ShouldReturnAllRecords()
    {
        // Arrange
        var (versions, styles) = await CreateMultipleTestDataAsync();

        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, versions[0], [styles[0]]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt2, versions[1], [styles[1]]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt3, versions[0], [styles[2]]);

        // Act
        var result = await PromptHistoryRepository.GetAllHistoryRecordsAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(h => h.Prompt.Value.Contains("landscape"));
        result.Value.Should().Contain(h => h.Prompt.Value.Contains("city"));
        result.Value.Should().Contain(h => h.Prompt.Value.Contains("Abstract"));
    }

    [Fact]
    public async Task GetAllHistoryRecordsAsync_WithNoRecords_ShouldReturnEmptyList()
    {
        // Act
        var result = await PromptHistoryRepository.GetAllHistoryRecordsAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }
}