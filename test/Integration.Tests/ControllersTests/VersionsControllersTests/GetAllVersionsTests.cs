using Application.UseCases.Versions.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.VersionsControllersTests.Base;
using System.Net;

namespace Integration.Tests.ControllersTests.VersionsControllersTests;

public sealed class GetAllVersionsTests(MidjourneyTestWebApplicationFactory factory) : VersionsControllerTestsBase(factory)
{
    [Fact]
    public async Task GetAll_ReturnsOk_WhenVersionsExist()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var versions = await DeserializeResponse<List<VersionResponse>>(response);
        versions.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_ReturnsValidJsonStructure()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        AssertOkResponse<VersionResponse>(response);

        var versions = await DeserializeResponse<List<VersionResponse>>(response);
        versions.Should().NotBeNull();

        if (versions.Count != 0)
        {
            var firstVersion = versions.First();
            firstVersion.Version.Should().NotBeNullOrEmpty();
            firstVersion.Parameter.Should().NotBeNullOrEmpty();
            firstVersion.ReleaseDate.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetAll_ReturnsConsistentResults()
    {
        // Act
        var response1 = await Client.GetAsync(BaseUrl);
        var response2 = await Client.GetAsync(BaseUrl);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var versions1 = await DeserializeResponse<List<VersionResponse>>(response1);
        var versions2 = await DeserializeResponse<List<VersionResponse>>(response2);

        versions1.Should().BeEquivalentTo(versions2);
    }

    [Fact]
    public async Task GetAll_PerformanceTest_CompletesWithinReasonableTime()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
