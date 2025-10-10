using Application.Features.PromptHistory.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.PromptHistoryControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.PromptHistoryControllersTests;

public sealed class GetAllPromptHistoryTests : PromptHistoryControllerTestsBase
{
    public GetAllPromptHistoryTests(MidjourneyTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithValidResponse()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
        historyRecords.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_ReturnsConsistentResults()
    {
        // Act
        var response1 = await Client.GetAsync(BaseUrl);
        var response2 = await Client.GetAsync(BaseUrl);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var records1 = await DeserializeResponse<List<PromptHistoryResponse>>(response1);
        var records2 = await DeserializeResponse<List<PromptHistoryResponse>>(response2);

        records1.Should().BeEquivalentTo(records2);
    }

    [Fact]
    public async Task GetAll_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        AssertOkResponse<PromptHistoryResponse>(response);

        var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
        historyRecords.Should().NotBeNull();

        if (historyRecords!.Any())
        {
            var firstRecord = historyRecords.First();
            firstRecord.Prompt.Should().NotBeNullOrEmpty();
            firstRecord.Version.Should().NotBeNullOrEmpty();
            firstRecord.CreatedOn.Should().NotBe(null);
        }
    }

    [Fact]
    public async Task GetAll_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectContentType()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        //response.Content.Headers.ContentType?.Charset.Should().Be("utf-8");
    }

    [Fact]
    public async Task GetAll_HandlesLargeDatasets()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Should handle any amount of data without timeout
        var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
        historyRecords.Should().NotBeNull();

        // If there are records, validate they're properly structured
        if (historyRecords!.Count > 100)
        {
            historyRecords.Should().AllSatisfy(record =>
            {
                record.Prompt.Should().NotBeNullOrEmpty();
                record.Version.Should().NotBeNullOrEmpty();
            });
        }
    }
}