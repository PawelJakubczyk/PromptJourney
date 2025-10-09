using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

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
    public void Create_WithInvalidValues_ShouldReturnFailure(string invalidValue)
    {
        // Act
        var result = StyleType.Create(invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Custom")]
    [InlineData("StyleReferences")]
    [InlineData("Personalization")]
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
    public void Create_WithNullOrWhitespaceValue_ShouldReturnFailure(string? invalidValue)
    {
        // Act
        var result = StyleType.Create(invalidValue!);

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
    public void Create_WithValueAtMaxLength_ShouldSuccessInLenghtValidation()
    {
        // Arrange
        var maxLengthValue = new string('A', StyleType.MaxLength);

        // Act
        var result = StyleType.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        var errorMessages = result.Errors.Select(e => e.ToString()).ToList();
        errorMessages.Should().NotContain
        (
            $"DomainError with Message='{nameof(StyleType)}: {maxLengthValue} cannot be longer than {StyleType.MaxLength} characters.'"
        );
    }

    [Fact]
    public void Create_WithValueAboveMaxLength_ShouldFailuedInLenghtValidation()
    {
        // Arrange
        var maxLengthValue = new string('A', StyleType.MaxLength + 5);

        // Act
        var result = StyleType.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors[0].Message.Should().Be($"StyleType: '{maxLengthValue}' cannot be longer than 30 characters.");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var typeString = "Personalization";
        var styleType = StyleType.Create(typeString).Value;

        // Act
        var result = styleType.ToString();

        // Assert
        result.Should().Be(typeString);
    }
}