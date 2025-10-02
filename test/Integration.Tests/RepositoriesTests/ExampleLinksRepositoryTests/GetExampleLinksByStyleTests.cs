using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class GetExampleLinksByStyleTests : RepositoryTestsBase
{
    public GetExampleLinksByStyleTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // GetExampleLinksByStyleAsync Tests
    [Fact]
    public async Task GetExampleLinksByStyleAsync_WithExistingStyle_ShouldReturnMatchingLinks()
    {
        // Arrange
        var (versions, styles) = await CreateMultipleTestDataAsync();

        var linkData = new[]
        {
            (DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1),
            (DefaultTestLink2, DefaultTestStyleName1, DefaultTestVersion2),
            (DefaultTestLink3, DefaultTestStyleName2, DefaultTestVersion1)
        };
        await CreateAndSaveMultipleExampleLinksAsync(linkData);

        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await ExampleLinkRepository.GetExampleLinksByStyleAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(link => link.StyleName.Value.Should().Be(DefaultTestStyleName1));
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink1);
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink2);
    }

    [Fact]
    public async Task GetExampleLinksByStyleAsync_WithNonExistentStyle_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        var nonExistentStyle = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await ExampleLinkRepository.GetExampleLinksByStyleAsync(nonExistentStyle, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetExampleLinksByStyleAsync_WithStyleHavingNoLinks_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await ExampleLinkRepository.GetExampleLinksByStyleAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }
}