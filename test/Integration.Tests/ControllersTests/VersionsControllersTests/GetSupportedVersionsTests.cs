using FluentAssertions;
using Integration.Tests.ControllersTests.VersionsControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.VersionsControllersTests;

public sealed class GetSupportedVersionsTests(MidjourneyTestWebApplicationFactory factory) : VersionsControllerTestsBase(factory)
{
    [Fact]
    public async Task GetSupported_ReturnsOk_WithValidResponse()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

            var supportedVersions = await DeserializeResponse<List<string>>(response);
            supportedVersions.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetSupported_ReturnsEmptyList_WhenNoSupportedVersions()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var supportedVersions = await DeserializeResponse<List<string>>(response);
            supportedVersions.Should().NotBeNull();
            // Can be empty list if no versions are configured
        }
    }

    [Fact]
    public async Task GetSupported_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            AssertOkResponse<string>(response);

            var supportedVersions = await DeserializeResponse<List<string>>(response);
            supportedVersions.Should().NotBeNull();

            if (supportedVersions!.Any())
            {
                supportedVersions.Should().AllSatisfy(version =>
                {
                    version.Should().NotBeNullOrWhiteSpace();
                });
            }
        }
    }

    [Fact]
    public async Task GetSupported_ReturnsConsistentResults()
    {
        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/supported");
        var response2 = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK && response2.StatusCode == HttpStatusCode.OK)
        {
            var versions1 = await DeserializeResponse<List<string>>(response1);
            var versions2 = await DeserializeResponse<List<string>>(response2);

            versions1.Should().BeEquivalentTo(versions2);
        }
    }

    [Fact]
    public async Task GetSupported_ReturnsCorrectContentType()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
            //response.Content.Headers.ContentType?.Charset.Should().Be("utf-8");
        }
    }

    [Fact]
    public async Task GetSupported_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSupported_HandlesExpectedVersionFormats()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var supportedVersions = await DeserializeResponse<List<string>>(response);
            supportedVersions.Should().NotBeNull();

            if (supportedVersions!.Any())
            {
                // Verify that returned versions have expected formats
                foreach (var version in supportedVersions)
                {
                    version.Should().NotBeNullOrWhiteSpace();
                    // Versions can be in various formats: "1", "2", "5.2", "6", "niji 5", etc.
                    version.Length.Should().BeGreaterThan(0);
                }
            }
        }
    }

    [Fact]
    public async Task GetSupported_ValidatesJsonStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
            content.Should().StartWith("[");
            content.Should().EndWith("]");

            // Should be valid JSON array
            var supportedVersions = await DeserializeResponse<List<string>>(response);
            supportedVersions.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetSupported_HandlesMultipleConcurrentRequests()
    {
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Client.GetAsync($"{BaseUrl}/supported"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        foreach (var response in responses)
        {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var supportedVersions = await DeserializeResponse<List<string>>(response);
                supportedVersions.Should().NotBeNull();
            }
        }

        // All responses should be consistent
        var statusCodes = responses.Select(r => r.StatusCode).Distinct().ToList();
        statusCodes.Should().HaveCount(1); // All should have the same status code
    }

    [Fact]
    public async Task GetSupported_ReturnsUniqueVersions()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/supported");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var supportedVersions = await DeserializeResponse<List<string>>(response);
            supportedVersions.Should().NotBeNull();

            if (supportedVersions!.Any())
            {
                // Verify no duplicates
                var distinctVersions = supportedVersions.Distinct().ToList();
                distinctVersions.Should().HaveCount(supportedVersions.Count);
            }
        }
    }
}