using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Integration.Tests.ControllersTests.ExampleLinks;

public class ExampleLinksControllerTestsBase : ControllerTestsBase
{
    protected const string BaseUrl = "/api/examplelinks";

    public ExampleLinksControllerTestsBase(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    // Helper method to generate unique test URLs
    protected static string GenerateTestUrl() => $"https://test.example.com/{Guid.NewGuid()}.jpg";

    // Helper method to generate unique style names
    protected static string GenerateTestStyleName() => $"TestStyle_{Guid.NewGuid().ToString("N")[..8]}";
}