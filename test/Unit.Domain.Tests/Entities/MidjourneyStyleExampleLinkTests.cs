using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;

namespace Unit.Domain.Tests.Entities;

public class MidjourneyStyleExampleLinkTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var linkResult = ExampleLink.Create("https://example.com/image.jpg");
        var styleNameResult = StyleName.Create("Abstract Art");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            styleNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Link.Value.Should().Be("https://example.com/image.jpg");
        result.Value.StyleName.Value.Should().Be("Abstract Art");
        result.Value.Version.Value.Should().Be("6.0");
    }

    [Fact]
    public void Create_WithInvalidLink_ShouldReturnFailure()
    {
        // Arrange
        var invalidLinkResult = ExampleLink.Create("invalid-url");
        var styleNameResult = StyleName.Create("Test Style");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            invalidLinkResult,
            styleNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidStyleName_ShouldReturnFailure()
    {
        // Arrange
        var linkResult = ExampleLink.Create("https://example.com/test.jpg");
        var invalidStyleNameResult = StyleName.Create("");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            invalidStyleNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidVersion_ShouldReturnFailure()
    {
        // Arrange
        var linkResult = ExampleLink.Create("https://example.com/test.jpg");
        var styleNameResult = StyleName.Create("Test Style");
        var invalidVersionResult = ModelVersion.Create("invalid_version");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            styleNameResult,
            invalidVersionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithAllInvalidInputs_ShouldReturnFailureWithMultipleErrors()
    {
        // Arrange
        var invalidLinkResult = ExampleLink.Create(null);
        var invalidStyleNameResult = StyleName.Create(null);
        var invalidVersionResult = ModelVersion.Create("");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            invalidLinkResult,
            invalidStyleNameResult,
            invalidVersionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCountGreaterOrEqualTo(3); // All three inputs are invalid
    }

    [Theory]
    [InlineData("https://example.com/image1.jpg", "Cyberpunk", "6.0")]
    [InlineData("https://test.com/photo.png", "Minimalist", "5.1")]
    [InlineData("http://sample.org/art.gif", "Abstract", "niji 6")]
    [InlineData("https://cdn.example.com/picture.jpeg", "Realistic", "niji 5")]
    public void Create_WithVariousValidCombinations_ShouldReturnSuccess(string url, string styleName, string version)
    {
        // Arrange
        var linkResult = ExampleLink.Create(url);
        var styleNameResult = StyleName.Create(styleName);
        var versionResult = ModelVersion.Create(version);

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            styleNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Link.Value.Should().Be(url);
        result.Value.StyleName.Value.Should().Be(styleName);
        result.Value.Version.Value.Should().Be(version);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    [InlineData("5.1")]
    [InlineData("6.0")]
    [InlineData("niji 4")]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    public void Create_WithDifferentVersions_ShouldReturnSuccess(string version)
    {
        // Arrange
        var linkResult = ExampleLink.Create("https://example.com/test.jpg");
        var styleNameResult = StyleName.Create("Test Style");
        var versionResult = ModelVersion.Create(version);

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            styleNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Version.Value.Should().Be(version);
    }

    [Fact]
    public void Create_WithLongValidUrl_ShouldReturnSuccess()
    {
        // Arrange
        var longPath = new string('a', 100);
        var longUrl = $"https://example.com/very/long/path/{longPath}/image.jpg";
        var linkResult = ExampleLink.Create(longUrl);
        var styleNameResult = StyleName.Create("Test Style");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            styleNameResult,
            versionResult
        );

        // Assert
        if (longUrl.Length <= ExampleLink.MaxLength)
        {
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Link.Value.Should().Be(longUrl);
        }
    }

    [Fact]
    public void Create_WithLongStyleName_ShouldReturnSuccess()
    {
        // Arrange
        var linkResult = ExampleLink.Create("https://example.com/test.jpg");
        var longStyleName = new string('A', 100) + " Style";
        var styleNameResult = StyleName.Create(longStyleName);
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            styleNameResult,
            versionResult
        );

        // Assert
        if (longStyleName.Length <= StyleName.MaxLength)
        {
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.StyleName.Value.Should().Be(longStyleName);
        }
    }

    [Theory]
    [InlineData("https://example.com/image.jpg")]
    [InlineData("https://cdn.website.com/assets/photo.png")]
    [InlineData("http://test.org/picture.gif")]
    public void Create_WithDifferentValidUrlSchemes_ShouldReturnSuccess(string url)
    {
        // Arrange
        var linkResult = ExampleLink.Create(url);
        var styleNameResult = StyleName.Create("Test Style");
        var versionResult = ModelVersion.Create("6.0");

        // Act
        var result = MidjourneyStyleExampleLink.Create
        (
            linkResult,
            styleNameResult,
            versionResult
        );

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Link.Value.Should().Be(url);
    }
}