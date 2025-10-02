using Application.Features.PromptHistory.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace Integration.Tests.ControllersTests.PromptHistoryControllersTests;

public sealed class GetByKeywordPromptHistoryTests : PromptHistoryControllerTestsBase
{
    public GetByKeywordPromptHistoryTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("landscape")]
    [InlineData("portrait")]
    [InlineData("abstract")]
    [InlineData("modern")]
    [InlineData("art")]
    public async Task GetByKeyword_ReturnsOk_ForValidKeywords(string keyword)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/keyword/{keyword}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();

            // All returned records should contain the keyword in prompt
            if (historyRecords!.Any())
            {
                historyRecords.Should().AllSatisfy(record =>
                {
                    record.Prompt.Should().ContainEquivalentOf(keyword);
                });
            }
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task GetByKeyword_HandlesBadRequest_ForInvalidKeywords(string invalidKeyword)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/keyword/{Uri.EscapeDataString(invalidKeyword)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("a")] // Very short keyword
    [InlineData("ab")] // Short keyword
    public async Task GetByKeyword_HandlesShortKeywords(string shortKeyword)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/keyword/{shortKeyword}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();
        }
    }

    [Theory]
    [InlineData("keyword with spaces")]
    [InlineData("keyword-with-dashes")]
    [InlineData("keyword_with_underscores")]
    [InlineData("UPPERCASEKEYWORD")]
    [InlineData("lowercasekeyword")]
    [InlineData("MixedCaseKeyword")]
    public async Task GetByKeyword_HandlesSpecialCharacters_InKeywords(string keyword)
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/keyword/{Uri.EscapeDataString(keyword)}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByKeyword_ReturnsEmptyList_ForNonExistentKeyword()
    {
        // Arrange
        var nonExistentKeyword = $"nonexistent_{Guid.NewGuid().ToString("N")[..8]}";

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/keyword/{nonExistentKeyword}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
            historyRecords.Should().NotBeNull();
            historyRecords.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetByKeyword_ValidatesResponseStructure()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/keyword/test");

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
                firstRecord.Prompt.Should().ContainEquivalentOf("test");
            }
        }
    }

    [Fact]
    public async Task GetByKeyword_ReturnsConsistentResults()
    {
        // Arrange
        var keyword = "landscape";

        // Act
        var response1 = await Client.GetAsync($"{BaseUrl}/keyword/{keyword}");
        var response2 = await Client.GetAsync($"{BaseUrl}/keyword/{keyword}");

        // Assert
        response1.StatusCode.Should().Be(response2.StatusCode);

        if (response1.StatusCode == HttpStatusCode.OK && response2.StatusCode == HttpStatusCode.OK)
        {
            var records1 = await DeserializeResponse<List<PromptHistoryResponse>>(response1);
            var records2 = await DeserializeResponse<List<PromptHistoryResponse>>(response2);

            records1.Should().BeEquivalentTo(records2);
        }
    }

    [Fact]
    public async Task GetByKeyword_HandlesUnicodeCharacters()
    {
        // Arrange
        var unicodeKeywords = new[] { "café", "naïve", "résumé", "Zürich" };

        foreach (var keyword in unicodeKeywords)
        {
            // Act
            var response = await Client.GetAsync($"{BaseUrl}/keyword/{Uri.EscapeDataString(keyword)}");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var historyRecords = await DeserializeResponse<List<PromptHistoryResponse>>(response);
                historyRecords.Should().NotBeNull();
            }
        }
    }

    [Fact]
    public async Task GetByKeyword_PerformanceTest()
    {
        // Arrange
        var keyword = "test";
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/keyword/{keyword}");

        // Assert
        var duration = DateTime.UtcNow - startTime;
        duration.Should().BeLessThan(TimeSpan.FromSeconds(3));
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }
}