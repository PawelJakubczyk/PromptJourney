using Domain.ValueObjects;

namespace Unit.Test.Domain.ValueObjects;

public class ParamTests
{
    [Theory]
    [InlineData("--v")]
    [InlineData("--ar")]
    [InlineData("--q")]
    [InlineData("--s")]
    [InlineData("--chaos")]
    [InlineData("--stylize")]
    [InlineData("--weird")]
    public void Create_WithValidParameters_ShouldReturnSuccess(string validParam)
    {
        // Act
        var result = Param.Create(validParam);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validParam);
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
        var result = Param.Create(invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('A', Param.MaxLength + 1);

        // Act
        var result = Param.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var maxLengthValue = new string('A', Param.MaxLength);

        // Act
        var result = Param.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(Param.MaxLength);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var paramString = "--aspect";
        var param = Param.Create(paramString).Value;

        // Act
        var result = param.ToString();

        // Assert
        result.Should().Be(paramString);
    }

    [Theory]
    [InlineData("--v 1.0")]
    [InlineData("--ar 16:9")]
    [InlineData("--q 2")]
    [InlineData("--stylize 100")]
    public void Create_WithParametersWithValues_ShouldReturnSuccess(string paramWithValue)
    {
        // Act
        var result = Param.Create(paramWithValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(paramWithValue);
    }

    [Fact]
    public void MaxLength_ShouldBe100()
    {
        // Assert
        Param.MaxLength.Should().Be(100);
    }
}