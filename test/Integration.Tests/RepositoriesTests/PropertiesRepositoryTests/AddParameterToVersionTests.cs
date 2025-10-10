using FluentAssertions;

namespace Integration.Tests.RepositoriesTests.PropertiesRepositoryTests;

public class AddParameterToVersionTests(MidjourneyDbFixture fixture) : RepositoryTestsBase(fixture)
{

    // Test Constants
    private const string TestPropertyName1 = "aspect";

    private const string TestParam1 = "--ar";
    private const string TestDefaultValue1 = "1:1";
    private const string TestDescription1 = "Aspect ratio parameter";

    // AddParameterToVersionAsync Tests
    [Fact]
    public async Task AddParameterToVersionAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var property = await CreateTestPropertyAsync(
            DefaultTestVersion1,
            TestPropertyName1,
            [TestParam1],
            TestDefaultValue1,
            null,
            null,
            TestDescription1);

        // Act
        var result = await PropertiesRepository.AddPropertyAsync(property, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.PropertyName.Value.Should().Be(TestPropertyName1);
        result.Value.Version.Value.Should().Be(DefaultTestVersion1);
        result.Value.DefaultValue?.Value.Should().Be(TestDefaultValue1);
        result.Value.Description?.Value.Should().Be(TestDescription1);
    }

    [Fact]
    public async Task AddParameterToVersionAsync_WithMinimalData_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var property = await CreateTestPropertyAsync(
            DefaultTestVersion1,
            TestPropertyName1,
            [TestParam1]);

        // Act
        var result = await PropertiesRepository.AddPropertyAsync(property, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.PropertyName.Value.Should().Be(TestPropertyName1);
        result.Value.Version.Value.Should().Be(DefaultTestVersion1);
        result.Value.DefaultValue.Should().BeNull();
        result.Value.MinValue.Should().BeNull();
        result.Value.MaxValue.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task AddParameterToVersionAsync_WithMultipleParameters_ShouldSucceed()
    {
        // Arrange
        await CreateAndSaveTestVersionAsync(DefaultTestVersion1);

        var property = await CreateTestPropertyAsync(
            DefaultTestVersion1,
            TestPropertyName1,
            [TestParam1, "--aspect", "--a"],
            TestDefaultValue1,
            null,
            null,
            TestDescription1);

        // Act
        var result = await PropertiesRepository.AddPropertyAsync(property, CancellationToken);

        // Assert
        AssertSuccessResult(result);
        result.Value.Parameters.Should().HaveCount(3);
        result.Value.Parameters.Should().Contain(p => p.Value == TestParam1);
        result.Value.Parameters.Should().Contain(p => p.Value == "--aspect");
        result.Value.Parameters.Should().Contain(p => p.Value == "--a");
    }
}