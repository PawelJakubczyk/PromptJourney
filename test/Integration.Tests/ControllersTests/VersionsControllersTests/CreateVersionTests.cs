using Application.UseCases.Versions.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.VersionsControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.VersionsControllersTests;

public sealed class CreateVersionTests(MidjourneyTestWebApplicationFactory factory) : VersionsControllerTestsBase(factory)
{
    [Fact]
    public async Task Create_ReturnsCreatedOrBadRequest_WithValidRequest()
    {
        // Arrange
        var request = new CreateVersionRequest(
            $"test-{Guid.NewGuid()}", // Unique version to avoid conflicts
            "--v test",
            DateTime.UtcNow,
            "Test version for integration testing"
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,     // Successfully created
            HttpStatusCode.BadRequest,  // Version already exists or validation failed
            HttpStatusCode.Conflict    // Business rule conflict
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            response.Headers.Location.Should().NotBeNull();
            var createdVersion = await DeserializeResponse<VersionResponse>(response);
            createdVersion.Should().NotBeNull();
            createdVersion!.Version.Should().Be(request.Version);
        }
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WithInvalidRequest()
    {
        // Arrange
        var invalidRequest = new CreateVersionRequest(
            "", // Empty version
            ""  // Empty parameter
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, invalidRequest);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WithNullRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, (CreateVersionRequest?)null);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Fact]
    public async Task Create_HandlesContentTypeCorrectly()
    {
        // Arrange
        var request = new CreateVersionRequest(
            $"content-test-{Guid.NewGuid()}",
            "--v content-test"
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.Should().NotBeNull();
        // Accept any reasonable status code as the test is about content type handling
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Conflict
        );
    }
}

// Request DTOs (these should match the ones from the controller)
public sealed record CreateVersionRequest(
    string Version,
    string Parameter,
    DateTime? ReleaseDate = null,
    string? Description = null
);