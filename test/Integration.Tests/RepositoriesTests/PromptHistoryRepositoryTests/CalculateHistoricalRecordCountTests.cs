using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PromptHistoryRepositoryTests;

public class CalculateHistoricalRecordCountTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // CalculateHistoricalRecordCountAsync Tests
    [Fact]
    public async Task CalculateHistoricalRecordCountAsync_WithMultipleRecords_ShouldReturnCorrectCount()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();

        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt2, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt3, version, [style]);
        // Act
        var result = await PromptHistoryRepository.CalculateHistoricalRecordCountAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().Be(3);
    }

    [Fact]
    public async Task CalculateHistoricalRecordCountAsync_WithNoRecords_ShouldReturnZero()
    {
        // Act
        var result = await PromptHistoryRepository.CalculateHistoricalRecordCountAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().Be(0);
    }

    [Fact]
    public async Task CalculateHistoricalRecordCountAsync_AfterAddingRecords_ShouldReturnUpdatedCount()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();

        // Add first record
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, version, [style]);

        // Act & Assert - First count
        var result1 = await PromptHistoryRepository.CalculateHistoricalRecordCountAsync(CancellationToken);
        result1.Value.Should().Be(1);

        // Add second record
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt2, version, [style]);

        // Act & Assert - Second count
        var result2 = await PromptHistoryRepository.CalculateHistoricalRecordCountAsync(CancellationToken);
        result2.Value.Should().Be(2);

        // Add third record
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt3, version, [style]);

        // Act & Assert - Third count
        var result3 = await PromptHistoryRepository.CalculateHistoricalRecordCountAsync(CancellationToken);
        result3.Value.Should().Be(3);
    }

    [Fact]
    public async Task CalculateHistoricalRecordCountAsync_WithRecordsFromDifferentVersions_ShouldCountAll()
    {
        // Arrange
        var versions = await CreateAndSaveMultipleVersionsAsync("1.0", "2.0");
        var style = await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt1, versions[0], [style]);
        await CreateAndSaveTestPromptHistoryAsync(DefaultTestPrompt2, versions[1], [style]);

        // Act
        var result = await PromptHistoryRepository.CalculateHistoricalRecordCountAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().Be(2);
    }
}