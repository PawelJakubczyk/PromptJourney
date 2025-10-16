using MediatR;
using Moq;
using Presentation.Controllers;

namespace Unit.Presentation.Tests.MoqControlersTests;

public class PromptHistoryControllerTestsBase : ControllerTestsBase
{
    protected static PromptHistoriesController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new PromptHistoriesController(sender);
    }
}