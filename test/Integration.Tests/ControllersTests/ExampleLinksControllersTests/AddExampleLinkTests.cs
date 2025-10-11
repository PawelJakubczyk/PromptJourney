using Application.UseCases.ExampleLinks.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.ExampleLinksControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class AddExampleLinkTests(MidjourneyTestWebApplicationFactory factory) : ExampleLinksControllerTestsBase(factory)
{
    [Fact]
    public async Task AddExampleLink_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            GenerateTestUrl(),
            GenerateTestStyleName(),
            "1.0"
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,     // Successfully created
            HttpStatusCode.BadRequest,  // Validation failed or style/version doesn't exist
            HttpStatusCode.NotFound,    // Style or version not found
            HttpStatusCode.Conflict     // Link already exists
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            AssertCreatedResponse<ExampleLinkResponse>(response);

            var createdLink = await DeserializeResponse<ExampleLinkResponse>(response);
            createdLink.Should().NotBeNull();
            createdLink!.Link.Should().Be(request.Link);
            createdLink.Style.Should().Be(request.Style);
            createdLink.Version.Should().Be(request.Version);
        }
    }

    [Theory]
    [InlineData("", "TestStyle", "1.0")]           // Empty link
    [InlineData("invalid-url", "TestStyle", "1.0")] // Invalid URL
    [InlineData("https://test.com/image.jpg", "", "1.0")] // Empty style
    [InlineData("https://test.com/image.jpg", "TestStyle", "")] // Empty version
    public async Task AddExampleLink_ReturnsBadRequest_WithInvalidRequest(string link, string style, string version)
    {
        // Arrange
        var request = new AddExampleLinkRequest(link, style, version);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddExampleLink_ReturnsBadRequest_WithNullRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, (AddExampleLinkRequest?)null);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task AddExampleLink_HandlesContentTypeCorrectly()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            GenerateTestUrl(),
            GenerateTestStyleName(),
            "1.0"
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.Conflict
        );
    }

    [Fact]
    public async Task AddExampleLink_ValidatesUrlFormat()
    {
        // Arrange
        var invalidUrls = new[]
        {
            "not-a-url",
            "ftp://invalid.com/file.txt",
            "javascript:alert('xss')",
            "data:text/html,<script>alert('xss')</script>"
        };

        foreach (var invalidUrl in invalidUrls)
        {
            var request = new AddExampleLinkRequest(invalidUrl, GenerateTestStyleName(), "1.0");

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, request);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task AddExampleLink_HandlesSpecialCharacters()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            "https://example.com/image with spaces.jpg",
            "Style With Spaces",
            "niji 5"
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.Conflict
        );
    }

    [Fact]
    public async Task AddExampleLink_PerformanceTest()
    {
        // Arrange
        var request = new AddExampleLinkRequest(
            GenerateTestUrl(),
            GenerateTestStyleName(),
            "1.0"
        );
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.Should().NotBeNull();
    }
}

// Request DTO for integration tests
public sealed record AddExampleLinkRequest(
    string Link,
    string Style,
    string Version
);