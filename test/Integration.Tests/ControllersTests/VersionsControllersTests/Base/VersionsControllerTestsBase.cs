using Microsoft.AspNetCore.Mvc.Testing;

namespace Integration.Tests.ControllersTests.VersionsControllersTests.Base;

public class VersionsControllerTestsBase : ControllerTestsBase
{
    protected const string BaseUrl = "/api/versions";

    public VersionsControllerTestsBase(MidjourneyTestWebApplicationFactory factory) : base(factory)
    {
    }
}