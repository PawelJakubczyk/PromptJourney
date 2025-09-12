using Domain.ValueObjects;

namespace Unit.Test.Domain.ValueObjects;

public class StyleTypeTests
{
    [Theory]
    [InlineData("Abstract")]
    [InlineData("Realistic")]
    [InlineData("Minimalist")]
    [InlineData("Photorealistic")]
    [InlineData("Cartoon")]
    [InlineData("Vintage")]
    [InlineData("Modern")]
    public void Create_WithValidValues_ShouldReturnSuccess(string validValue)
    {
        // Act
        var result = StyleType.Create(validValue);

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
        var result = StyleType.Create(invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('A', StyleType.MaxLength + 1);

        // Act
        var result = StyleType.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('A', StyleType.MaxLength);

        // Act
        var result = StyleType.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(StyleType.MaxLength);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var typeString = "Abstract";
        var styleType = StyleType.Create(typeString).Value;

        // Act
        var result = styleType.ToString();

        // Assert
        result.Should().Be(typeString);
    }

    [Fact]
    public void MaxLength_ShouldBe100()
    {
        // Assert
        StyleType.MaxLength.Should().Be(100);
    }
}