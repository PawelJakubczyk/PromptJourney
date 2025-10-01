using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

public class DefaultValueTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("1:1")]
    [InlineData("100")]
    [InlineData("0.5")]
    [InlineData("default")]
    public void Create_WithValidValues_ShouldReturnSuccess(string validValue)
    {
        // Act
        var result = DefaultValue.Create(validValue);

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
        var result = DefaultValue.Create(null);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithWhitespaceValue_ShouldReturnFailure(string whitespaceValue)
    {
        // Act
        var result = DefaultValue.Create(whitespaceValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors[0].Message.Should().Be("DefaultValue: cannot be whitespace.");
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('1', DefaultValue.MaxLength + 1);

        // Act
        var result = DefaultValue.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors[0].Message.Should().Be($"DefaultValue: '{tooLongValue}' cannot be longer than 50 characters.");
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('1', DefaultValue.MaxLength);

        // Act
        var result = DefaultValue.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(DefaultValue.MaxLength);
    }

    [Fact]
    public void ToString_WithValidValue_ShouldReturnValue()
    {
        // Arrange
        var defaultValueString = "auto";
        var defaultValue = DefaultValue.Create(defaultValueString).Value;

        // Act
        var result = defaultValue.ToString();

        // Assert
        result.Should().Be(defaultValueString);
    }

    [Fact]
    public void ToString_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var defaultValue = DefaultValue.Create(null).Value;

        // Act
        var result = defaultValue.ToString();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("16:9")]
    [InlineData("medium")]
    [InlineData("true")]
    [InlineData("false")]
    public void Create_WithVariousDefaultFormats_ShouldReturnSuccess(string defaultFormat)
    {
        // Act
        var result = DefaultValue.Create(defaultFormat);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(defaultFormat);
    }

    [Fact]
    public void MaxLength_ShouldBe50()
    {
        // Assert
        DefaultValue.MaxLength.Should().Be(50);
    }
}