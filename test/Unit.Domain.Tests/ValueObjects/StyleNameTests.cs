using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

public class StyleNameTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnSuccess()
    {
        // Arrange
        var validValue = "Abstract Art";

        // Act
        var result = StyleName.Create(validValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validValue);
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
        var result = StyleName.Create(invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('A', StyleName.MaxLength + 1);

        // Act
        var result = StyleName.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('A', StyleName.MaxLength);

        // Act
        var result = StyleName.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(StyleName.MaxLength);
    }

    [Theory]
    [InlineData("Minimalist")]
    [InlineData("Abstract Art Style")]
    [InlineData("Photorealistic")]
    [InlineData("Vintage")]
    [InlineData("Cyberpunk")]
    public void Create_WithVariousValidValues_ShouldReturnSuccess(string validValue)
    {
        // Act
        var result = StyleName.Create(validValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validValue);
    }

    [Fact]
    public void Create_WithSpecialCharacters_ShouldReturnSuccess()
    {
        // Arrange
        var valueWithSpecialChars = "Style-Name_123 & More!";

        // Act
        var result = StyleName.Create(valueWithSpecialChars);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(valueWithSpecialChars);
    }

    [Fact]
    public void Create_WithUnicodeCharacters_ShouldReturnSuccess()
    {
        // Arrange
        var unicodeValue = "Стиль Фантазия 🎨";

        // Act
        var result = StyleName.Create(unicodeValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(unicodeValue);
    }

    [Fact]
    public void MaxLength_ShouldBe150()
    {
        // Assert
        StyleName.MaxLength.Should().Be(150);
    }
}