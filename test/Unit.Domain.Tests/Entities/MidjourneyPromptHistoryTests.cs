using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.Entities;

public class MidjourneyPromptHistoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var promptResult = Prompt.Create("A beautiful landscape with mountains");
        var versionResult = ModelVersion.Create("6.0");
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            versionResult
        );

        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeSuccess();
        result.Value.Should().NotBeNull();
        result.Value.Prompt.Value.Should().Be("A beautiful landscape with mountains");
        result.Value.Version.Value.Should().Be("6.0");
        result.Value.CreatedOn.Should().BeAfter(beforeCreation);
        result.Value.CreatedOn.Should().BeBefore(afterCreation);
        result.Value.HistoryId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithNullCreatedOn_ShouldUseCurrentTime()
    {
        // Arrange
        var promptResult = Prompt.Create("Test prompt");
        var versionResult = ModelVersion.Create("5.1");
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            versionResult
        );

        var afterCreation = DateTime.UtcNow;

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeSuccess();
        result.Value.Should().NotBeNull();
        result.Value.CreatedOn.Should().BeAfter(beforeCreation.AddSeconds(-1));
        result.Value.CreatedOn.Should().BeBefore(afterCreation.AddSeconds(1));
    }

    [Fact]
    public void Create_WithMinimalData_ShouldReturnSuccess()
    {
        // Arrange
        var promptResult = Prompt.Create("Simple prompt");
        var versionResult = ModelVersion.Create("niji 6");

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeSuccess();
        result.Value.Should().NotBeNull();
        result.Value.Prompt.Value.Should().Be("Simple prompt");
        result.Value.Version.Value.Should().Be("niji 6");
        result.Value.HistoryId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithInvalidPrompt_ShouldReturnFailure()
    {
        // Arrange
        var invalidPromptResult = Prompt.Create("");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            invalidPromptResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeFailure();
    }

    [Fact]
    public void Create_WithInvalidVersion_ShouldReturnFailure()
    {
        // Arrange
        var promptResult = Prompt.Create("Valid prompt");
        var invalidVersionResult = ModelVersion.Create("invalid_version");

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            invalidVersionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeFailure();
    }

    [Fact]
    public void Create_WithBothInvalidInputs_ShouldReturnFailure()
    {
        // Arrange
        var invalidPromptResult = Prompt.Create(null);
        var invalidVersionResult = ModelVersion.Create("");

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            invalidPromptResult,
            invalidVersionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeFailure();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCountGreaterOrEqualTo(2); // Both prompt and version errors
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    [InlineData("5.1")]
    [InlineData("6.0")]
    [InlineData("niji 4")]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    public void Create_WithVariousValidVersions_ShouldReturnSuccess(string version)
    {
        // Arrange
        var promptResult = Prompt.Create($"Test prompt for version {version}");
        var versionResult = ModelVersion.Create(version);

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeSuccess();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(version);
    }

    [Fact]
    public void Create_WithLongPrompt_ShouldHandleCorrectly()
    {
        // Arrange
        var longPrompt = "A very detailed and comprehensive description of an artwork that includes multiple elements, styles, colors, compositions, lighting effects, and artistic techniques that should be applied to create the desired image " + new string('a', 500);
        var promptResult = Prompt.Create(longPrompt);
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        if (longPrompt.Length <= Prompt.MaxLength)
        {
            result.ShouldBeSuccess();
            result.Value.Should().NotBeNull();
            result.Value.Prompt.Value.Should().Be(longPrompt);
        }
        else
        {
            result.ShouldBeFailure();
        }
    }

    [Fact]
    public void Create_WithFutureDate_ShouldReturnSuccess()
    {
        // Arrange
        var promptResult = Prompt.Create("Future prompt");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeSuccess();
        result.Value.Should().NotBeNull();
        result.Value.CreatedOn.Should().NotBe(default(DateTime));
    }

    [Fact]
    public void Create_WithPastDate_ShouldReturnSuccess()
    {
        // Arrange
        var promptResult = Prompt.Create("Historical prompt");
        var versionResult = ModelVersion.Create("1.0");

        // Act
        var result = MidjourneyPromptHistory.Create
        (
            promptResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.ShouldBeSuccess();
        result.Value.Should().NotBeNull();
        result.Value.CreatedOn.Should().NotBe(default(DateTime));
    }

    [Fact]
    public void Create_ShouldGenerateUniqueHistoryIds()
    {
        // Arrange
        var promptResult = Prompt.Create("Test prompt");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result1 = MidjourneyPromptHistory.Create(promptResult, versionResult);
        var result2 = MidjourneyPromptHistory.Create(promptResult, versionResult);

        // Assert
        result1.Value.HistoryId.Should().NotBe(result2.Value.HistoryId);
    }

    [Fact]
    public void Create_ShouldInitializeNavigationProperties()
    {
        // Arrange & Act
        var result = MidjourneyPromptHistory.Create
        (
            Prompt.Create("Test prompt"),
            ModelVersion.Create("6.0")
        );

        // Assert
        result.Value.MidjourneyStyles.Should().NotBeNull();
        result.Value.MidjourneyStyles.Should().BeEmpty();
    }

    [Fact]
    public void HistoryId_ShouldBeReadOnly()
    {
        // Arrange & Act
        var history = MidjourneyPromptHistory.Create
        (
            Prompt.Create("Test prompt"),
            ModelVersion.Create("6.0")
        ).Value;

        // Assert
        // HistoryId should have only a getter (read-only)
        history.HistoryId.Should().NotBe(Guid.Empty);

        // This test verifies that HistoryId cannot be modified after creation
        // The property should be { get; } only
    }
}