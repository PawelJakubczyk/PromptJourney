using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.VersionsControllersTests;

public class VersionsControllerTestsBase : ControllerTestsBase
{
    protected const string BaseUrl = "/api/versions";

    public VersionsControllerTestsBase(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
}