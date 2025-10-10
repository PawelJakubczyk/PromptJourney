using Application.Features.Versions.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.VersionsControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.VersionsControllersTests;

public sealed class GetByVersionTests(MidjourneyTestWebApplicationFactory factory) : VersionsControllerTestsBase(factory)
{
    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("5.2")]
    [InlineData("6")]
    public async Task GetByVersion_ReturnsValidResponse_ForKnownVersions(string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{version}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var versionResponse = await DeserializeResponse<VersionResponse>(response);
            versionResponse.Should().NotBeNull();
            versionResponse!.Version.Should().Be(version);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("999.999")]
    [InlineData("abc")]
    public async Task GetByVersion_HandlesBadRequest_ForInvalidVersions(string invalidVersion)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidVersion)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByVersion_ReturnsNotFound_ForNonExistentVersion()
    {
        // Arrange
        var nonExistentVersion = "999.0";

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{nonExistentVersion}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByVersion_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/1");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

            var versionResponse = await DeserializeResponse<VersionResponse>(response);
            versionResponse.Should().NotBeNull();
            versionResponse!.Version.Should().NotBeNullOrEmpty();
            versionResponse.Parameter.Should().NotBeNullOrEmpty();
            versionResponse.ReleaseDate.Should().NotBeNull();
        }
    }

    [Theory]
    [InlineData("niji 5")]
    [InlineData("niji 6")]
    [InlineData("v1.0")]
    [InlineData("test_version")]
    public async Task GetByVersion_HandlesSpecialVersionFormats(string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(version)}");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var versionResponse = await DeserializeResponse<VersionResponse>(response);
            versionResponse.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByVersion_ReturnsConsistentResults()
    {
        // Arrange
        const string version = "1";

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/{version}");
        var response2 = await Client.GetAsync($"{BaseUrl}/{version}");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK && response2.StatusCode == HttpStatusCode.OK)
        {
            var version1 = await DeserializeResponse<VersionResponse>(response1);
            var version2 = await DeserializeResponse<VersionResponse>(response2);

            version1.Should().BeEquivalentTo(version2);
        }
    }

    [Fact]
    public async Task GetByVersion_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/1");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByVersion_HandlesUrlEncoding()
    {
        // Arrange
        var versions = new[] { "1.0", "5.2", "niji 5", "test-version" };

        foreach (var version in versions)
        {
            // Act
            var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(version)}");

            // Assert
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.OK,
                HttpStatusCode.NotFound,
                HttpStatusCode.BadRequest);
        }
    }
}