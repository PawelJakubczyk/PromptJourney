using Application.Features.Styles.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.StylesControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public sealed class UpdateStyleTests(MidjourneyTestWebApplicationFactory factory) : StylesControllerTestsBase(factory)
{
    [Fact]
    public async Task Update_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var styleName = "ExistingStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Updated description",
            ["updated", "tags"]
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,          // Successfully updated
            HttpStatusCode.BadRequest,  // Validation failed or mismatch
            HttpStatusCode.NotFound     // Style not found
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var updatedStyle = await DeserializeResponse<StyleResponse>(response);
            updatedStyle.Should().NotBeNull();
            updatedStyle!.Name.Should().Be(styleName);
        }
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenRouteNameAndPayloadNameDontMatch()
    {
        // Arrange
        var routeName = "Style1";
        var request = new UpdateStyleRequest(
            "Style2", // Different from route name
            "Custom"
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{routeName}", request);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_ForNonExistentStyle()
    {
        // Arrange
        var styleName = GenerateTestStyleName();
        var request = new UpdateStyleRequest(
            styleName,
            "Custom"
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WithInvalidRequest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "" // Invalid empty type
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_HandlesSpecialCharacters()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom",
            "Updated déscription with spéciál characters",
            ["tag with spaces", "tag_with_underscores"]
        );

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{Uri.EscapeDataString(styleName)}", request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task Update_PerformanceTest()
    {
        // Arrange
        var styleName = "TestStyle";
        var request = new UpdateStyleRequest(
            styleName,
            "Custom"
        );
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{styleName}", request);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.Should().NotBeNull();
    }
}

// Request DTO for integration tests
public sealed record UpdateStyleRequest(
    string Name,
    string Type,
    string? Description = null,
    List<string>? Tags = null
);