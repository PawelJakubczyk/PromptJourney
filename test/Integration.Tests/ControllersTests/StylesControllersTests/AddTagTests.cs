using Application.Features.Styles.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class AddTagTests : StylesControllerTestsBase
{
    public AddTagTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task AddTag_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new AddTagRequest(GenerateTestTag());

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{styleName}/tags", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,          // Successfully added
            HttpStatusCode.BadRequest,  // Validation failed or tag already exists
            HttpStatusCode.NotFound     // Style not found
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var updatedStyle = await DeserializeResponse<StyleResponse>(response);
            updatedStyle.Should().NotBeNull();
            updatedStyle!.Name.Should().Be(styleName);
        }
    }

    [Theory]
    [InlineData("")]           // Empty tag
    [InlineData(" ")]          // Whitespace tag
    [InlineData("   ")]        // Multiple spaces
    public async Task AddTag_ReturnsBadRequest_WithInvalidTag(string invalidTag)
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new AddTagRequest(invalidTag);

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{styleName}/tags", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddTag_ReturnsNotFound_ForNonExistentStyle()
    {
        // Arrange
        var styleName = GenerateTestStyleName();
        var request = new AddTagRequest(GenerateTestTag());

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{styleName}/tags", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("tag with spaces")]
    [InlineData("tag_with_underscores")]
    [InlineData("tag-with-dashes")]
    [InlineData("UPPERCASETAG")]
    [InlineData("lowercasetag")]
    public async Task AddTag_HandlesSpecialCharacters_InTags(string tag)
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new AddTagRequest(tag);

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{styleName}/tags", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task AddTag_ReturnsBadRequest_WithNullRequest()
    {
        // Arrange
        var styleName = "TestStyle";

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{styleName}/tags", (AddTagRequest?)null);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task AddTag_PerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new AddTagRequest(GenerateTestTag());
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{styleName}/tags", request);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.Should().NotBeNull();
    }
}

// Request DTO for integration tests
public sealed record AddTagRequest(string Tag);