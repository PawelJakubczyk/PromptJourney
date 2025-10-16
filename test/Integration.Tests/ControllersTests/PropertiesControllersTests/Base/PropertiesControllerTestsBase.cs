namespace Integration.Tests.ControllersTests.PropertiesControllersTests.Base;

public class PropertiesControllerTestsBase(MidjourneyTestWebApplicationFactory factory) : ControllerTestsBase(factory)
{
    protected const string BaseUrl = "/api/properties";

    // Helper method to generate unique property names
    protected static string GenerateTestPropertyName() => $"TestProperty_{Guid.NewGuid().ToString("N")[..8]}";

    // Helper method to generate test parameters
    protected static List<string> GenerateTestParameters() => [$"--{GenerateTestPropertyName().ToLower()}", $"--{GenerateTestPropertyName().ToLower()[..3]}"];
}