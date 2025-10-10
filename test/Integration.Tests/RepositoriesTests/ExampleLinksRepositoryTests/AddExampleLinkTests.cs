using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class AddExampleLinkTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // AddExampleLinkAsync Tests
    [Fact]
    public async Task AddExampleLinkAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        await CreateBasicTestDataAsync();

        var exampleLink = MidjourneyStyleExampleLink.Create(
            ExampleLink.Create(DefaultTestLink1),
            StyleName.Create(DefaultTestStyleName1),
            ModelVersion.Create(DefaultTestVersion1)
        ).Value;

        // Act
        var result = await ExampleLinkRepository.AddExampleLinkAsync(exampleLink, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Link.Value.Should().Be(DefaultTestLink1);
        result.Value.StyleName.Value.Should().Be(DefaultTestStyleName1);
        result.Value.Version.Value.Should().Be(DefaultTestVersion1);
    }

    [Fact]
    public async Task AddExampleLinkAsync_WithDuplicateLink_ShouldFail()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        var duplicateExampleLink = MidjourneyStyleExampleLink.Create(
            ExampleLink.Create(DefaultTestLink1),
            StyleName.Create(DefaultTestStyleName1),
            ModelVersion.Create(DefaultTestVersion1)
        ).Value;

        // Act
        var result = await ExampleLinkRepository.AddExampleLinkAsync(duplicateExampleLink, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Theory]
    [InlineData("https://example.com/image1.jpg")]
    [InlineData("https://cdn.example.com/assets/image.png")]
    [InlineData("http://test.com/photo.jpeg")]
    [InlineData("https://media.website.org/picture.gif")]
    public async Task AddExampleLinkAsync_WithVariousValidUrls_ShouldSucceed(string url)
    {
        // Arrange
        var uniqueStyleName = $"TestStyle_{Guid.NewGuid():N}";
        await CreateAndSaveTestVersionAsync("6.0");
        await CreateAndSaveTestStyleAsync(uniqueStyleName);

        var exampleLink = MidjourneyStyleExampleLink.Create(
            ExampleLink.Create(url),
            StyleName.Create(uniqueStyleName),
            ModelVersion.Create("6.0")
        ).Value;

        // Act
        var result = await ExampleLinkRepository.AddExampleLinkAsync(exampleLink, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Link.Value.Should().Be(url);
    }

    [Fact]
    public async Task AddExampleLinkAsync_WithNonExistentVersion_ShouldFail()
    {
        // Arrange
        var styleName = "TestStyle";
        await CreateAndSaveTestStyleAsync(styleName);

        var exampleLink = MidjourneyStyleExampleLink.Create(
            ExampleLink.Create("https://example.com/test.jpg"),
            StyleName.Create(styleName),
            ModelVersion.Create("999.0")
        ).Value;

        // Act
        var result = await ExampleLinkRepository.AddExampleLinkAsync(exampleLink, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }

    [Fact]
    public async Task AddExampleLinkAsync_WithNonExistentStyle_ShouldFail()
    {
        // Arrange
        var modelVersion = "6.0";
        await CreateAndSaveTestVersionAsync(modelVersion);

        var exampleLink = MidjourneyStyleExampleLink.Create(
            ExampleLink.Create("https://example.com/test.jpg"),
            StyleName.Create("NonExistentStyle"),
            ModelVersion.Create(modelVersion)
        ).Value;

        // Act
        var result = await ExampleLinkRepository.AddExampleLinkAsync(exampleLink, CancellationToken);

        // Assert
        AssertFailureResult(result);
    }
}