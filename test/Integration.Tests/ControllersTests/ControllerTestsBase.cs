using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace Integration.Tests.ControllersTests;

[Collection("Integration Tests")]
public abstract class ControllerTestsBase {
    protected readonly MidjourneyTestWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected ControllerTestsBase(MidjourneyTestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // Generic helper for callers that expect a typed list
    protected static async Task AssertOkResponse<T>(HttpResponseMessage response, int expectedCount = -1)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        if (expectedCount >= 0)
        {
            var items = await DeserializeResponse<List<T>>(response);
            items.Should().NotBeNull();
            items!.Count.Should().Be(expectedCount);
        }
    }

    // Non-generic helper for callers that don't want to specify a type
    protected static void AssertOkResponse(HttpResponseMessage response, int expectedCount = -1)
    {
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        if (expectedCount >= 0)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrWhiteSpace(content))
            {
                content.Should().NotBeNullOrEmpty();
            }
            else
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    doc.RootElement.GetArrayLength().Should().Be(expectedCount);
                }
                else
                {
                    // If the response is not an array, fail the assertion
                    throw new Exception("Expected JSON array when asserting expectedCount.");
                }
            }
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
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    // Helper method to check if object exists in response
    protected static async Task<bool> GetExistsFromResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ExistsResponse>(content, JsonOptions);
        return result?.Exists ?? false;
    }

    // Helper method to get count from response
    protected static async Task<int> GetCountFromResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CountResponse>(content, JsonOptions);
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
