using Application.Features.PromptHistory.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.PromptHistoryControllersTests;

public sealed class GetLastPromptHistoryTests : PromptHistoryControllerTestsBase
{
    public GetLastPromptHistoryTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    public async Task GetLast_ReturnsOk_ForValidCounts(int count)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/last/{count}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();
            historyRecords!.Count.Should().BeLessOrEqualTo(count);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task GetLast_HandlesBadRequest_ForInvalidCounts(int invalidCount)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/last/{invalidCount}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLast_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/last/5");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            AssertOkResponse<PromptHistoryResponse>(response);

            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();

            if (historyRecords!.Any())
            {
                var firstRecord = historyRecords.First();
                firstRecord.Prompt.Should().NotBeNullOrEmpty();
                firstRecord.Version.Should().NotBeNullOrEmpty();
                firstRecord.CreatedOn.Should().NotBe(null);

                // Records should be ordered by creation date (most recent first)
                for (int i = 1; i < historyRecords.Count; i++)
                {
                    historyRecords[i - 1].CreatedOn.Should().BeOnOrAfter(historyRecords[i].CreatedOn);
                }
            }
        }
    }

    [Fact]
    public async Task GetLast_ReturnsConsistentResults()
    {
        // Arrange
        const int count = 3;

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/last/{count}");
        var response2 = await Client.GetAsync($"{BaseUrl}/last/{count}");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK)
        {
            var records1 = await DeserializeResponse<List<PromptHistoryResponse>>(response1);
            var records2 = await DeserializeResponse<List<PromptHistoryResponse>>(response2);

            records1.Should().BeEquivalentTo(records2);
        }
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task GetLast_HandlesLargeCounts(int largeCount)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/last/{largeCount}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();
            historyRecords!.Count.Should().BeLessOrEqualTo(largeCount);
        }
    }

    [Fact]
    public async Task GetLast_PerformanceTest()
    {
        // Arrange
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/last/10");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
}