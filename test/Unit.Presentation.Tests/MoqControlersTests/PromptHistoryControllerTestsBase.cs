using MediatR;
using Moq;
using Presentation.Controllers;

namespace Unit.Presentation.Tests.MoqControlersTests;

public class PromptHistoryControllerTestsBase : ControllerTestsBase
{
    protected static PromptHistoryController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new PromptHistoryController(sender);
    }
}