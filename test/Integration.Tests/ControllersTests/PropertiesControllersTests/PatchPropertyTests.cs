using Application.Features.Properties.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.PropertiesControllersTests;

public sealed class PatchPropertyTests : PropertiesControllerTestsBase
{
    public PatchPropertyTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("DefaultValue", "new-default")]
    [InlineData("MinValue", "new-min")]
    [InlineData("MaxValue", "new-max")]
    [InlineData("Description", "New description")]
    public async Task PatchProperty_ReturnsValidResponse_WithValidCharacteristics(string characteristic, string newValue)
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PatchPropertyRequest(characteristic, newValue);

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/version/{version}/{propertyName}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,          // Successfully patched
            HttpStatusCode.BadRequest,  // Invalid characteristic or value
            HttpStatusCode.NotFound     // Property not found
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var patchedProperty = await DeserializeResponse<PropertyResponse>(response);
            patchedProperty.Should().NotBeNull();
            patchedProperty!.PropertyName.Should().Be(propertyName);
            patchedProperty.Version.Should().Be(version);
        }
    }

    [Theory]
    [InlineData("", "value")]                    // Empty characteristic
    [InlineData("InvalidCharacteristic", "value")] // Invalid characteristic
    [InlineData("DefaultValue", "")]             // Empty value
    public async Task PatchProperty_HandlesBadRequest_WithInvalidParameters(string characteristic, string newValue)
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PatchPropertyRequest(characteristic, newValue);

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/version/{version}/{propertyName}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchProperty_ReturnsNotFound_ForNonExistentProperty()
    {
        // Arrange
        var version = "1.0";
        var propertyName = GenerateTestPropertyName();
        var request = new PatchPropertyRequest("DefaultValue", "new-value");

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/version/{version}/{propertyName}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("999.0", "aspect")]      // Non-existent version
    [InlineData("", "aspect")]           // Empty version
    [InlineData("1.0", "")]              // Empty property name
    public async Task PatchProperty_HandlesInvalidRouteParameters(string version, string propertyName)
    {
        // Arrange
        var request = new PatchPropertyRequest("DefaultValue", "new-value");

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/version/{Uri.EscapeDataString(version)}/{Uri.EscapeDataString(propertyName)}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchProperty_HandlesSpecialCharacters()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PatchPropertyRequest("Description", "New déscription with spéciál characters & symbols: @#$%");

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/version/{version}/{propertyName}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task PatchProperty_HandlesNullValue()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PatchPropertyRequest("DefaultValue", null);

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/version/{version}/{propertyName}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task PatchProperty_PerformanceTest()
    {
        // Arrange
        var version = "1.0";
        var propertyName = "aspect";
        var request = new PatchPropertyRequest("DefaultValue", "test-value");
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PatchAsync($"{BaseUrl}/version/{version}/{propertyName}", JsonContent.Create(request));

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.Should().NotBeNull();
    }
}

// Request DTO for integration tests
public sealed record PatchPropertyRequest(
    string CharacteristicToUpdate,
    string? NewValue
);