using Domain.ValueObjects;

namespace Unit.Test.Domain.ValueObjects;

public class MinValueTests
{
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("0.25")]
    [InlineData("1:3")]
    [InlineData("100")]
    public void Create_WithValidValues_ShouldReturnSuccess(string validValue)
    {
        // Act
        var result = MinValue.Create(validValue);

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
        var result = MinValue.Create(null);

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
        var result = MinValue.Create(whitespaceValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('1', MinValue.MaxLength + 1);

        // Act
        var result = MinValue.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('1', MinValue.MaxLength);

        // Act
        var result = MinValue.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(MinValue.MaxLength);
    }

    [Fact]
    public void ToString_WithValidValue_ShouldReturnValue()
    {
        // Arrange
        var minValueString = "0.5";
        var minValue = MinValue.Create(minValueString).Value;

        // Act
        var result = minValue.ToString();

        // Assert
        result.Should().Be(minValueString);
    }

    [Fact]
    public void ToString_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var minValue = MinValue.Create(null).Value;

        // Act
        var result = minValue.ToString();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("0.1")]
    [InlineData("10")]
    [InlineData("1000")]
    [InlineData("-5")]
    [InlineData("16:9")]
    public void Create_WithVariousNumericFormats_ShouldReturnSuccess(string numericValue)
    {
        // Act
        var result = MinValue.Create(numericValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(numericValue);
    }

    [Fact]
    public void MaxLength_ShouldBe50()
    {
        // Assert
        MinValue.MaxLength.Should().Be(50);
    }
}