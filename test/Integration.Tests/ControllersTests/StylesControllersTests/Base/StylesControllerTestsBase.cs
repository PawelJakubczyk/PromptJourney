using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.StylesControllersTests.Base;

public class StylesControllerTestsBase(MidjourneyTestWebApplicationFactory factory) : ControllerTestsBase(factory)
{
    protected const string BaseUrl = "/api/styles";

    // Helper method to generate unique style names
    protected static string GenerateTestStyleName() => $"TestStyle_{Guid.NewGuid().ToString("N")[..8]}";

    // Helper method to generate unique tag names
    protected static string GenerateTestTag() => $"tag_{Guid.NewGuid().ToString("N")[..6]}";
}