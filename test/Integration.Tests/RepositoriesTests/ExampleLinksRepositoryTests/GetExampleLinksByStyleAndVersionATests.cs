using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class GetExampleLinksByStyleAndVersionTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // GetExampleLinksByStyleAndVersionAsync Tests
    [Fact]
    public async Task GetExampleLinksByStyleAndVersionAsync_WithExistingStyleAndVersion_ShouldReturnMatchingLinks()
    {
        // Arrange
        var (_, _) = await CreateMultipleTestDataAsync();

        var linkData = new[]
        {
            (DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1),
            (DefaultTestLink2, DefaultTestStyleName2, DefaultTestVersion1),
            (DefaultTestLink3, DefaultTestStyleName1, DefaultTestVersion2)
        };
        await CreateAndSaveMultipleExampleLinksAsync(linkData);

        var styleName = StyleName.Create(DefaultTestStyleName1).Value;
        var version = ModelVersion.Create(DefaultTestVersion1).Value;

        // Act
        var result = await ExampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(1);
        result.Value[0].StyleName.Value.Should().Be(DefaultTestStyleName1);
        result.Value[0].Version.Value.Should().Be(DefaultTestVersion1);
        result.Value[0].Link.Value.Should().Be(DefaultTestLink1);
    }

    [Fact]
    public async Task GetExampleLinksByStyleAndVersionAsync_WithNonExistentCombination_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        var styleName = StyleName.Create("NonExistentStyle").Value;
        var version = ModelVersion.Create("7.0").Value;

        // Act
        var result = await ExampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleName, version, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExampleLinksByStyleAndVersionAsync_WithMultipleMatchingLinks_ShouldReturnAllMatches()
    {
        // Arrange
        await CreateBasicTestDataAsync();

        // Create multiple links for same style and version
        var linkUrls = new[]
        {
            "https://example.com/image1.jpg",
            "https://example.com/image2.jpg"
        };

        foreach (var linkUrl in linkUrls)
        {
            await CreateAndSaveTestExampleLinkAsync(linkUrl, DefaultTestStyleName1, DefaultTestVersion1);
        }

        var styleNameVo = StyleName.Create(DefaultTestStyleName1).Value;
        var versionVo = ModelVersion.Create(DefaultTestVersion1).Value;

        // Act
        var result = await ExampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(styleNameVo, versionVo, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(link =>
        {
            link.StyleName.Value.Should().Be(DefaultTestStyleName1);
            link.Version.Value.Should().Be(DefaultTestVersion1);
        });
    }
}