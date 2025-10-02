using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

public class DescriptionTests
{
    // Valid Creation Tests

    [Theory]
    [InlineData("Simple description")]
    [InlineData("A detailed description of the style")]
    [InlineData("This is a longer description that explains the characteristics of the style in more detail")]
    public void Create_WithValidValues_ShouldReturnSuccess(string validValue)
    {
        // Act
        var result = Description.Create(validValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validValue);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnSuccessWithNull()
    {
        // Act
        var result = Description.Create(null);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('A', Description.MaxLength);

        // Act
        var result = Description.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(Description.MaxLength);
    }

    // Invalid Creation Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithWhitespaceValue_ShouldReturnFailure(string whitespaceValue)
    {
        // Act
        var result = Description.Create(whitespaceValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('A', Description.MaxLength + 1);

        // Act
        var result = Description.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    // ToString Tests

    [Fact]
    public void ToString_WithValidValue_ShouldReturnValue()
    {
        // Arrange
        var descriptionString = "Test description";
        var description = Description.Create(descriptionString).Value;

        // Act
        var result = description.ToString();

        // Assert
        result.Should().Be(descriptionString);
    }

    [Fact]
    public void ToString_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var description = Description.Create(null).Value;

        // Act
        var result = description.ToString();

        // Assert
        result.Should().BeNull();
    }

    // Constants Tests

    [Fact]
    public void MaxLength_ShouldBeCorrect()
    {
        // Assert
        Description.MaxLength.Should().BeGreaterThan(0);
    }
}