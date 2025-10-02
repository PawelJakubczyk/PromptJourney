using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Integration.Tests.ControllersTests.StylesControllersTests;

public class StylesControllerTestsBase : ControllerTestsBase
{
    protected const string BaseUrl = "/api/styles";

    public StylesControllerTestsBase(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    // Helper method to generate unique style names
    protected static string GenerateTestStyleName() => $"TestStyle_{Guid.NewGuid().ToString("N")[..8]}";

    // Helper method to generate unique tag names
    protected static string GenerateTestTag() => $"tag_{Guid.NewGuid().ToString("N")[..6]}";
}