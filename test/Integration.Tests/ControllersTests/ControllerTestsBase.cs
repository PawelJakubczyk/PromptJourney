using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Text.Json;

namespace Integration.Tests.ControllersTests;

public abstract class ControllerTestsBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    protected ControllerTestsBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    // Helper methods for HTTP assertions
    protected static void AssertOkResponse<T>(HttpResponseMessage response, int expectedCount = -1)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        if (expectedCount >= 0)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            var items = JsonSerializer.Deserialize<List<T>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            items.Should().HaveCount(expectedCount);
        }
    }

    protected static void AssertCreatedResponse<T>(HttpResponseMessage response)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        response.Headers.Location.Should().NotBeNull();
    }

    protected static void AssertNoContentResponse(HttpResponseMessage response)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    protected static void AssertBadRequestResponse(HttpResponseMessage response)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    protected static void AssertNotFoundResponse(HttpResponseMessage response)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    protected static void AssertInternalServerErrorResponse(HttpResponseMessage response)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    protected static void AssertErrorResponse(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(expectedStatusCode);
    }

    // Helper method to deserialize response content
    protected static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    // Helper method to check if object exists in response
    protected static async Task<bool> GetExistsFromResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ExistsResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result?.Exists ?? false;
    }

    // Helper method to get count from response
    protected static async Task<int> GetCountFromResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CountResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return result?.Count ?? 0;
    }

    // Response models for deserialization
    public class ExistsResponse
    {
        public bool Exists { get; set; }
    }

    public class CountResponse
    {
        public int Count { get; set; }
    }

    public class IsEmptyResponse
    {
        public bool IsEmpty { get; set; }
    }
}