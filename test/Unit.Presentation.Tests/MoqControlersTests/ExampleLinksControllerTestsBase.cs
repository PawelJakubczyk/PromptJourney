using MediatR;
using Moq;
using Presentation.Controllers;

namespace Unit.Presentation.Tests.MoqControlersTests;

public class ExampleLinksControllerTestsBase : ControllerTestsBase
{
    protected static ExampleLinksController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new ExampleLinksController(sender);
    }
}