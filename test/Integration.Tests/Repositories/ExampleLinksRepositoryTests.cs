using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Repositories;
using System.ComponentModel;

namespace Integration.Tests.Repositories;

public class ExampleLinksRepositoryTests : BaseTransactionIntegrationTest
{
    private const string TestLink1 = "https://example.com/default-image1.jpg";
    private const string TestLink2 = "https://example.com/default-image2.jpg";
    private const string TestLink3 = "https://example.com/default-image3.jpg";
    private const string IncorectLink = "IncorectLink";

    private const string TestStyleName1 = "DefaultTestStyle1";
    private const string TestStyleName2 = "DefaultTestStyle2";
    private const string TestStyleName3 = "DefaultTestStyle3";
    private const string IncorectName = "";

    private const string TestVersion1 = "1.0";
    private const string TestVersion2 = "2.0";
    private const string TestVersion3 = "3.0";
    private const string IncorecteVersion = "";

    private readonly VersionsRepository _versionsRepository;
    private readonly ExampleLinkRepository _exampleLinkRepository;
    private readonly StylesRepository _stylesRepository;

    private readonly CancellationToken _cancellationToken;

    public ExampleLinksRepositoryTests(MidjourneyDbFixture fixture) : base(fixture)
    {
        _exampleLinkRepository = new ExampleLinkRepository(DbContext);
        _versionsRepository = new VersionsRepository(DbContext);
        _stylesRepository = new StylesRepository(DbContext);
    }

    #region AddExampleLink
    [Fact]
    public async Task AddExampleLink_WithValidData_ShouldSucceed_WhenVersionAndStyleExistAndLinkNotExist()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);

        var exampleLink = MidjourneyStyleExampleLink.Create
        (
            ExampleLink.Create(TestLink1),
            StyleName.Create(TestStyleName1),
            ModelVersion.Create(TestVersion1)
        ).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Link.Value.Should().Be(TestLink1);
        result.Value.StyleName.Value.Should().Be(TestStyleName1);
        result.Value.Version.Value.Should().Be(TestVersion1);
    }

    [Theory]
    [InlineData(null, TestStyleName1, TestVersion1)]
    [InlineData(TestLink1, null, TestVersion1)]
    [InlineData(TestLink1, TestStyleName1, null)]
    [InlineData(IncorectLink, TestStyleName1, TestVersion1)]
    [InlineData(TestLink1, IncorectName, TestVersion1)]
    [InlineData(TestLink1, TestStyleName1, IncorecteVersion)]
    public async Task AddExampleLink_WithInvalidData_ShouldFail(string? link, string? styleName, string? version)
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);

        // Act

        var invalidExampleLink = MidjourneyStyleExampleLink.Create
        (
            ExampleLink.Create(link),
            StyleName.Create(styleName),
            ModelVersion.Create(version)
        );

        // Assert
        invalidExampleLink.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AddExampleLink_WithDuplicateLink_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestExampleLinkAsync
        (
            TestLink1, 
            TestStyleName1, 
            TestVersion1
        );
        
        var duplicateExampleLink = MidjourneyStyleExampleLink.Create
        (
            ExampleLink.Create(TestLink1).Value,
            StyleName.Create(TestStyleName1).Value,
            ModelVersion.Create(TestVersion1).Value
        ).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(duplicateExampleLink, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddExampleLink_WithUrlLenghtInUrlCharacterLimit_ShouldSucceed()
    {
        // Arrange
        var longUrl = $"https://example.com/{new string('a', 150)}.jpg"; // Within 200 char limit

        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);

        var exampleLink = MidjourneyStyleExampleLink.Create
        (
            ExampleLink.Create(longUrl).Value,
            StyleName.Create(TestStyleName1).Value,
            ModelVersion.Create(TestVersion1).Value
        ).Value;

        // Act
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink, _cancellationToken);

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
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink, _cancellationToken);

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
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink, _cancellationToken);

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
        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
    #endregion AddExampleLink
    #region CheckExampleLinkExists

    [Fact]
    public async Task CheckExampleLinkExists_WithExistingLink_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        
        var link = ExampleLink.Create(TestLink1).Value;

        // Act
        var result = await _exampleLinkRepository.CheckExampleLinkExistsAsync(link, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckExampleLinkExists_WithNonExistentLink_ShouldReturnFalse()
    {
        // Arrange
        var link = ExampleLink.Create(TestLink1).Value;

        // Act
        var result = await _exampleLinkRepository.CheckExampleLinkExistsAsync(link, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    #endregion CheckExampleLinkExists
    #region CheckExampleLinkWithStyle
    [Fact]
    public async Task CheckExampleLinkWithStyleExists_WithExistingStyle_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        
        var styleNameVo = StyleName.Create(TestStyleName1).Value;

        // Act
        var result = await _exampleLinkRepository.CheckExampleLinkWithStyleExistsAsync(styleNameVo, _cancellationToken);

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
        var result = await _exampleLinkRepository.CheckExampleLinkWithStyleExistsAsync(styleName, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    #endregion CheckExampleLinkWithStyleExists
    #region CheckAnyExampleLinksExist
    [Fact]
    public async Task CheckAnyExampleLinksExist_WithExistingLinks_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);

        // Act
        var result = await _exampleLinkRepository.CheckAnyExampleLinksExistAsync(_cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckAnyExampleLinksExisty_WithNoLinks_ShouldReturnFalse()
    {
        // Act
        var result = await _exampleLinkRepository.CheckAnyExampleLinksExistAsync(_cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    #endregion CheckAnyExampleLinksExist
    #region GetAllExampleLinks
    [Fact]
    public async Task GetAllExampleLinks_WithMultipleLinks_ShouldReturnAllLinks()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestVersionAsync(TestVersion2);
        await CreateAndSaveTestVersionAsync(TestVersion3);

        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestStyleAsync(TestStyleName2);
        await CreateAndSaveTestStyleAsync(TestStyleName3);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        await CreateAndSaveTestExampleLinkAsync(TestLink2, TestStyleName2, TestVersion2);
        await CreateAndSaveTestExampleLinkAsync(TestLink3, TestStyleName3, TestVersion3);

        // Act
        var result = await _exampleLinkRepository.GetAllExampleLinksAsync(_cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(link => link.Link.Value == TestLink1);
        result.Value.Should().Contain(link => link.Link.Value == TestLink2);
        result.Value.Should().Contain(link => link.Link.Value == TestLink3);
    }

    [Fact]
    public async Task GetAllExampleLinks_WithNoLinks_ShouldReturnEmptyList()
    {
        // Act
        var result = await _exampleLinkRepository.GetAllExampleLinksAsync(_cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    #endregion GetAllExampleLinks
    #region GetExampleLinksByStyle
    [Fact]
    public async Task GetExampleLinksByStyle_WithExistingStyle_ShouldReturnMatchingLinks()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestVersionAsync(TestVersion2);

        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestStyleAsync(TestStyleName2);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        await CreateAndSaveTestExampleLinkAsync(TestLink2, TestStyleName1, TestVersion2);
        await CreateAndSaveTestExampleLinkAsync(TestLink3, TestStyleName2, TestVersion1);

        var styleName = StyleName.Create(TestStyleName1).Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAsync(styleName, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(link => link.StyleName.Value.Should().Be(TestStyleName1));
        result.Value.Should().Contain(link => link.Link.Value == TestLink1);
        result.Value.Should().Contain(link => link.Link.Value == TestLink2);
    }

    [Fact]
    public async Task GetExampleLinksByStyle_WithNonExistentStyle_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        
        var nonExistentStyle = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAsync(nonExistentStyle, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    #endregion GetExampleLinksByStyle
    #region GetExampleLinksByStyleAndVersion
    [Fact]
    public async Task GetExampleLinksByStyleAndVersion_WithExistingStyleAndVersion_ShouldReturnMatchingLinks()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestVersionAsync(TestVersion2);

        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestStyleAsync(TestStyleName2);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        await CreateAndSaveTestExampleLinkAsync(TestLink2, TestStyleName2, TestVersion1);
        await CreateAndSaveTestExampleLinkAsync(TestLink3, TestStyleName1, TestVersion2);

        var styleName = StyleName.Create(TestStyleName1).Value;
        var version = ModelVersion.Create(TestVersion1).Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value[0].StyleName.Value.Should().Be(TestStyleName1);
        result.Value[0].Version.Value.Should().Be(TestVersion1);
        result.Value[0].Link.Value.Should().Be(TestLink1);
    }

    [Fact]
    public async Task GetExampleLinksByStyleAndVersion_WithNonExistentCombination_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var version = ModelVersion.Create("7.0").Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExampleLinksByStyleAndVersion_WithNonExistedVersion_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        
        var styleName = StyleName.Create(TestStyleName1).Value;
        var version = ModelVersion.Create("7.0").Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExampleLinksByStyleAndVersion_WithNonExistedStyle_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var version = ModelVersion.Create(TestVersion1).Value;

        // Act
        var result = await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    #endregion GetExampleLinksByStyleAndVersion
    #region DeleteExampleLink
    [Fact]
    public async Task DeleteExampleLink_WithExistingLink_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestStyleAsync(TestStyleName1);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        
        var link = ExampleLink.Create(TestLink1).Value;

        // Act
        var result = await _exampleLinkRepository.DeleteExampleLinkAsync(link, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Link.Value.Should().Be(TestLink1);

        // Verify it's been deleted
        var checkResult = await _exampleLinkRepository.CheckExampleLinkExistsAsync(link, _cancellationToken);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteExampleLink_WithNonExistentLink_ShouldFail()
    {
        // Arrange
        var link = ExampleLink.Create(TestLink1).Value;

        // Act
        var result = await _exampleLinkRepository.DeleteExampleLinkAsync(link, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Failed to delete example link: Value cannot be null. (Parameter 'entity')"));
    }

    #endregion DeleteExampleLink
    #region DeleteAllExampleLinksByStyle
    [Fact]
    public async Task DeleteAllExampleLinksByStyle_WithExistingStyle_ShouldDeleteAllMatchingLinks()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(TestVersion1);
        await CreateAndSaveTestVersionAsync(TestVersion2);

        await CreateAndSaveTestStyleAsync(TestStyleName1);
        await CreateAndSaveTestStyleAsync(TestStyleName2);

        await CreateAndSaveTestExampleLinkAsync(TestLink1, TestStyleName1, TestVersion1);
        await CreateAndSaveTestExampleLinkAsync(TestLink2, TestStyleName1, TestVersion1);
        await CreateAndSaveTestExampleLinkAsync(TestLink3, TestStyleName2, TestVersion2);

        var styleName = StyleName.Create(TestStyleName1).Value;

        // Act
        var result = await _exampleLinkRepository.DeleteAllExampleLinksByStyleAsync(styleName, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2);

        // Verify deletion
        var remainingLinks = await _exampleLinkRepository.GetAllExampleLinksAsync(_cancellationToken);
        remainingLinks.Value.Should().HaveCount(1);
        remainingLinks.Value[0].StyleName.Value.Should().Be(TestStyleName2);
    }

    #endregion DeleteAllExampleLinksByStyle
    #region HelperMethods
    private async Task<MidjourneyVersion> CreateAndSaveTestVersionAsync(string versionValue)
    {
        var version = ModelVersion.Create(versionValue).Value;
        var parameter = Param.Create($"--v {versionValue}").Value;
        var description = Description.Create($"Test version {versionValue}").Value;

        var versionEntity = MidjourneyVersion.Create(version, parameter, DateTime.UtcNow, description).Value;
        var result = await _versionsRepository.AddVersionAsync(versionEntity, _cancellationToken);

        return result.Value;
    }

    private async Task<MidjourneyStyle> CreateAndSaveTestStyleAsync(string styleName, string styleType = "Custom")
    {
        var name = StyleName.Create(styleName).Value;
        var type = StyleType.Create(styleType).Value;
        var description = Description.Create($"Test style {styleName}").Value;

        var style = MidjourneyStyle.Create(name, type, description).Value;

        var result = await _stylesRepository.AddStyleAsync(style, _cancellationToken);
        return result.Value;
    }

    private async Task<MidjourneyStyleExampleLink> CreateAndSaveTestExampleLinkAsync
    (
        string linkUrl,
        string styleName,
        string versionValue
    )
    {
        var link = ExampleLink.Create(linkUrl).Value;
        var styleNameVo = StyleName.Create(styleName).Value;
        var versionVo = ModelVersion.Create(versionValue).Value;

        var exampleLink = MidjourneyStyleExampleLink.Create(link, styleNameVo, versionVo).Value;

        var result = await _exampleLinkRepository.AddExampleLinkAsync(exampleLink, _cancellationToken);
        return result.Value;
    }
    #endregion HelperMethods
}

