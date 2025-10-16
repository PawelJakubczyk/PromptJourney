using MediatR;
using Moq;
using Presentation.Controllers;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;

public class VersionsControllerTestsBase : ControllerTestsBase
{
    protected static VersionsController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new VersionsController(sender);
    }
}