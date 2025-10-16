using MediatR;
using Moq;
using Presentation.Controllers;

namespace Unit.Presentation.Tests.MoqControlersTests.PropertiesMoqControlersTests.Base;

public class PropertiesControllerTestsBase : ControllerTestsBase
{
    protected static PropertiesController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new PropertiesController(sender);
    }
}