using MediatR;
using Moq;
using Presentation.Controllers;

namespace Unit.Presentation.Tests.MoqControlersTests.StylesMoqControlersTests.Base;

public class StylesControllerTestsBase : ControllerTestsBase
{
    protected static StylesController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new StylesController(sender);
    }
}