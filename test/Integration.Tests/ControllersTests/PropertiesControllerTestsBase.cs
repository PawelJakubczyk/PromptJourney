using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.PropertiesControllersTests;

public class PropertiesControllerTestsBase : ControllerTestsBase
{
    protected const string BaseUrl = "/api/properties";

    public PropertiesControllerTestsBase(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    // Helper method to generate unique property names
    protected static string GenerateTestPropertyName() => $"TestProperty_{Guid.NewGuid().ToString("N")[..8]}";

    // Helper method to generate test parameters
    protected static List<string> GenerateTestParameters() => [$"--{GenerateTestPropertyName().ToLower()}", $"--{GenerateTestPropertyName().ToLower()[..3]}"];
}