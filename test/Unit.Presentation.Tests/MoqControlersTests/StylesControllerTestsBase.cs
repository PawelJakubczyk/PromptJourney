using MediatR;
using Moq;
using Presentation.Controllers;

namespace Unit.Presentation.Tests.MoqControlersTests;

public class StylesControllerTestsBase : ControllerTestsBase
{
    protected static StylesController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new StylesController(sender);
    }
}