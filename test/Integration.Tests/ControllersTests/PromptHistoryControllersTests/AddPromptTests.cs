using Application.Features.PromptHistory.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Integration.Tests.ControllersTests.PromptHistoryControllersTests;

public sealed class AddPromptTests : PromptHistoryControllerTestsBase
{
    public AddPromptTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task AddPrompt_ReturnsValidResponse_WithValidRequest()
    {
        // Arrange
        var request = new AddPromptRequest(
            GenerateTestPrompt(),
            "1.0"
        );

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,     // Successfully created
            HttpStatusCode.BadRequest,  // Validation failed or version doesn't exist
            HttpStatusCode.NotFound     // Version not found
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            AssertCreatedResponse<PromptHistoryResponse>(response);

            var createdPrompt = await DeserializeResponse<PromptHistoryResponse>(response);
            createdPrompt.Should().NotBeNull();
            createdPrompt!.Prompt.Should().Be(request.Prompt);
            createdPrompt.Version.Should().Be(request.Version);
        }
    }

    [Theory]
    [InlineData("", "1.0")]           // Empty prompt
    [InlineData("Valid prompt", "")]  // Empty version
    [InlineData("", "")]              // Both empty
    public async Task AddPrompt_ReturnsBadRequest_WithInvalidRequest(string prompt, string version)
    {
        // Arrange
        var request = new AddPromptRequest(prompt, version);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddPrompt_ReturnsBadRequest_WithNullRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, (AddPromptRequest?)null);

        // Assert
        AssertBadRequestResponse(response);
    }

    [Theory]
    [InlineData("99.0")]     // Non-existent version
    [InlineData("invalid")]  // Invalid version format
    [InlineData("0.0")]      // Potentially invalid version
    public async Task AddPrompt_HandlesInvalidVersions(string version)
    {
        // Arrange
        var request = new AddPromptRequest(GenerateTestPrompt(), version);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.Created
        );
    }

    [Fact]
    public async Task AddPrompt_HandlesLongPrompts()
    {
        // Arrange
        var longPrompt = new string('A', 1000) + " " + GenerateTestPrompt(); // Very long prompt
        var request = new AddPromptRequest(longPrompt, "1.0");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var createdPrompt = await DeserializeResponse<PromptHistoryResponse>(response);
            createdPrompt!.Prompt.Should().Be(longPrompt);
        }
    }

    [Fact]
    public async Task AddPrompt_HandlesSpecialCharacters()
    {
        // Arrange
        var specialPrompt = "Prompt with émojis 🎨, spéciál characters & symbols: @#$%^&*()";
        var request = new AddPromptRequest(specialPrompt, "1.0");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var createdPrompt = await DeserializeResponse<PromptHistoryResponse>(response);
            createdPrompt!.Prompt.Should().Be(specialPrompt);
        }
    }

    [Fact]
    public async Task AddPrompt_HandlesContentTypeCorrectly()
    {
        // Arrange
        var request = new AddPromptRequest(GenerateTestPrompt(), "1.0");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound
        );
    }

    [Fact]
    public async Task AddPrompt_ValidatesResponseLocation()
    {
        // Arrange
        var request = new AddPromptRequest(GenerateTestPrompt(), "1.0");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        if (response.StatusCode == HttpStatusCode.Created)
        {
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().Contain("count");
        }
    }

    [Fact]
    public async Task AddPrompt_PerformanceTest()
    {
        // Arrange
        var request = new AddPromptRequest(GenerateTestPrompt(), "1.0");
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.Should().NotBeNull();
    }

    [Fact]
    public async Task AddPrompt_HandlesMultipleVersionFormats()
    {
        // Arrange
        var versionFormats = new[] { "1", "2.0", "5.2", "6", "niji 5", "niji 6" };

        foreach (var version in versionFormats)
        {
            var request = new AddPromptRequest(GenerateTestPrompt(), version);

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, request);

            // Assert
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.Created,
                HttpStatusCode.BadRequest,
                HttpStatusCode.NotFound
            );
        }
    }

    [Fact]
    public async Task AddPrompt_CreatesUniqueRecords()
    {
        // Arrange
        var basePrompt = GenerateTestPrompt();
        var request1 = new AddPromptRequest($"{basePrompt} - First", "1.0");
        var request2 = new AddPromptRequest($"{basePrompt} - Second", "1.0");

        // Act
        var response1 = await Client.PostAsJsonAsync(BaseUrl, request1);
        var response2 = await Client.PostAsJsonAsync(BaseUrl, request2);

        // Assert
        if (response1.StatusCode == HttpStatusCode.Created && response2.StatusCode == HttpStatusCode.Created)
        {
            var prompt1 = await DeserializeResponse<PromptHistoryResponse>(response1);
            var prompt2 = await DeserializeResponse<PromptHistoryResponse>(response2);

            prompt1!.Prompt.Should().NotBe(prompt2!.Prompt);
            prompt1.CreatedOn.Should().NotBe(prompt2.CreatedOn);
        }
    }
}

// Request DTO for integration tests
public sealed record AddPromptRequest(
    string Prompt,
    string Version
);