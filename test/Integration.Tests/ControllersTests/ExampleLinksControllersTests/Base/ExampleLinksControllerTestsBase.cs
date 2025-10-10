using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.ExampleLinksControllersTests.Base;

public class ExampleLinksControllerTestsBase : ControllerTestsBase
{
    protected const string BaseUrl = "/api/examplelinks";

    public ExampleLinksControllerTestsBase(MidjourneyTestWebApplicationFactory factory) : base(factory)
    {
    }

    // Helper method to generate unique test URLs
    protected static string GenerateTestUrl() => $"https://test.example.com/{Guid.NewGuid()}.jpg";

    // Helper method to generate unique style names
    protected static string GenerateTestStyleName() => $"TestStyle_{Guid.NewGuid().ToString("N")[..8]}";
}