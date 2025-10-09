using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.PromptHistoryControllersTests.Base;

public class PromptHistoryControllerTestsBase : ControllerTestsBase
{
    protected const string BaseUrl = "/api/prompthistory";

    public PromptHistoryControllerTestsBase(MidjourneyTestWebApplicationFactory factory) : base(factory)
    {
    }

    // Helper method to generate unique prompt text
    protected static string GenerateTestPrompt() => $"Test prompt {Guid.NewGuid().ToString("N")[..8]} with beautiful landscape";

    // Helper method to format date for query parameters
    protected static string FormatDateForQuery(DateTime date) => date.ToString("yyyy-MM-dd");
}