using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.ValueObjects;

public class PromptTests
{
    [Theory]
    [InlineData("A beautiful sunset")]
    [InlineData("Cat sitting on a windowsill")]
    [InlineData("Modern architecture building")]
    [InlineData("Abstract art with vibrant colors")]
    public void Create_WithValidPrompts_ShouldReturnSuccess(string validPrompt)
    {
        // Act
        var result = Prompt.Create(validPrompt);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validPrompt);
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
        var result = Prompt.Create(invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('A', Prompt.MaxLength + 1);

        // Act
        var result = Prompt.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('A', Prompt.MaxLength);

        // Act
        var result = Prompt.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(Prompt.MaxLength);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var promptString = "Beautiful landscape with mountains";
        var prompt = Prompt.Create(promptString).Value;

        // Act
        var result = prompt.ToString();

        // Assert
        result.Should().Be(promptString);
    }

    [Theory]
    [InlineData("A detailed portrait of a person wearing a red jacket, photorealistic style, high resolution")]
    [InlineData("Fantasy castle on a floating island, magical atmosphere, digital art style")]
    [InlineData("Cyberpunk city at night with neon lights, futuristic vehicles, rain, atmospheric lighting")]
    public void Create_WithComplexPrompts_ShouldReturnSuccess(string complexPrompt)
    {
        // Act
        var result = Prompt.Create(complexPrompt);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(complexPrompt);
    }

    [Fact]
    public void Create_WithSpecialCharacters_ShouldReturnSuccess()
    {
        // Arrange
        var promptWithSpecialChars = "A scene with symbols: @#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var result = Prompt.Create(promptWithSpecialChars);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(promptWithSpecialChars);
    }

    [Fact]
    public void MaxLength_ShouldBe1000()
    {
        // Assert
        Prompt.MaxLength.Should().Be(1000);
    }
}