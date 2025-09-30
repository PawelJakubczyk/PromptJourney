using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

public class KeywordTests
{
    [Theory]
    [InlineData("landscape")]
    [InlineData("portrait")]
    [InlineData("abstract")]
    [InlineData("nature")]
    [InlineData("architecture")]
    public void Create_WithValidKeywords_ShouldReturnSuccess(string validKeyword)
    {
        // Act
        var result = Keyword.Create(validKeyword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validKeyword);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnFailureWithNull()
    {
        // Act
        var result = Keyword.Create(null);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithWhitespaceValue_ShouldReturnFailure(string whitespaceValue)
    {
        // Act
        var result = Keyword.Create(whitespaceValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('A', Keyword.MaxLength + 1);

        // Act
        var result = Keyword.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('A', Keyword.MaxLength);

        // Act
        var result = Keyword.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(Keyword.MaxLength);
    }

    [Fact]
    public void ToString_WithValidValue_ShouldReturnValue()
    {
        // Arrange
        var keywordString = "search";
        var keyword = Keyword.Create(keywordString).Value;

        // Act
        var result = keyword.ToString();

        // Assert
        result.Should().Be(keywordString);
    }

    [Theory]
    [InlineData("multi-word")]
    [InlineData("under_score")]
    [InlineData("CamelCase")]
    [InlineData("UPPERCASE")]
    public void Create_WithVariousKeywordFormats_ShouldReturnSuccess(string keywordFormat)
    {
        // Act
        var result = Keyword.Create(keywordFormat);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(keywordFormat);
    }
}