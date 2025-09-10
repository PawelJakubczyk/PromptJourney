using Domain.Entities.MidjourneyStyleExampleLinks;
using Domain.Entities.MidjourneyVersions;
using Domain.Entities.MidjourneyStyle;
using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Repositories;

namespace Integration.Tests.Repositories;

public class ExampleLinksRepositoryTests : BaseTransactionIntegrationTest
{
    private const string DefaultTestLink1 = "https://example.com/default-image1.jpg";
    private const string DefaultTestLink2 = "https://example.com/default-image2.jpg";
    private const string DefaultTestLink3 = "https://example.com/default-image3.jpg";

    private const string DefaultTestStyleName1 = "DefaultTestStyle1";
    private const string DefaultTestStyleName2 = "DefaultTestStyle2";
    private const string DefaultTestStyleName3 = "DefaultTestStyle3";

    private const string DefaultTestVersion1 = "1.0";
    private const string DefaultTestVersion2 = "2.0";
    private const string DefaultTestVersion3 = "3.0";

    private readonly VersionsRepository _versionsRepository;
    private readonly ExampleLinkRepository _exampleLinkRepository;
    private readonly StylesRepository _stylesRepository;

    public ExampleLinksRepositoryTests(MidjourneyDbFixture fixture) : base(fixture)
    {
        _exampleLinkRepository = new ExampleLinkRepository(DbContext);
        _versionsRepository = new VersionsRepository(DbContext);
        _stylesRepository = new StylesRepository(DbContext);
    }

    [Fact]
    public async Task AddExampleLink_WithValidData_ShouldSucceed_WhenVersionAndStyleExist()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);

        var exampleLink = MidjourneyStyleExampleLink.Create
        (
            ExampleLink.Create(DefaultTestLink1).Value,
            StyleName.Create(DefaultTestStyleName1).Value,
            ModelVersion.Create(DefaultTestVersion1).Value
        ).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Link.Value.Should().Be(DefaultTestLink1);
        result.Value.StyleName.Value.Should().Be(DefaultTestStyleName1);
        result.Value.Version.Value.Should().Be(DefaultTestVersion1);
    }

    [Fact]
    public async Task AddExampleLink_WithDuplicateLink_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var firstExampleLink = await CreateAndSaveTestExampleLinkAsync
        (
            DefaultTestLink1, 
            DefaultTestStyleName1, 
            DefaultTestVersion1
        );
        
        var duplicateExampleLink = MidjourneyStyleExampleLink.Create
        (
            ExampleLink.Create(DefaultTestLink1).Value,
            StyleName.Create(DefaultTestStyleName1).Value,
            ModelVersion.Create(DefaultTestVersion1).Value
        ).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(duplicateExampleLink);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CheckExampleLinkExists_WithExistingLink_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);
        
        var link = ExampleLink.Create(DefaultTestLink1).Value;

        // Act
        var result = await _exampleLinkRepository.CheckExampleLinkExistsAsync(link);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckExampleLinkExists_WithNonExistentLink_ShouldReturnFalse()
    {
        // Arrange
        var link = ExampleLink.Create("https://example.com/non-existent.jpg").Value;

        // Act
        var result = await _exampleLinkRepository.CheckExampleLinkExistsAsync(link);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckExampleLinkWithStyleExists_WithExistingStyle_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);
        
        var styleNameVo = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await _exampleLinkRepository.CheckExampleLinkWithStyleExistsAsync(styleNameVo);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckExampleLinkWithStyleExists_WithNonExistentStyle_ShouldReturnFalse()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await _exampleLinkRepository.CheckExampleLinkWithStyleExistsAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckAnyExampleLinksExist_WithExistingLinks_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        // Act
        var result = await _exampleLinkRepository.CheckAnyExampleLinksExist();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckAnyExampleLinksExisty_WithNoLinks_ShouldReturnFalse()
    {
        // Act
        var result = await _exampleLinkRepository.CheckAnyExampleLinksExist();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllExampleLinks_WithMultipleLinks_ShouldReturnAllLinks()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);
        await CreateAndSaveTestVersionAsync(DefaultTestVersion2);
        await CreateAndSaveTestVersionAsync(DefaultTestVersion3);

        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName2);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName3);

        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink2, DefaultTestStyleName2, DefaultTestVersion2);
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink3, DefaultTestStyleName3, DefaultTestVersion3);

        // Act
        var result = await _exampleLinkRepository.GetAllExampleLinksAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink1);
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink2);
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink3);
    }

    [Fact]
    public async Task GetAllExampleLinks_WithNoLinks_ShouldReturnEmptyList()
    {
        // Act
        var result = await _exampleLinkRepository.GetAllExampleLinksAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExampleLinksByStyle_WithExistingStyle_ShouldReturnMatchingLinks()
    {
        // Arrange
        var targetStyle = "TargetStyle";


        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, targetStyle, "6.0");
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink2, targetStyle, "6.1");
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink3, "OtherStyle", "6.0");

        var styleName = StyleName.Create(targetStyle).Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(link => link.StyleName.Value.Should().Be(targetStyle));
        result.Value.Should().Contain(link => link.Link.Value == "https://example.com/target1.jpg");
        result.Value.Should().Contain(link => link.Link.Value == "https://example.com/target2.jpg");
    }

    [Fact]
    public async Task GetExampleLinksByStyle_WithNonExistentStyle_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestExampleLinkAsync("https://example.com/test.jpg", "ExistingStyle", "6.0");
        
        var nonExistentStyle = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAsync(nonExistentStyle);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExampleLinksByStyleAndVersion_WithExistingStyleAndVersion_ShouldReturnMatchingLinks()
    {
        // Arrange
        var targetStyle = "TargetStyle";
        var targetVersion = "6.0";
        await CreateAndSaveTestExampleLinkAsync("https://example.com/target.jpg", targetStyle, targetVersion);
        await CreateAndSaveTestExampleLinkAsync("https://example.com/other-style.jpg", "OtherStyle", targetVersion);
        await CreateAndSaveTestExampleLinkAsync("https://example.com/other-version.jpg", targetStyle, "6.1");

        var styleName = StyleName.Create(targetStyle).Value;
        var version = ModelVersion.Create(targetVersion).Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value[0].StyleName.Value.Should().Be(targetStyle);
        result.Value[0].Version.Value.Should().Be(targetVersion);
        result.Value[0].Link.Value.Should().Be("https://example.com/target.jpg");
    }

    [Fact]
    public async Task GetExampleLinksByStyleAndVersion_WithNonExistentCombination_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestExampleLinkAsync("https://example.com/test.jpg", "ExistingStyle", "6.0");
        
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var version = ModelVersion.Create("7.0").Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteExampleLink_WithExistingLink_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);
        
        var link = ExampleLink.Create(DefaultTestLink1).Value;

        // Act
        var result = await _exampleLinkRepository.DeleteExampleLinkAsync(link);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Link.Value.Should().Be(DefaultTestLink1);

        // Verify it's been deleted
        var checkResult = await _exampleLinkRepository.CheckExampleLinkExistsAsync(link);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteExampleLink_WithNonExistentLink_ShouldFail()
    {
        // Arrange
        var link = ExampleLink.Create("https://example.com/non-existent.jpg").Value;

        // Act
        var result = await _exampleLinkRepository.DeleteExampleLinkAsync(link);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("not found"));
    }

    [Fact]
    public async Task DeleteAllExampleLinksByStyle_WithExistingStyle_ShouldDeleteAllMatchingLinks()
    {
        // Arrange
        var targetStyle = "StyleToDelete";
        await CreateAndSaveTestExampleLinkAsync("https://example.com/delete1.jpg", targetStyle, "6.0");
        await CreateAndSaveTestExampleLinkAsync("https://example.com/delete2.jpg", targetStyle, "6.1");
        await CreateAndSaveTestExampleLinkAsync("https://example.com/keep.jpg", "KeepStyle", "6.0");

        var styleName = StyleName.Create(targetStyle).Value;

        // Act
        var result = await _exampleLinkRepository.DeleteAllExampleLinksByStyleAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(link => link.StyleName.Value.Should().Be(targetStyle));

        // Verify deletion
        var remainingLinks = await _exampleLinkRepository.GetAllExampleLinksAsync();
        remainingLinks.Value.Should().HaveCount(1);
        remainingLinks.Value[0].StyleName.Value.Should().Be("KeepStyle");
    }

    [Fact]
    public async Task DeleteAllExampleLinksByStyle_WithNonExistentStyle_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestExampleLinkAsync("https://example.com/test.jpg", "ExistingStyle", "6.0");
        
        var nonExistentStyle = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await _exampleLinkRepository.DeleteAllExampleLinksByStyleAsync(nonExistentStyle);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();

        // Verify no links were deleted
        var allLinks = await _exampleLinkRepository.GetAllExampleLinksAsync();
        allLinks.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddExampleLink_WithInvalidUrl_ShouldFail()
    {
        // This should fail during ExampleLink.Create() due to invalid URL format
        var invalidLinkResult = ExampleLink.Create("not-a-valid-url");
        
        // Assert that creating an invalid link fails
        invalidLinkResult.IsSuccess.Should().BeFalse();
    }

    [Fact] 
    public async Task AddExampleLink_WithVeryLongUrl_ShouldSucceed()
    {
        // Arrange
        var modelVersion = "6.0";
        var styleName = "TestStyle";
        
        await CreateAndSaveTestVersionAsync(modelVersion);
        await CreateAndSaveTestStyleAsync(styleName);
        
        var longUrl = "https://example.com/" + new string('a', 150) + ".jpg"; // Within 200 char limit
        var link = ExampleLink.Create(longUrl).Value;
        var styleNameVo = StyleName.Create(styleName).Value;
        var versionVo = ModelVersion.Create(modelVersion).Value;
        
        var exampleLink = MidjourneyStyleExampleLink.Create(link, styleNameVo, versionVo).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Link.Value.Should().Be(longUrl);
    }

    [Theory]
    [InlineData("https://example.com/image1.jpg")]
    [InlineData("https://cdn.example.com/assets/image.png")]
    [InlineData("http://test.com/photo.jpeg")]
    [InlineData("https://media.website.org/picture.gif")]
    public async Task AddExampleLink_WithVariousValidUrls_ShouldSucceed(string url)
    {
        // Arrange
        var modelVersion = "6.0";
        var uniqueStyleName = $"TestStyle_{Guid.NewGuid():N}"; // Unique style name
        
        await CreateAndSaveTestVersionAsync(modelVersion);
        await CreateAndSaveTestStyleAsync(uniqueStyleName);
        
        var link = ExampleLink.Create(url).Value;
        var styleName = StyleName.Create(uniqueStyleName).Value;
        var versionVo = ModelVersion.Create(modelVersion).Value;
        
        var exampleLink = MidjourneyStyleExampleLink.Create(link, styleName, versionVo).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Link.Value.Should().Be(url);
    }

    [Fact]
    public async Task AddExampleLink_WithNonExistentVersion_ShouldFail()
    {
        // Arrange
        var styleName = "TestStyle";
        await CreateAndSaveTestStyleAsync(styleName);
        
        var link = ExampleLink.Create("https://example.com/test.jpg").Value;
        var styleNameVo = StyleName.Create(styleName).Value;
        var nonExistentVersion = ModelVersion.Create("999.0").Value;
        
        var exampleLink = MidjourneyStyleExampleLink.Create(link, styleNameVo, nonExistentVersion).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddExampleLink_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var modelVersion = "6.0";
        await CreateAndSaveTestVersionAsync(modelVersion);
        
        var link = ExampleLink.Create("https://example.com/test.jpg").Value;
        var nonExistentStyle = StyleName.Create("NonExistentStyle").Value;
        var versionVo = ModelVersion.Create(modelVersion).Value;
        
        var exampleLink = MidjourneyStyleExampleLink.Create(link, nonExistentStyle, versionVo).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    // Helper methods - poprawione
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

    private async Task<MidjourneyStyleExampleLink> CreateAndSaveTestExampleLinkAsync
    (
        string linkUrl,
        string styleName,
        string versionValue
    )
    {
        // Ensure version and style exist
            var link = ExampleLink.Create(linkUrl).Value;
        var styleNameVo = StyleName.Create(styleName).Value;
        var versionVo = ModelVersion.Create(versionValue).Value;

        var exampleLink = MidjourneyStyleExampleLink.Create(link, styleNameVo, versionVo).Value;

        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink);
        return result.Value;
    }
}

