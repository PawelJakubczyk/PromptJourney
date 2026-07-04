using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

public class MaxValueTests
{
    [Theory]
    [InlineData("2")]
    [InlineData("1000")]
    [InlineData("2.0")]
    [InlineData("3:1")]
    [InlineData("999")]
    public void Create_WithValidValues_ShouldReturnSuccess(string validValue)
    {
        // Act
        var result = MaxValue.Create(validValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validValue);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnSuccessWithNone()
    {
        // Act
        var result = MaxValue.Create(null);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().Be(MaxValue.None);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithWhitespaceValue_ShouldReturnSuccessWithNone(string whitespaceValue)
    {
        // Act
        var result = MaxValue.Create(whitespaceValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(MaxValue.None);
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('9', MaxValue.MaxLength + 1);

        // Act
        var result = MaxValue.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('9', MaxValue.MaxLength);

        // Act
        var result = MaxValue.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(MaxValue.MaxLength);
    }

    [Fact]
    public void ToString_WithValidValue_ShouldReturnValue()
    {
        // Arrange
        var maxValueString = "100";
        var maxValue = MaxValue.Create(maxValueString).Value;

        // Act
        var result = maxValue.ToString();

        // Assert
        result.Should().Be(maxValueString);
    }

    [Fact]
    public void ToString_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var maxValue = MaxValue.Create(null).Value;

        // Act
        var result = maxValue.ToString();

        // Assert
        result.Should().Be(String.Empty);
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("50")]
    [InlineData("10000")]
    [InlineData("9:16")]
    [InlineData("∞")]
    public void Create_WithVariousNumericFormats_ShouldReturnSuccess(string numericValue)
    {
        // Act
        var result = MaxValue.Create(numericValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(numericValue);
    }
}