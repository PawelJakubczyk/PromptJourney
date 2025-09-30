using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Utilities.Constants;
using Utilities.Errors;

namespace Integration.Tests.Controlers.ExampleLinks;

public class ExampleLinksControllerTestsBase
{
    protected static ExampleLinksController CreateController(Mock<ISender> senderMock)
    {
        var sender = senderMock.Object;
        return new ExampleLinksController(sender);
    }
}

