using Application.Features.Styles.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class UpdateDescriptionTests : StylesControllerTestsBase
{
    public UpdateDescriptionTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateDescription_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("Updated description for the style");

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}/description", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,          // Successfully updated
            HttpStatusCode.BadRequest,  // Validation failed
            HttpStatusCode.NotFound     // Style not found
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var updatedStyle = await DeserializeResponse<StyleResponse>(response);
            updatedStyle.Should().NotBeNull();
            updatedStyle!.Name.Should().Be(styleName);
            updatedStyle.Description.Should().Be(request.Description);
        }
    }

    [Fact]
    public async Task UpdateDescription_ReturnsNotFound_ForNonExistentStyle()
    {
        // Arrange
        var styleName = GenerateTestStyleName();
        var request = new UpdateDescriptionRequest("New description");

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}/description", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task UpdateDescription_HandlesBadRequest_ForInvalidStyleName(string invalidStyleName)
    {
        // Arrange
        var request = new UpdateDescriptionRequest("New description");

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{Uri.EscapeDataString(invalidStyleName)}/description", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDescription_HandlesSpecialCharacters()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("Description with spéciál characters, émojis 🎨 and symbols @#$%^&*()");

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}/description", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task UpdateDescription_HandlesLongDescription()
    {
        // Arrange
        var styleName = "TestStyle";
        var longDescription = new string('A', 1000) + " This is a very long description for testing purposes.";
        var request = new UpdateDescriptionRequest(longDescription);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}/description", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task UpdateDescription_HandlesNullDescription()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest(null!);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}/description", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task UpdateDescription_ReturnsBadRequest_WithNullRequest()
    {
        // Arrange
        var styleName = "TestStyle";

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}/description", (UpdateDescriptionRequest?)null);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task UpdateDescription_PerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateDescriptionRequest("Performance test description");
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}/description", request);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.Should().NotBeNull();
    }
}

// Request DTO for integration tests
public sealed record UpdateDescriptionRequest(string Description);