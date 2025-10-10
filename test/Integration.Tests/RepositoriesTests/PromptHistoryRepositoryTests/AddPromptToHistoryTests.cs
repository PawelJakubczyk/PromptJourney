using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PromptHistoryRepositoryTests;

public class AddPromptToHistoryTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // AddPromptToHistoryAsync Tests
    [Fact]
    public async Task AddPromptToHistoryAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var (version, style) = await CreateBasicTestDataAsync();
        var promptHistory = await CreateTestPromptHistoryAsync(DefaultTestPrompt1, version, [style]);

        // Act
        var result = await PromptHistoryRepository.AddPromptToHistoryAsync(promptHistory, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Prompt.Value.Should().Be(DefaultTestPrompt1);
        result.Value.Version.Value.Should().Be(DefaultTestVersion1);
        result.Value.HistoryId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task AddPromptToHistoryAsync_WithMultipleStyles_ShouldSucceed()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        var styles = await CreateAndSaveMultipleStylesAsync("Style1", "Style2", "Style3");
        var promptHistory = await CreateTestPromptHistoryAsync(DefaultTestPrompt1, version, styles);

        // Act
        var result = await PromptHistoryRepository.AddPromptToHistoryAsync(promptHistory, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.MidjourneyStyles.Should().HaveCount(3);
        result.Value.MidjourneyStyles.Should().Contain(s => s.StyleName.Value == "Style1");
        result.Value.MidjourneyStyles.Should().Contain(s => s.StyleName.Value == "Style2");
        result.Value.MidjourneyStyles.Should().Contain(s => s.StyleName.Value == "Style3");
    }

    [Fact]
    public async Task AddPromptToHistoryAsync_WithNoStyles_ShouldSucceed()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        var promptHistory = await CreateTestPromptHistoryAsync(DefaultTestPrompt1, version, []);

        // Act
        var result = await PromptHistoryRepository.AddPromptToHistoryAsync(promptHistory, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Prompt.Value.Should().Be(DefaultTestPrompt1);
        result.Value.MidjourneyStyles.Should().BeEmpty();
    }

    [Fact]
    public async Task AddPromptToHistoryAsync_WithLongPrompt_ShouldSucceed()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        var style = await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var longPrompt = "A very detailed and comprehensive description " + new string('a', 800); // Within 1000 char limit

        var promptHistory = await CreateTestPromptHistoryAsync(longPrompt, version, [style]);

        // Act
        var result = await PromptHistoryRepository.AddPromptToHistoryAsync(promptHistory, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Prompt.Value.Should().HaveLength(longPrompt.Length);
    }
}