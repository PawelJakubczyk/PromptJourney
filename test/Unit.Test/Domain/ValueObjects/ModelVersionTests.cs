using Domain.ValueObjects;

namespace Unit.Test.Domain.ValueObjects;

public class ModelVersionTests
{
    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    [InlineData("5.1")]
    [InlineData("6.0")]
    [InlineData("niji 4")]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    public void Create_WithValidVersions_ShouldReturnSuccess(string validVersion)
    {
        // Act
        var result = ModelVersion.Create(validVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validVersion);
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
        var result = ModelVersion.Create(invalidValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var tooLongValue = new string('1', ModelVersion.MaxLength + 1);

        // Act
        var result = ModelVersion.Create(tooLongValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithValueAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange - Creating a valid version at max length (like "niji 6.123")
        var maxLengthValue = "niji 6.123"; // 10 characters exactly

        // Act
        var result = ModelVersion.Create(maxLengthValue);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(maxLengthValue);
        result.Value.Value.Should().HaveLength(10);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("version")]
    [InlineData("1.0.0.0")]
    [InlineData("v1.0")]
    [InlineData("abc")]
    public void Create_WithInvalidVersionFormat_ShouldReturnFailure(string invalidVersion)
    {
        // Act
        var result = ModelVersion.Create(invalidVersion);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var versionString = "6.0";
        var version = ModelVersion.Create(versionString).Value;

        // Act
        var result = version.ToString();

        // Assert
        result.Should().Be(versionString);
    }

    [Fact]
    public void ModelVersionResult_ShouldReturnSameAsCreateCall()
    {
        // Arrange
        var versionString = "5.1";
        var version = ModelVersion.Create(versionString).Value;

        // Act
        var result = version.ModelVersionResult;

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(versionString);
    }

    [Fact]
    public void MaxLength_ShouldBe10()
    {
        // Assert
        ModelVersion.MaxLength.Should().Be(10);
    }
}