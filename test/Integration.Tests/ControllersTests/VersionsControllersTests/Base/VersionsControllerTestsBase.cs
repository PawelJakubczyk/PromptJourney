using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.VersionsControllersTests.Base;

public class VersionsControllerTestsBase(MidjourneyTestWebApplicationFactory factory) : ControllerTestsBase(factory)
{
    protected const string BaseUrl = "/api/versions";
}