using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyVersions;
using Domain.Entities.MidjourneyStyle;
using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Repositories;

namespace Integration.Tests.Repositories;

public class PromptHistoryRepositoryTests : BaseTransactionIntegrationTest
{
    private const string TestVersion1 = "1.0";
    private const string TestVersion2 = "2.0";
    private const string TestVersion3 = "3.0";

    private const string TestStyleName1 = "TestStyle1";
    private const string TestStyleName2 = "TestStyle2";
    private const string TestStyleName3 = "TestStyle3";

    private const string TestPrompt1 = "A beautiful landscape with mountains and lakes";
    private const string TestPrompt2 = "Modern city skyline at sunset";
    private const string TestPrompt3 = "Abstract art with vibrant colors";

    private readonly PromptHistoryRepository _promptHistoryRepository;
    private readonly VersionsRepository _versionsRepository;
    private readonly StylesRepository _stylesRepository;

    public PromptHistoryRepositoryTests(MidjourneyDbFixture fixture) : base(fixture)
    {
        _promptHistoryRepository = new PromptHistoryRepository(DbContext);
        _versionsRepository = new VersionsRepository(DbContext);
        _stylesRepository = new StylesRepository(DbContext);
    }

    // AddPromptToHistoryAsync Tests
    [Fact]
    public async Task AddPromptToHistoryAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        var promptHistory = await CreateTestPromptHistoryAsync(
            TestPrompt1, 
            version, 
            [style]);

        // Act
        var result = await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Prompt.Value.Should().Be(TestPrompt1);
        result.Value.Version.Value.Should().Be(TestVersion1);
        result.Value.MidjourneyStyles.Should().HaveCount(1);
        result.Value.MidjourneyStyles.First().StyleName.Value.Should().Be(TestStyleName1);
    }

    [Fact]
    public async Task AddPromptToHistoryAsync_WithMultipleStyles_ShouldSucceed()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style1 = await CreateAndSaveTestStyleAsync(TestStyleName1);
        var style2 = await CreateAndSaveTestStyleAsync(TestStyleName2);
        var style3 = await CreateAndSaveTestStyleAsync(TestStyleName3);

        var promptHistory = await CreateTestPromptHistoryAsync(
            TestPrompt1, 
            version, 
            [style1, style2, style3]);

        // Act
        var result = await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.MidjourneyStyles.Should().HaveCount(3);
        result.Value.MidjourneyStyles.Should().Contain(s => s.StyleName.Value == TestStyleName1);
        result.Value.MidjourneyStyles.Should().Contain(s => s.StyleName.Value == TestStyleName2);
        result.Value.MidjourneyStyles.Should().Contain(s => s.StyleName.Value == TestStyleName3);
    }

    [Fact]
    public async Task AddPromptToHistoryAsync_WithEmptyStyleList_ShouldSucceed()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);

        var promptHistory = await CreateTestPromptHistoryAsync(
            TestPrompt1, 
            version, 
            []);

        // Act
        var result = await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.MidjourneyStyles.Should().BeEmpty();
    }

    [Fact]
    public async Task AddPromptToHistoryAsync_WithLongPrompt_ShouldSucceed()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        var longPrompt = new string('A', 400); // Close to 500 char limit
        var promptHistory = await CreateTestPromptHistoryAsync(
            longPrompt, 
            version, 
            [style]);

        // Act
        var result = await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Prompt.Value.Should().HaveLength(400);
    }

    // GetAllHistoryRecordsAsync Tests
    [Fact]
    public async Task GetAllHistoryRecordsAsync_WithMultipleRecords_ShouldReturnAllRecords()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt3, version, [style]);

        // Act
        var result = await _promptHistoryRepository.GetAllHistoryRecordsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(h => h.Prompt.Value == TestPrompt1);
        result.Value.Should().Contain(h => h.Prompt.Value == TestPrompt2);
        result.Value.Should().Contain(h => h.Prompt.Value == TestPrompt3);
    }

    [Fact]
    public async Task GetAllHistoryRecordsAsync_WithNoRecords_ShouldReturnEmptyList()
    {
        // Act
        var result = await _promptHistoryRepository.GetAllHistoryRecordsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllHistoryRecordsAsync_ShouldReturnRecordsOrderedByCreatedOnDescending()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        var firstRecord = await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);
        await Task.Delay(100); // Ensure different timestamps
        var secondRecord = await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style]);
        await Task.Delay(100);
        var thirdRecord = await CreateAndSaveTestPromptHistoryAsync(TestPrompt3, version, [style]);

        // Act
        var result = await _promptHistoryRepository.GetAllHistoryRecordsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        
        // Should be ordered by CreatedOn descending (newest first)
        result.Value[0].CreatedOn.Should().BeAfter(result.Value[1].CreatedOn);
        result.Value[1].CreatedOn.Should().BeAfter(result.Value[2].CreatedOn);
    }

    // GetHistoryByDateRangeAsync Tests
    [Fact]
    public async Task GetHistoryByDateRangeAsync_WithRecordsInRange_ShouldReturnMatchingRecords()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        var startDate = DateTime.UtcNow.AddDays(-2);
        var endDate = DateTime.UtcNow.AddDays(-1);

        // Create records: one before range, two in range, one after range
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style], DateTime.UtcNow.AddDays(-3));
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style], DateTime.UtcNow.AddDays(-1.5));
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt3, version, [style], DateTime.UtcNow.AddHours(-30));
        await CreateAndSaveTestPromptHistoryAsync("Future prompt", version, [style], DateTime.UtcNow);

        // Act
        var result = await _promptHistoryRepository.GetHistoryByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(h => h.Prompt.Value == TestPrompt2);
        result.Value.Should().Contain(h => h.Prompt.Value == TestPrompt3);
    }

    [Fact]
    public async Task GetHistoryByDateRangeAsync_WithNoRecordsInRange_ShouldReturnEmptyList()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style], DateTime.UtcNow.AddDays(-10));

        var startDate = DateTime.UtcNow.AddDays(-5);
        var endDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = await _promptHistoryRepository.GetHistoryByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetHistoryByDateRangeAsync_WithSameDateRange_ShouldWork()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        var targetDate = DateTime.UtcNow.Date;
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style], targetDate.AddHours(12));

        // Act
        var result = await _promptHistoryRepository.GetHistoryByDateRangeAsync(targetDate, targetDate.AddDays(1));

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    // GetHistoryRecordsByPromptKeywordAsync Tests
    [Fact]
    public async Task GetHistoryRecordsByPromptKeywordAsync_WithMatchingKeyword_ShouldReturnMatchingRecords()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync("Beautiful landscape with mountains", version, [style]);
        await CreateAndSaveTestPromptHistoryAsync("Amazing mountain view", version, [style]);
        await CreateAndSaveTestPromptHistoryAsync("City skyline at night", version, [style]);

        var keyword = Keyword.Create("mountain").Value;

        // Act
        var result = await _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(h => h.Prompt.Value.Contains("mountains"));
        result.Value.Should().Contain(h => h.Prompt.Value.Contains("mountain view"));
    }

    [Fact]
    public async Task GetHistoryRecordsByPromptKeywordAsync_WithNonMatchingKeyword_ShouldReturnEmptyList()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style]);

        var keyword = Keyword.Create("nonexistent").Value;

        // Act
        var result = await _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetHistoryRecordsByPromptKeywordAsync_CaseInsensitive_ShouldWork()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync("Beautiful LANDSCAPE with mountains", version, [style]);

        var keyword = Keyword.Create("landscape").Value;

        // Act
        var result = await _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    // GetLastHistoryRecordsAsync Tests
    [Fact]
    public async Task GetLastHistoryRecordsAsync_WithValidCount_ShouldReturnCorrectNumber()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);
        await Task.Delay(100);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style]);
        await Task.Delay(100);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt3, version, [style]);
        await Task.Delay(100);
        await CreateAndSaveTestPromptHistoryAsync("Fourth prompt", version, [style]);
        await Task.Delay(100);
        await CreateAndSaveTestPromptHistoryAsync("Fifth prompt", version, [style]);

        // Act
        var result = await _promptHistoryRepository.GetLastHistoryRecordsAsync(3);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        
        // Should return the 3 most recent records
        result.Value[0].CreatedOn.Should().BeAfter(result.Value[1].CreatedOn);
        result.Value[1].CreatedOn.Should().BeAfter(result.Value[2].CreatedOn);
    }

    [Fact]
    public async Task GetLastHistoryRecordsAsync_WithCountGreaterThanAvailable_ShouldReturnAllRecords()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style]);

        // Act
        var result = await _promptHistoryRepository.GetLastHistoryRecordsAsync(5);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLastHistoryRecordsAsync_WithZeroCount_ShouldReturnEmptyList()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);

        // Act
        var result = await _promptHistoryRepository.GetLastHistoryRecordsAsync(0);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // CalculateHistoricalRecordCountAsync Tests
    [Fact]
    public async Task CalculateHistoricalRecordCountAsync_WithMultipleRecords_ShouldReturnCorrectCount()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style]);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt3, version, [style]);

        // Act
        var result = await _promptHistoryRepository.CalculateHistoricalRecordCountAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(3);
    }

    [Fact]
    public async Task CalculateHistoricalRecordCountAsync_WithNoRecords_ShouldReturnZero()
    {
        // Act
        var result = await _promptHistoryRepository.CalculateHistoricalRecordCountAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
    }

    // Integration and Edge Case Tests
    [Fact]
    public async Task AddPromptToHistoryAsync_WithDifferentVersions_ShouldWork()
    {
        // Arrange
        var version1 = await CreateAndSaveTestVersionAsync(TestVersion1);
        var version2 = await CreateAndSaveTestVersionAsync(TestVersion2);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version1, [style]);
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version2, [style]);

        // Act
        var allRecords = await _promptHistoryRepository.GetAllHistoryRecordsAsync();

        // Assert
        allRecords.Value.Should().HaveCount(2);
        allRecords.Value.Should().Contain(h => h.Version.Value == TestVersion1);
        allRecords.Value.Should().Contain(h => h.Version.Value == TestVersion2);
    }

    [Fact]
    public async Task GetHistoryByDateRangeAsync_WithFutureEndDate_ShouldWork()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]);

        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = await _promptHistoryRepository.GetHistoryByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("landscape")]
    [InlineData("city")]
    [InlineData("art")]
    [InlineData("beautiful")]
    public async Task GetHistoryRecordsByPromptKeywordAsync_WithVariousKeywords_ShouldWork(string keywordValue)
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestPromptHistoryAsync(TestPrompt1, version, [style]); // Contains "landscape"
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt2, version, [style]); // Contains "city"
        await CreateAndSaveTestPromptHistoryAsync(TestPrompt3, version, [style]); // Contains "art"

        var keyword = Keyword.Create(keywordValue).Value;

        // Act
        var result = await _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        if (keywordValue == "beautiful")
        {
            result.Value.Should().HaveCount(1); // Only TestPrompt1 contains "beautiful"
        }
        else
        {
            result.Value.Should().HaveCountGreaterThan(0);
        }
    }

    [Fact]
    public async Task CRUD_Operations_Integration_ShouldWorkCorrectly()
    {
        // Arrange
        var version = await CreateAndSaveTestVersionAsync(TestVersion1);
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1);

        // Create
        var promptHistory = await CreateTestPromptHistoryAsync(TestPrompt1, version, [style]);
        var addResult = await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory);
        addResult.IsSuccess.Should().BeTrue();

        // Read - Get all
        var allRecords = await _promptHistoryRepository.GetAllHistoryRecordsAsync();
        allRecords.Value.Should().HaveCount(1);

        // Read - Count
        var count = await _promptHistoryRepository.CalculateHistoricalRecordCountAsync();
        count.Value.Should().Be(1);

        // Read - Get last
        var lastRecords = await _promptHistoryRepository.GetLastHistoryRecordsAsync(1);
        lastRecords.Value.Should().HaveCount(1);
        lastRecords.Value[0].Prompt.Value.Should().Be(TestPrompt1);
    }

    // Helper methods
    private async Task<MidjourneyVersion> CreateAndSaveTestVersionAsync(string versionValue)
    {
        var version = ModelVersion.Create(versionValue).Value;
        var parameter = Param.Create($"--v {versionValue}").Value;
        var description = Description.Create($"Test version {versionValue}").Value;

        var versionEntity = MidjourneyVersion.Create(version, parameter, DateTime.UtcNow, description).Value;
        var result = await _versionsRepository.AddVersionAsync(versionEntity);

        return result.Value;
    }

    private async Task<MidjourneyStyle> CreateAndSaveTestStyleAsync(string styleName, string styleType = "Abstract")
    {
        var name = StyleName.Create(styleName).Value;
        var type = StyleType.Create(styleType).Value;
        var description = Description.Create($"Test style {styleName}").Value;

        var style = MidjourneyStyle.Create(name, type, description).Value;
        var result = await _stylesRepository.AddStyleAsync(style);

        return result.Value;
    }

    private async Task<MidjourneyPromptHistory> CreateTestPromptHistoryAsync(
        string promptText, 
        MidjourneyVersion version, 
        List<MidjourneyStyle> styles,
        DateTime? createdOn = null)
    {
        var prompt = Prompt.Create(promptText).Value;

        var promptHistory = MidjourneyPromptHistory.Create
        (
            prompt, 
            version.Version,
            createdOn
        ).Value;

        foreach (var style in styles)
        {
            promptHistory.MidjourneyStyles.Add(style);
        }

        return promptHistory;
    }

    private async Task<MidjourneyPromptHistory> CreateAndSaveTestPromptHistoryAsync(
        string promptText, 
        MidjourneyVersion version, 
        List<MidjourneyStyle> styles,
        DateTime? createdOn = null)
    {
        var promptHistory = await CreateTestPromptHistoryAsync(promptText, version, styles, createdOn);
        var result = await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory);
        return result.Value;
    }
}