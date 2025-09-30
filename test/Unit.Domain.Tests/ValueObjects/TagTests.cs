using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

public class TagTests
{


    [Theory]
    [InlineData("abstract")]
    [InlineData("realistic")]
    [InlineData("vintage")]
    [InlineData("modern")]
    [InlineData("cyberpunk")]
    [InlineData("fantasy")]
    public void Create_WithValidTags_ShouldReturnSuccess(string validTag)
    {
        // Act
        var result = Tag.Create(validTag);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validTag);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithNullOrWhitespaceValue_ShouldReturnFailure(string invalidValue)
    {
        // Act
        var result = Tag.Create(invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('A', Tag.MaxLength + 1);

        // Act
        var result = Tag.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('A', Tag.MaxLength);

        // Act
        var result = Tag.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(Tag.MaxLength);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var tagString = "landscape";
        var tag = Tag.Create(tagString).Value;

        // Act
        var result = tag.ToString();

        // Assert
        result.Should().Be(tagString);
    }

    [Theory]
    [InlineData("art-style")]
    [InlineData("3D_render")]
    [InlineData("photo.realistic")]
    [InlineData("AI-generated")]
    public void Create_WithSpecialCharacters_ShouldReturnSuccess(string tagWithSpecialChars)
    {
        // Act
        var result = Tag.Create(tagWithSpecialChars);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(tagWithSpecialChars);
    }

    [Fact]
    public void MaxLength_ShouldBe50()
    {
        // Assert
        Tag.MaxLength.Should().Be(50);
    }
}