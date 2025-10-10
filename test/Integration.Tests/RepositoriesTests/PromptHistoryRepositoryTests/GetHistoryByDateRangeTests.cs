using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PromptHistoryRepositoryTests;

public sealed class GetHistoryByDateRangeTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // GetHistoryByDateRangeAsync Tests
    [Fact]
    public async Task GetHistoryByDateRangeAsync_WithRecordsInRange_ShouldReturnMatchingRecords()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();

        var dateFrom = DateTime.UtcNow.AddDays(-2);
        var dateTo = DateTime.UtcNow.AddDays(1);

        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, version, [style], DateTime.UtcNow.AddDays(-1));
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt2, version, [style], DateTime.UtcNow.AddDays(-5)); // Outside range

        // Act
        var result = await PromptHistoryRepository.GetHistoryByDateRangeAsync(dateFrom, dateTo, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
        result.Value[0].Prompt.Value.Should().Contain("landscape");
    }
}