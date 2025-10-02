using Domain.ValueObjects;
using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.StylesRepositoryTests;

public sealed class GetStyleByNameTests : RepositoryTestsBase
{
    public GetStyleByNameTests(MidjourneyDbFixture fixture) : base(fixture)
    {
    }

    // Test Constants
    private const string TestStyleType1 = "Abstract";

    [Fact]
    public async Task GetStyleByNameAsync_WithExistingStyle_ShouldReturnStyle()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1, TestStyleType1);
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await StylesRepository.GetStyleByNameAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be(DefaultTestStyleName1);
        result.Value.Type.Value.Should().Be(TestStyleType1);
        result.Value.Description!.Value.Should().Be($"Test style {DefaultTestStyleName1}");
    }

    [Fact]
    public async Task GetStyleByNameAsync_WithNonExistentStyle_ShouldReturnNull()
    {
        // Arrange
        var styleName = StyleName.Create("NonExistentStyle").Value;

        // Act
        var result = await StylesRepository.GetStyleByNameAsync(styleName, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetStyleByNameAsync_WithEmptyDatabase_ShouldReturnNull()
    {
        // Arrange
        var styleName = StyleName.Create(DefaultTestStyleName1).Value;

        // Act
        var result = await StylesRepository.GetStyleByNameAsync(styleName, CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetStyleByNameAsync_WithMultipleStylesExists_ShouldReturnCorrectOne()
    {
        // Arrange
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName1, TestStyleType1);
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName2, "Realistic");
        await CreateAndSaveTestStyleAsync(DefaultTestStyleName3, "Minimalist");

        var styleName = StyleName.Create(DefaultTestStyleName2).Value;

        // Act
        var result = await StylesRepository.GetStyleByNameAsync(styleName, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.StyleName.Value.Should().Be(DefaultTestStyleName2);
        result.Value.Type.Value.Should().Be("Realistic");
    }
}