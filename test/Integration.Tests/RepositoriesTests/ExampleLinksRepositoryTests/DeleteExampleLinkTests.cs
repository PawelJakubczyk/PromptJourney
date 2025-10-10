using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.ExampleLinksRepositoryTests;

public class DeleteExampleLinkTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // DeleteExampleLinkAsync Tests
    [Fact]
    public async Task DeleteExampleLinkAsync_WithExistingLink_ShouldSucceed()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        var link = ExampleLink.Create(DefaultTestLink1).Value;

        // Act
        var result = await ExampleLinkRepository.DeleteExampleLinkAsync(link, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Link.Value.Should().Be(DefaultTestLink1);

        // Verify it's been deleted
        var checkResult = await ExampleLinkRepository.CheckExampleLinkExistsAsync(link, CancellationToken);
        checkResult.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteExampleLinkAsync_WithNonExistentLink_ShouldFail()
    {
        // Arrange
        var link = ExampleLink.Create("https://nonexistent.com/test.jpg").Value;

        // Act
        var result = await ExampleLinkRepository.DeleteExampleLinkAsync(link, CancellationToken);

        // Assert
        AssertFailureResult(result, "Failed to delete example link");
    }

    [Fact]
    public async Task DeleteExampleLinkAsync_AfterDeletion_ShouldNotExistInDatabase()
    {
        // Arrange
        await CreateBasicTestDataAsync();
        await CreateAndSaveTestExampleLinkAsync(DefaultTestLink1, DefaultTestStyleName1, DefaultTestVersion1);

        var link = ExampleLink.Create(DefaultTestLink1).Value;

        // Act
        var deleteResult = await ExampleLinkRepository.DeleteExampleLinkAsync(link, CancellationToken);

        // Assert
        AssertSuccessResult(deleteResult);

        // Verify it doesn't exist anymore
        var allLinks = await ExampleLinkRepository.GetAllExampleLinksAsync(CancellationToken);
        allLinks.Value.Should().NotContain(l => l.Link.Value == DefaultTestLink1);
    }
}