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

    protected static readonly List<string> supportedVersions = ["1.0", "2.0", "3.0", "4.0", "5.0", "5.1", "5.2", "6.0", "niji 5", "niji 6"];
    protected static readonly List<string> supportedEmpty = [];
}