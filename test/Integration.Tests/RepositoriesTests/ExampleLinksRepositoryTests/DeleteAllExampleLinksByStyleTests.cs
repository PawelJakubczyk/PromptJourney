using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class DeleteAllExampleLinksByStyleTests : RepositoryTestsBase
{
    public DeleteAllExampleLinksByStyleTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // DeleteAllExampleLinksByStyleAsync Tests
    [Fact]
    public async Task DeleteAllExampleLinksByStyleAsync_WithExistingStyle_ShouldDeleteAllMatchingLinks()
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
        var result = await ExampleLinkRepository.DeleteAllExampleLinksByStyleAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().Be(2); // Should delete 2 links for DefaultTestStyleName1

        // Verify deletion by checking remaining links
        var remainingLinks = await ExampleLinkRepository.GetAllExampleLinksAsync(CancellationToken);
        AssertSuccessResult(remainingLinks);
        remainingLinks.Value.Should().HaveCount(1);
        remainingLinks.Value[0].StyleName.Value.Should().Be(DefaultTestStyleName2);
    }

    [Fact]
    public async Task DeleteAllExampleLinksByStyleAsync_WithNonExistentStyle_ShouldReturnZero()
    {
        // Arrange
        var nonExistentStyle = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await ExampleLinkRepository.DeleteAllExampleLinksByStyleAsync(nonExistentStyle, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().Be(0);
    }

    [Fact]
    public async Task DeleteAllExampleLinksByStyleAsync_WithStyleHavingNoLinks_ShouldReturnZero()
    {
        // Arrange
        var styleName = "TestStyleWithNoLinks";
        await CreateAndSaveTestStyleAsync(styleName);

        var styleNameVo = StyleName.Create(styleName).Value;

        // Act
        var result = await ExampleLinkRepository.DeleteAllExampleLinksByStyleAsync(styleNameVo, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().Be(0);
    }
}