using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.ExampleLinksControllersTests.Base;

public class ExampleLinksControllerTestsBase(MidjourneyTestWebApplicationFactory factory) : ControllerTestsBase(factory)
{
    protected const string BaseUrl = "/api/examplelinks";

    // Helper method to generate unique test URLs
    protected static string GenerateTestUrl() => $"https://test.example.com/{Guid.NewGuid()}.jpg";

    // Helper method to generate unique style names
    protected static string GenerateTestStyleName() => $"TestStyle_{Guid.NewGuid().ToString("N")[..8]}";
}