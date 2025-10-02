using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace Integration.Tests.ControllersTests.PromptHistoryControllersTests;

public sealed class GetRecordCountPromptHistoryTests : PromptHistoryControllerTestsBase
{
    public GetRecordCountPromptHistoryTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetRecordCount_ReturnsOk_WithValidResponse()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetRecordCount_ReturnsJsonWithCountProperty()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("count");
        content.Should().MatchRegex(@"\{.*""count"":\d+.*\}");
    }

    [Fact]
    public async Task GetRecordCount_ReturnsNonNegativeCount()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var count = await GetCountFromResponse(response);
        count.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsConsistentResults()
    {
        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/count");
        var response2 = await Client.GetAsync($"{BaseUrl}/count");

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var count1 = await GetCountFromResponse(response1);
        var count2 = await GetCountFromResponse(response2);

        // Counts should be the same or close (allowing for concurrent operations)
        Math.Abs(count1 - count2).Should().BeLessOrEqualTo(10);
    }

    [Fact]
    public async Task GetRecordCount_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseObject = await DeserializeResponse<CountResponse>(response);
        responseObject.Should().NotBeNull();
        responseObject!.Count.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetRecordCount_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/count");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRecordCount_HandlesMultipleConcurrentRequests()
    {
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Client.GetAsync($"{BaseUrl}/count"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        foreach (var response in responses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var count = await GetCountFromResponse(response);
            count.Should().BeGreaterOrEqualTo(0);
        }

        // All counts should be similar (allowing for concurrent operations)
        var counts = new List<int>();
        foreach (var response in responses)
        {
            counts.Add(await GetCountFromResponse(response));
        }

        var minCount = counts.Min();
        var maxCount = counts.Max();
        (maxCount - minCount).Should().BeLessOrEqualTo(10);
    }

    [Fact]
    public async Task GetRecordCount_ReturnsValidJsonFormat()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        // Should be valid JSON with count property
        var json = System.Text.Json.JsonDocument.Parse(content);
        json.RootElement.TryGetProperty("count", out var countProperty).Should().BeTrue();
        countProperty.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Number);
    }

    // Helper class for response deserialization
    public class CountResponse
    {
        public int Count { get; set; }
    }
}