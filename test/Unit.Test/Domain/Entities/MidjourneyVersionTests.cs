using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;

namespace Unit.Test.Domain.Entities;

public class MidjourneyVersionTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var versionResult = ModelVersion.Create("6.0");
        var parameterResult = Param.Create("--v 6.0");
        var releaseDate = DateTime.UtcNow.AddDays(-30);
        var descriptionResult = Description.Create("Test version description");

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult,
            releaseDate,
            descriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be("6.0");
        result.Value.Parameter.Value.Should().Be("--v 6.0");
        result.Value.ReleaseDate.Should().Be(releaseDate);
        result.Value.Description?.Value.Should().Be("Test version description");
    }

    [Fact]
    public void Create_WithNullReleaseDate_ShouldReturnSuccess()
    {
        // Arrange
        var versionResult = ModelVersion.Create("5.1");
        var parameterResult = Param.Create("--v 5.1");
        var descriptionResult = Description.Create("Test description");

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult,
            null,
            descriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be("5.1");
        result.Value.Parameter.Value.Should().Be("--v 5.1");
        result.Value.ReleaseDate.Should().BeNull();
        result.Value.Description?.Value.Should().Be("Test description");
    }

    [Fact]
    public void Create_WithNullDescription_ShouldReturnSuccess()
    {
        // Arrange
        var versionResult = ModelVersion.Create("niji 6");
        var parameterResult = Param.Create("--niji 6");
        var releaseDate = DateTime.UtcNow;

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult,
            releaseDate,
            null
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be("niji 6");
        result.Value.Parameter.Value.Should().Be("--niji 6");
        result.Value.ReleaseDate.Should().Be(releaseDate);
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var versionResult = ModelVersion.Create("1.0");
        var parameterResult = Param.Create("--v 1.0");

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be("1.0");
        result.Value.Parameter.Value.Should().Be("--v 1.0");
        result.Value.ReleaseDate.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithInvalidVersion_ShouldReturnFailure()
    {
        // Arrange
        var invalidVersionResult = ModelVersion.Create("invalid_version");
        var parameterResult = Param.Create("--v test");

        // Act
        var result = MidjourneyVersion.Create
        (
            invalidVersionResult,
            parameterResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidParameter_ShouldReturnFailure()
    {
        // Arrange
        var versionResult = ModelVersion.Create("6.0");
        var invalidParameterResult = Param.Create(null);

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            invalidParameterResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidDescription_ShouldReturnFailure()
    {
        // Arrange
        var versionResult = ModelVersion.Create("6.0");
        var parameterResult = Param.Create("--v 6.0");
        var invalidDescriptionResult = Description.Create("");

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult,
            null,
            invalidDescriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("1.0", "--v 1.0")]
    [InlineData("2.0", "--v 2.0")]
    [InlineData("5.1", "--v 5.1")]
    [InlineData("6.0", "--v 6.0")]
    [InlineData("niji 4", "--niji 4")]
    [InlineData("niji 5", "--niji 5")]
    [InlineData("niji 6", "--niji 6")]
    public void Create_WithVariousValidVersions_ShouldReturnSuccess(string version, string parameter)
    {
        // Arrange
        var versionResult = ModelVersion.Create(version);
        var parameterResult = Param.Create(parameter);
        var description = $"Description for {version}";
        var descriptionResult = Description.Create(description);

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult,
            DateTime.UtcNow,
            descriptionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(version);
        result.Value.Parameter.Value.Should().Be(parameter);
        result.Value.Description?.Value.Should().Be(description);
    }

    [Fact]
    public void Create_WithFutureReleaseDate_ShouldReturnSuccess()
    {
        // Arrange
        var versionResult = ModelVersion.Create("7.0");
        var parameterResult = Param.Create("--v 7.0");
        var futureDate = DateTime.UtcNow.AddYears(1);

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult,
            futureDate
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ReleaseDate.Should().Be(futureDate);
    }

    [Fact]
    public void Create_WithPastReleaseDate_ShouldReturnSuccess()
    {
        // Arrange
        var versionResult = ModelVersion.Create("1.0");
        var parameterResult = Param.Create("--v 1.0");
        var pastDate = DateTime.UtcNow.AddYears(-2);

        // Act
        var result = MidjourneyVersion.Create
        (
            versionResult,
            parameterResult,
            pastDate
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ReleaseDate.Should().Be(pastDate);
    }
}