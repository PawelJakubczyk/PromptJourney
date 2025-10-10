using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PromptHistoryRepositoryTests;

public sealed class GetLastHistoryRecordsTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // GetLastHistoryRecordsAsync Tests
    [Fact]
    public async Task GetLastHistoryRecordsAsync_WithSpecificCount_ShouldReturnCorrectNumber()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();

        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt2, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt3, version, [style]);

        // Act
        var result = await PromptHistoryRepository.GetLastHistoryRecordsAsync(2, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLastHistoryRecordsAsync_WithCountGreaterThanRecords_ShouldReturnAllAvailable()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, version, [style]);

        // Act
        var result = await PromptHistoryRepository.GetLastHistoryRecordsAsync(5, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
    }
}