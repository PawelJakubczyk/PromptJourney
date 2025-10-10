using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class GetAllExampleLinksTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // GetAllExampleLinksAsync Tests
    [Fact]
    public async Task GetAllExampleLinksAsync_WithMultipleLinks_ShouldReturnAllLinks()
    {
        // Arrange
        var (versions, styles) = await CreateMultipleTestDataAsync();

        var linkData = new[]
        {
            (DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1),
            (DefaultTestLink2, DefaultTestStyleName2, DefaultTestVersion2),
            (DefaultTestLink3, DefaultTestStyleName3, DefaultTestVersion3)
        };
        await CreateAndSaveMultipleExampleLinksAsync(linkData);

        // Act
        var result = await ExampleLinkRepository.GetAllExampleLinksAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().HaveCount(3);
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink1);
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink2);
        result.Value.Should().Contain(link => link.Link.Value == DefaultTestLink3);
    }

    [Fact]
    public async Task GetAllExampleLinksAsync_WithNoLinks_ShouldReturnEmptyList()
    {
        // Act
        var result = await ExampleLinkRepository.GetAllExampleLinksAsync(CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Should().BeEmpty();
    }
}