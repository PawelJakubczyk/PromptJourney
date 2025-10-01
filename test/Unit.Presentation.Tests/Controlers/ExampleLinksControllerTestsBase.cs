using MediatR;
using Moq;
using Presentation.Controllers;

namespace Integration.Tests.Controlers.ExampleLinks;

public class ExampleLinksControllerTestsBase
{
    protected static ExampleLinksController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new ExampleLinksController(sender);
    }
}

