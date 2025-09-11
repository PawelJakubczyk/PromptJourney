using Domain.Entities.MidjourneyStyle;
using Domain.ValueObjects;
using FluentAssertions;
using Persistence.Repositories;

namespace Integration.Tests.Repositories;

public class StylesRepositoryTests : BaseTransactionIntegrationTest
{
    private const string TestStyleName1 = "TestStyle1";
    private const string TestStyleName2 = "TestStyle2";
    private const string TestStyleName3 = "TestStyle3";

    private const string TestStyleType1 = "Abstract";
    private const string TestStyleType2 = "Realistic";
    private const string TestStyleType3 = "Minimalist";

    private const string TestDescription1 = "Test description for style 1";
    private const string TestDescription2 = "Test description for style 2";
    private const string TestDescription3 = "Test description for style 3";

    private const string TestTag1 = "tag1";
    private const string TestTag2 = "tag2";
    private const string TestTag3 = "tag3";

    private readonly StylesRepository _stylesRepository;

    public StylesRepositoryTests(MidjourneyDbFixture fixture) : base(fixture)
    {
        _stylesRepository = new StylesRepository(DbContext);
    }

    // AddStyleAsync Tests
    [Fact]
    public async Task AddStyleAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var styleName = StyleName.Create(TestStyleName1);
        var styleType = StyleType.Create(TestStyleType1);
        var description = Description.Create(TestDescription1);

        var style = MidjourneyStyle.Create(styleName, styleType, description).Value;

        // Act
        var result = await _stylesRepository.AddStyleAsync(style);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StyleName.Value.Should().Be(TestStyleName1);
        result.Value.Type.Value.Should().Be(TestStyleType1);
        result.Value.Description?.Value.Should().Be(TestDescription1);
    }

    [Fact]
    public async Task AddStyleAsync_WithDuplicateStyleName_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);

        var duplicateStyleName = StyleName.Create(TestStyleName1);
        var styleType = StyleType.Create(TestStyleType2);
        var description = Description.Create(TestDescription2);

        var duplicateStyle = MidjourneyStyle.Create(duplicateStyleName, styleType, description).Value;

        // Act
        var result = await _stylesRepository.AddStyleAsync(duplicateStyle);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddStyleAsync_WithNullDescription_ShouldSucceed()
    {
        // Arrange
        var styleName = StyleName.Create(TestStyleName1);
        var styleType = StyleType.Create(TestStyleType1);

        var style = MidjourneyStyle.Create(styleName, styleType, null).Value;

        // Act
        var result = await _stylesRepository.AddStyleAsync(style);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StyleName.Value.Should().Be(TestStyleName1);
        result.Value.Type.Value.Should().Be(TestStyleType1);
        result.Value.Description.Should().BeNull();
    }

    // GetAllStylesAsync Tests
    [Fact]
    public async Task GetAllStylesAsync_WithMultipleStyles_ShouldReturnAllStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        await CreateAndSaveTestStyleAsync(TestStyleName2, TestStyleType2, TestDescription2);
        await CreateAndSaveTestStyleAsync(TestStyleName3, TestStyleType3, TestDescription3);

        // Act
        var result = await _stylesRepository.GetAllStylesAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName2);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName3);
    }

    [Fact]
    public async Task GetAllStylesAsync_WithNoStyles_ShouldReturnEmptyList()
    {
        // Act
        var result = await _stylesRepository.GetAllStylesAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    // GetStyleByNameAsync Tests
    [Fact]
    public async Task GetStyleByNameAsync_WithExistingStyle_ShouldReturnStyle()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var styleName = StyleName.Create(TestStyleName1).Value;

        // Act
        var result = await _stylesRepository.GetStyleByNameAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StyleName.Value.Should().Be(TestStyleName1);
        result.Value.Type.Value.Should().Be(TestStyleType1);
        result.Value.Description.Value.Should().Be(TestDescription1);
    }

    [Fact]
    public async Task GetStyleByNameAsync_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await _stylesRepository.GetStyleByNameAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Style with name 'NonExistentStyle' not found"));
    }

    // GetStylesByTypeAsync Tests
    [Fact]
    public async Task GetStylesByTypeAsync_WithExistingType_ShouldReturnMatchingStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        await CreateAndSaveTestStyleAsync(TestStyleName2, TestStyleType1, TestDescription2);
        await CreateAndSaveTestStyleAsync(TestStyleName3, TestStyleType2, TestDescription3);

        var styleType = StyleType.Create(TestStyleType1).Value;

        // Act
        var result = await _stylesRepository.GetStylesByTypeAsync(styleType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(s => s.Type.Value.Should().Be(TestStyleType1));
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName2);
    }

    [Fact]
    public async Task GetStylesByTypeAsync_WithNonExistentType_ShouldFail()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var styleType = StyleType.Create("NonExistentType").Value;

        // Act
        var result = await _stylesRepository.GetStylesByTypeAsync(styleType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("No styles found for type: 'NonExistentType'"));
    }

    // GetStylesByTagsAsync Tests
    [Fact]
    public async Task GetStylesByTagsAsync_WithExistingTags_ShouldReturnMatchingStyles()
    {
        // Arrange
        var style1 = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var style2 = await CreateAndSaveTestStyleAsync(TestStyleName2, TestStyleType2, TestDescription2);
        var style3 = await CreateAndSaveTestStyleAsync(TestStyleName3, TestStyleType3, TestDescription3);

        var tag1 = Tag.Create(TestTag1).Value;
        var tag2 = Tag.Create(TestTag2).Value;

        // Add tags to styles
        await _stylesRepository.AddTagToStyleAsync(style1.StyleName, tag1);
        await _stylesRepository.AddTagToStyleAsync(style1.StyleName, tag2);
        await _stylesRepository.AddTagToStyleAsync(style2.StyleName, tag1);

        var tagsToSearch = new List<Tag> { tag1 };

        // Act
        var result = await _stylesRepository.GetStylesByTagsAsync(tagsToSearch);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName2);
    }

    [Fact]
    public async Task GetStylesByTagsAsync_WithNonExistentTags_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var nonExistentTag = Tag.Create("NonExistentTag").Value;
        var tagsToSearch = new List<Tag> { nonExistentTag };

        // Act
        var result = await _stylesRepository.GetStylesByTagsAsync(tagsToSearch);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    // GetStylesByDescriptionKeywordAsync Tests
    [Fact]
    public async Task GetStylesByDescriptionKeywordAsync_WithExistingKeyword_ShouldReturnMatchingStyles()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, "Description with keyword test");
        await CreateAndSaveTestStyleAsync(TestStyleName2, TestStyleType2, "Another description with test word");
        await CreateAndSaveTestStyleAsync(TestStyleName3, TestStyleType3, "Different description");

        var keyword = Keyword.Create("test").Value;

        // Act
        var result = await _stylesRepository.GetStylesByDescriptionKeywordAsync(keyword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName1);
        result.Value.Should().Contain(s => s.StyleName.Value == TestStyleName2);
    }

    [Fact]
    public async Task GetStylesByDescriptionKeywordAsync_WithNonExistentKeyword_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var keyword = Keyword.Create("nonexistent").Value;

        // Act
        var result = await _stylesRepository.GetStylesByDescriptionKeywordAsync(keyword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    // CheckStyleExistsAsync Tests
    [Fact]
    public async Task CheckStyleExistsAsync_WithExistingStyle_ShouldReturnTrue()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var styleName = StyleName.Create(TestStyleName1).Value;

        // Act
        var result = await _stylesRepository.CheckStyleExistsAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckStyleExistsAsync_WithNonExistentStyle_ShouldReturnFalse()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await _stylesRepository.CheckStyleExistsAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    // CheckTagExistsInStyleAsync Tests
    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithExistingTag_ShouldReturnTrue()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var tag = Tag.Create(TestTag1).Value;
        await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag);

        // Act
        var result = await _stylesRepository.CheckTagExistsInStyleAsync(style.StyleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithNonExistentTag_ShouldReturnFalse()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await _stylesRepository.CheckTagExistsInStyleAsync(style.StyleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task CheckTagExistsInStyleAsync_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await _stylesRepository.CheckTagExistsInStyleAsync(styleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Style with name 'NonExistentStyle' not found"));
    }

    // UpdateStyleAsync Tests
    [Fact]
    public async Task UpdateStyleAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var newDescription = Description.Create("Updated description").Value;
        style.EditDescription(newDescription);

        // Act
        var result = await _stylesRepository.UpdateStyleAsync(style);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description.Value.Should().Be("Updated description");
    }

    // DeleteStyleAsync Tests
    [Fact]
    public async Task DeleteStyleAsync_WithExistingStyle_ShouldSucceed()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);

        // Act
        var result = await _stylesRepository.DeleteStyleAsync(style.StyleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StyleName.Value.Should().Be(TestStyleName1);

        // Verify deletion
        var checkResult = await _stylesRepository.CheckStyleExistsAsync(style.StyleName);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteStyleAsync_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await _stylesRepository.DeleteStyleAsync(styleName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Style with name 'NonExistentStyle' not found"));
    }

    // AddTagToStyleAsync Tests
    [Fact]
    public async Task AddTagToStyleAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        // Verify tag was added
        var checkResult = await _stylesRepository.CheckTagExistsInStyleAsync(style.StyleName, tag);
        checkResult.Value.Should().BeTrue();
    }

    [Fact]
    public async Task AddTagToStyleAsync_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await _stylesRepository.AddTagToStyleAsync(styleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Style with name 'NonExistentStyle' not found"));
    }

    [Fact]
    public async Task AddTagToStyleAsync_WithDuplicateTag_ShouldFail()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var tag = Tag.Create(TestTag1).Value;
        await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag);

        // Act
        var result = await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    // DeleteTagFromStyleAsync Tests
    [Fact]
    public async Task DeleteTagFromStyleAsync_WithExistingTag_ShouldSucceed()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var tag = Tag.Create(TestTag1).Value;
        await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag);

        // Act
        var result = await _stylesRepository.DeleteTagFromStyleAsync(style.StyleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        // Verify tag was removed
        var checkResult = await _stylesRepository.CheckTagExistsInStyleAsync(style.StyleName, tag);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteTagFromStyleAsync_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await _stylesRepository.DeleteTagFromStyleAsync(styleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Style with name 'NonExistentStyle' not found"));
    }

    [Fact]
    public async Task DeleteTagFromStyleAsync_WithNonExistentTag_ShouldFail()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var tag = Tag.Create(TestTag1).Value;

        // Act
        var result = await _stylesRepository.DeleteTagFromStyleAsync(style.StyleName, tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    // UpadteStyleDescription Tests
    [Fact]
    public async Task UpdateStyleDescription_WithValidData_ShouldSucceed()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var newDescription = Description.Create("Updated description").Value;

        // Act
        var result = await _stylesRepository.UpadteStyleDescription(style.StyleName, newDescription);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description.Value.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateStyleDescription_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;
        var description = Description.Create("Some description").Value;

        // Act
        var result = await _stylesRepository.UpadteStyleDescription(styleName, description);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Style with name 'NonExistentStyle' not found"));
    }

    [Fact]
    public async Task UpdateStyleDescription_WithNullDescription_ShouldSucceed()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);

        // Act
        var result = await _stylesRepository.UpadteStyleDescription(style.StyleName, null);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description.Should().BeNull();
    }

    // Edge Cases and Validation Tests
    [Theory]
    [InlineData("Abstract")]
    [InlineData("Realistic")]
    [InlineData("Minimalist")]
    [InlineData("Surreal")]
    public async Task AddStyleAsync_WithVariousValidTypes_ShouldSucceed(string styleType)
    {
        // Arrange
        var uniqueStyleName = $"TestStyle_{Guid.NewGuid():N}";
        var styleName = StyleName.Create(uniqueStyleName);
        var type = StyleType.Create(styleType);
        var description = Description.Create($"Test style of type {styleType}");

        var style = MidjourneyStyle.Create(styleName, type, description).Value;

        // Act
        var result = await _stylesRepository.AddStyleAsync(style);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Type.Value.Should().Be(styleType);
    }

    [Fact]
    public async Task AddTagToStyleAsync_WithMultipleTags_ShouldSucceed()
    {
        // Arrange
        var style = await CreateAndSaveTestStyleAsync(TestStyleName1, TestStyleType1, TestDescription1);
        var tag1 = Tag.Create(TestTag1).Value;
        var tag2 = Tag.Create(TestTag2).Value;
        var tag3 = Tag.Create(TestTag3).Value;

        // Act
        await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag1);
        await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag2);
        var result = await _stylesRepository.AddTagToStyleAsync(style.StyleName, tag3);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify all tags exist
        var checkTag1 = await _stylesRepository.CheckTagExistsInStyleAsync(style.StyleName, tag1);
        var checkTag2 = await _stylesRepository.CheckTagExistsInStyleAsync(style.StyleName, tag2);
        var checkTag3 = await _stylesRepository.CheckTagExistsInStyleAsync(style.StyleName, tag3);

        checkTag1.Value.Should().BeTrue();
        checkTag2.Value.Should().BeTrue();
        checkTag3.Value.Should().BeTrue();
    }

    // Helper methods
    private async Task<MidjourneyStyle> CreateAndSaveTestStyleAsync(string styleName, string styleType, string description)
    {
        var name = StyleName.Create(styleName);
        var type = StyleType.Create(styleType);
        var desc = description != null ? Description.Create(description) : null;

        var style = MidjourneyStyle.Create(name, type, desc).Value;
        var result = await _stylesRepository.AddStyleAsync(style);

        return result.Value;
    }
}