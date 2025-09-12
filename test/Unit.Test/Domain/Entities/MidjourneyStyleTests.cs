using Domain.Entities.MidjourneyStyle;
using Domain.ValueObjects;

namespace Unit.Test.Domain.Entities;

public class MidjourneyStyleTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var nameResult = StyleName.Create("Abstract Art");
        var typeResult = StyleType.Create("Abstract");
        var descriptionResult = Description.Create("A beautiful abstract art style");

        // Act
        var result = MidjourneyStyle.Create
        (
            nameResult,
            typeResult,
            descriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StyleName.Value.Should().Be("Abstract Art");
        result.Value.Type.Value.Should().Be("Abstract");
        result.Value.Description?.Value.Should().Be("A beautiful abstract art style");
    }

    [Fact]
    public void Create_WithTags_ShouldReturnSuccess()
    {
        // Arrange
        var nameResult = StyleName.Create("Cyberpunk");
        var typeResult = StyleType.Create("Futuristic");
        var descriptionResult = Description.Create("Cyberpunk art style");
        var tagResults = new List<Result<Tag>?>
        {
            Tag.Create("neon"),
            Tag.Create("futuristic"),
            Tag.Create("tech")
        };

        // Act
        var result = MidjourneyStyle.Create
        (
            nameResult,
            typeResult,
            descriptionResult,
            tagResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StyleName.Value.Should().Be("Cyberpunk");
        result.Value.Type.Value.Should().Be("Futuristic");
        result.Value.Tags.Should().NotBeNull();
        result.Value.Tags.Should().HaveCount(3);
        result.Value.Tags.Should().Contain(t => t.Value == "neon");
        result.Value.Tags.Should().Contain(t => t.Value == "futuristic");
        result.Value.Tags.Should().Contain(t => t.Value == "tech");
    }

    [Fact]
    public void Create_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var nameResult = StyleName.Create("Minimalist");
        var typeResult = StyleType.Create("Clean");

        // Act
        var result = MidjourneyStyle.Create
        (
            nameResult,
            typeResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StyleName.Value.Should().Be("Minimalist");
        result.Value.Type.Value.Should().Be("Clean");
        result.Value.Description.Should().BeNull();
        result.Value.Tags.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullDescription_ShouldReturnSuccess()
    {
        // Arrange
        var nameResult = StyleName.Create("Test Style");
        var typeResult = StyleType.Create("Test Type");

        // Act
        var result = MidjourneyStyle.Create
        (
            nameResult,
            typeResult,
            null
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyTagsList_ShouldReturnSuccessWithNullTags()
    {
        // Arrange
        var nameResult = StyleName.Create("Test Style");
        var typeResult = StyleType.Create("Test Type");
        var emptyTagResults = new List<Result<Tag>?>();

        // Act
        var result = MidjourneyStyle.Create
        (
            nameResult,
            typeResult,
            null,
            emptyTagResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Tags.Should().BeNull();
    }

    [Fact]
    public void Create_WithInvalidStyleName_ShouldReturnFailure()
    {
        // Arrange
        var invalidNameResult = StyleName.Create("");
        var typeResult = StyleType.Create("Abstract");

        // Act
        var result = MidjourneyStyle.Create
        (
            invalidNameResult,
            typeResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidStyleType_ShouldReturnFailure()
    {
        // Arrange
        var nameResult = StyleName.Create("Test Style");
        var invalidTypeResult = StyleType.Create(null);

        // Act
        var result = MidjourneyStyle.Create
        (
            nameResult,
            invalidTypeResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidTag_ShouldReturnFailure()
    {
        // Arrange
        var nameResult = StyleName.Create("Test Style");
        var typeResult = StyleType.Create("Test Type");
        var tagResults = new List<Result<Tag>?>
        {
            Tag.Create("valid"),
            Tag.Create("") // Invalid tag
        };

        // Act
        var result = MidjourneyStyle.Create
        (
            nameResult,
            typeResult,
            null,
            tagResults
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void AddTag_WithValidTag_ShouldReturnSuccess()
    {
        // Arrange
        var style = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type")
        ).Value;

        var newTag = Tag.Create("newtag").Value;

        // Act
        var result = style.AddTag(newTag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        style.Tags.Should().NotBeNull();
        style.Tags.Should().Contain(t => t.Value == "newtag");
    }

    [Fact]
    public void AddTag_WithExistingTag_ShouldReturnFailure()
    {
        // Arrange
        var tagResults = new List<Result<Tag>?>
        {
            Tag.Create("existing")
        };

        var style = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type"),
            null,
            tagResults
        ).Value;

        var duplicateTag = Tag.Create("existing").Value;

        // Act
        var result = style.AddTag(duplicateTag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void RemoveTag_WithExistingTag_ShouldReturnSuccess()
    {
        // Arrange
        var tagResults = new List<Result<Tag>?>
        {
            Tag.Create("tag1"),
            Tag.Create("tag2")
        };

        var style = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type"),
            null,
            tagResults
        ).Value;

        var tagToRemove = Tag.Create("tag1").Value;

        // Act
        var result = style.RemoveTag(tagToRemove);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        style.Tags.Should().NotContain(t => t.Value == "tag1");
        style.Tags.Should().Contain(t => t.Value == "tag2");
    }

    [Fact]
    public void RemoveTag_WithNonExistingTag_ShouldReturnFailure()
    {
        // Arrange
        var tagResults = new List<Result<Tag>?>
        {
            Tag.Create("existing")
        };

        var style = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type"),
            null,
            tagResults
        ).Value;

        var nonExistingTag = Tag.Create("nonexisting").Value;

        // Act
        var result = style.RemoveTag(nonExistingTag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void RemoveTag_WithEmptyTagsList_ShouldReturnFailure()
    {
        // Arrange
        var style = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type")
        ).Value;

        var tag = Tag.Create("anytag").Value;

        // Act
        var result = style.RemoveTag(tag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void EditDescription_WithValidDescription_ShouldReturnSuccess()
    {
        // Arrange
        var style = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type")
        ).Value;

        var newDescription = Description.Create("New description").Value;

        // Act
        var result = style.EditDescription(newDescription);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        style.Description?.Value.Should().Be("New description");
    }

    [Fact]
    public void EditDescription_WithNullDescription_ShouldReturnSuccess()
    {
        // Arrange
        var style = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type"),
            Description.Create("Original description")
        ).Value;

        // Act
        var result = style.EditDescription(null);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        style.Description.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldInitializeNavigationProperties()
    {
        // Arrange & Act
        var result = MidjourneyStyle.Create
        (
            StyleName.Create("Test Style"),
            StyleType.Create("Test Type")
        );

        // Assert
        result.Value.MidjourneyPromptHistories.Should().NotBeNull();
        result.Value.MidjourneyPromptHistories.Should().BeEmpty();
        result.Value.ExampleLinks.Should().NotBeNull();
        result.Value.ExampleLinks.Should().BeEmpty();
    }
}