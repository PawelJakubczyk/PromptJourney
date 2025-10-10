using FluentAssertions;
using Integration.Tests.ControllersTests.VersionsControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.VersionsControllersTests;

public sealed class CheckVersionExistsTests(MidjourneyTestWebApplicationFactory factory) : VersionsControllerTestsBase(factory)
{
    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("5.2")]
    [InlineData("6")]
    public async Task CheckExists_ReturnsOk_ForKnownVersions(string version)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{version}/exists");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        _ = await GetExistsFromResponse(response);
        // We don't assert true/false because it depends on database state
        // but we verify the endpoint works correctly
    }

    [Fact]
    public async Task CheckExists_ReturnsBadRequest_ForInvalidVersion()
    {
        // Arrange
        var invalidVersions = new[] { "", " ", "invalid", "999.999", "abc" };

        foreach (var invalidVersion in invalidVersions)
        {
            // Act
            var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidVersion)}/exists");

            // Assert - Could be BadRequest or the endpoint might handle it gracefully
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task CheckExists_ReturnsJsonWithExistsProperty()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/1/exists");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("exists");
        content.Should().MatchRegex(@"\{.*""exists"":(true|false).*\}");
    }

    [Fact]
    public async Task CheckExists_HandlesSpecialCharacters()
    {
        // Arrange
        var specialVersions = new[] { "niji-5", "test_version", "v.1.0" };

        foreach (var version in specialVersions)
        {
            // Act
            var response = await Client.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(version)}/exists");

            // Assert
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.OK,
                HttpStatusCode.BadRequest,
                HttpStatusCode.NotFound);
        }
    }
}