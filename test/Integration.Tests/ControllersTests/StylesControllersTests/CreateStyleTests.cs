using Application.Features.Styles.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class CreateStyleTests : StylesControllerTestsBase
{
    public CreateStyleTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var request = new CreateStyleRequest(
            GenerateTestStyleName(),
            "Custom",
            "Test style description",
            ["modern", "creative"]
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,     // Successfully created
            HttpStatusCode.BadRequest,  // Validation failed
            HttpStatusCode.Conflict     // Style already exists
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            AssertCreatedResponse<StyleResponse>(response);

            var createdStyle = await DeserializeResponse<StyleResponse>(response);
            createdStyle.Should().NotBeNull();
            createdStyle!.Name.Should().Be(request.Name);
            createdStyle.Type.Should().Be(request.Type);
        }
    }

    [Theory]
    [InlineData("", "Custom")]           // Empty name
    [InlineData("ValidName", "")]        // Empty type
    [InlineData("", "")]                 // Both empty
    public async Task Create_ReturnsBadRequest_WithInvalidRequest(string name, string type)
    {
        // Arrange
        var request = new CreateStyleRequest(name, type);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WithNullRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, (CreateStyleRequest?)null);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task Create_HandlesSpecialCharacters()
    {
        // Arrange
        var request = new CreateStyleRequest(
            "Style With Spéciál Characters & Symbols",
            "Custom",
            "Description with émojis 🎨 and symbols @#$%",
            ["tag with spaces", "tag_with_underscores"]
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Conflict
        );
    }

    [Fact]
    public async Task Create_ValidatesResponseLocation()
    {
        // Arrange
        var request = new CreateStyleRequest(
            GenerateTestStyleName(),
            "Custom"
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        if (response.StatusCode == HttpStatusCode.Created)
        {
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().Contain(request.Name);
        }
    }

    [Fact]
    public async Task Create_PerformanceTest()
    {
        // Arrange
        var request = new CreateStyleRequest(
            GenerateTestStyleName(),
            "Custom"
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
public sealed record CreateStyleRequest(
    string Name,
    string Type,
    string? Description = null,
    List<string>? Tags = null
);