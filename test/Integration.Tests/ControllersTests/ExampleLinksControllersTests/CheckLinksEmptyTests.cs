using FluentAssertions;
using Integration.Tests.ControllersTests.ExampleLinksControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public sealed class CheckLinksEmptyTests : ExampleLinksControllerTestsBase
{
    public CheckLinksEmptyTests(MidjourneyTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsOk_WithValidResponse()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/noempty");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsJsonWithIsEmptyProperty()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/noempty");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("isEmpty");
        content.Should().MatchRegex(@"\{.*""isEmpty"":(true|false).*\}");
    }

    [Fact]
    public async Task CheckLinksEmpty_ReturnsConsistentResults()
    {
        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/noempty");
        var response2 = await Client.GetAsync($"{BaseUrl}/noempty");

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();

        content1.Should().Be(content2);
    }

    [Fact]
    public async Task CheckLinksEmpty_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/noempty");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckLinksEmpty_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/noempty");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseObject = await DeserializeResponse<IsEmptyResponse>(response);
        responseObject.Should().NotBeNull();
        //responseObject!.IsEmpty.Should().BeOneOf(true, false);
    }

    [Fact]
    public async Task CheckLinksEmpty_HandlesMultipleConcurrentRequests()
    {
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Client.GetAsync($"{BaseUrl}/noempty"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        foreach (var response in responses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("isEmpty");
        }
    }

    // Helper class for response deserialization
    public class IsEmptyResponse
    {
        public bool IsEmpty { get; set; }
    }
}