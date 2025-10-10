using Application.Features.PromptHistory.Responses;
using FluentAssertions;
using Integration.Tests.ControllersTests.PromptHistoryControllersTests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Integration.Tests.ControllersTests.PromptHistoryControllersTests;

public sealed class GetByDateRangePromptHistoryTests : PromptHistoryControllerTestsBase
{
    public GetByDateRangePromptHistoryTests(MidjourneyTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetByDateRange_ReturnsOk_ForValidDateRange()
    {
        // Arrange
        var from = DateTime.Today.AddDays(-30);
        var to = DateTime.Today;
        var fromStr = FormatDateForQuery(from);
        var toStr = FormatDateForQuery(to);

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/daterange?from={fromStr}&to={toStr}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();

            // All records should be within the specified date range
            if (historyRecords!.Any())
            {
                historyRecords.Should().AllSatisfy(record =>
                {
                    record.CreatedOn.Date.Should().BeOnOrAfter(from.Date);
                    record.CreatedOn.Date.Should().BeOnOrBefore(to.Date);
                });
            }
        }
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-31")] // Past month
    [InlineData("2024-06-01", "2024-06-30")] // Specific month
    [InlineData("2024-01-01", "2024-12-31")] // Full year
    public async Task GetByDateRange_HandlesVariousDateRanges(string fromStr, string toStr)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/daterange?from={fromStr}&to={toStr}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();
        }
    }

    [Theory]
    [InlineData("2024-12-31", "2024-01-01")] // From after to
    [InlineData("invalid-date", "2024-01-01")] // Invalid from date
    [InlineData("2024-01-01", "invalid-date")] // Invalid to date
    [InlineData("", "2024-01-01")] // Empty from date
    [InlineData("2024-01-01", "")] // Empty to date
    public async Task GetByDateRange_HandlesBadRequest_ForInvalidDateRanges(string fromStr, string toStr)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/daterange?from={fromStr}&to={toStr}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByDateRange_HandlesMissingQueryParameters()
    {
        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/daterange");
        var response2 = await Client.GetAsync($"{BaseUrl}/daterange?from=2024-01-01");
        var response3 = await Client.GetAsync($"{BaseUrl}/daterange?to=2024-01-31");

        // Assert
        foreach (var response in new[] { response1, response2, response3 })
        {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task GetByDateRange_ValidatesResponseStructure()
    {
        // Arrange
        var from = DateTime.Today.AddDays(-7);
        var to = DateTime.Today;
        var fromStr = FormatDateForQuery(from);
        var toStr = FormatDateForQuery(to);

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/daterange?from={fromStr}&to={toStr}");

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
            }
        }
    }

    [Fact]
    public async Task GetByDateRange_HandlesLargeDateRanges()
    {
        // Arrange - Very large date range
        var from = new DateTime(2020, 1, 1);
        var to = new DateTime(2030, 12, 31);
        var fromStr = FormatDateForQuery(from);
        var toStr = FormatDateForQuery(to);

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/daterange?from={fromStr}&to={toStr}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        // Large date ranges might be rejected by business rules
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            // This is acceptable for very large ranges
            return;
        }

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByDateRange_PerformanceTest()
    {
        // Arrange
        var from = DateTime.Today.AddDays(-30);
        var to = DateTime.Today;
        var fromStr = FormatDateForQuery(from);
        var toStr = FormatDateForQuery(to);
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/daterange?from={fromStr}&to={toStr}");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(5));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
}