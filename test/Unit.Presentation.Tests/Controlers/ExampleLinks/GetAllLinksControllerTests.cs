using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Features.ExampleLinks.Responses;
using Utilities.Constants;
using Utilities.Errors;

namespace Integration.Tests.Controlers.ExampleLinks;

public class GetAllLinksControllerTests: ExampleLinksControllerTestsBase
{

    [Fact]
    public async Task GetAll_ReturnsOk_WhenHandlerSucceeds()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://a", "styleA", "v1"),
            new("http://b", "styleB", "v2")
        };

        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        var value = Assert.IsAssignableFrom<IEnumerable<ExampleLinkResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetAll_ReturnsErrorResponse_WhenHandlerFails_WithSingleLayerError()
    {
        // Arrange
        var error = new Error<PersistenceLayer>("Database failure", StatusCodes.Status500InternalServerError);
        var result = Result.Fail(new List<Error> { error });
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);

        // body shape: { MainError = { code, message }, details = [...] }
        var body = objResult.Value;
        Assert.NotNull(body);

        // basic reflection checks to ensure main error and details exist
        var bodyProps = body!.GetType().GetProperty("MainError");
        Assert.NotNull(bodyProps);
        var main = bodyProps!.GetValue(body);
        Assert.NotNull(main);
        var mainCodeProp = main!.GetType().GetProperty("code");
        Assert.NotNull(mainCodeProp);
        Assert.Equal(StatusCodes.Status500InternalServerError, (int)mainCodeProp!.GetValue(main)!);

        var detailsProp = body.GetType().GetProperty("details");
        Assert.NotNull(detailsProp);
        var details = detailsProp!.GetValue(body) as System.Collections.IEnumerable;
        Assert.NotNull(details);
        // ensure at least one detail present
        var enumerator = details!.GetEnumerator();
        Assert.True(enumerator.MoveNext());
    }

    [Fact]
    public async Task GetAll_ReturnsHighestPriorityStatus_WhenMultipleLayerErrorsPresent()
    {
        // Arrange
        var err500 = new Error<PersistenceLayer>("DB crash", StatusCodes.Status500InternalServerError);
        var err400 = new Error<ApplicationLayer>("Bad input", StatusCodes.Status400BadRequest);
        var err409 = new Error<DomainLayer>("Conflict", StatusCodes.Status409Conflict);

        // Order of insertion shouldn't matter — controller should pick highest priority (500 over 409/400)
        var result = Result.Fail(new List<Error> { err400, err409, err500 });

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);

        // Ensure main code equals 500 in body
        var body = objResult.Value!;
        var main = body.GetType().GetProperty("MainError")!.GetValue(body);
        var mainCode = (int)main!.GetType().GetProperty("code")!.GetValue(main)!;
        Assert.Equal(StatusCodes.Status500InternalServerError, mainCode);
    }

    [Fact]
    public async Task GetAll_IncludesAllErrorsInResponseBody_WhenFailed()
    {
        // Arrange
        var e1 = new Error<PersistenceLayer>("DB crash", StatusCodes.Status500InternalServerError);
        var e2 = new Error<ApplicationLayer>("Invalid", StatusCodes.Status400BadRequest);

        var result = Result.Fail(new List<Error> { e1, e2 });

        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(actionResult);
        var body = objResult.Value!;
        var details = body.GetType().GetProperty("details")!.GetValue(body) as System.Collections.IEnumerable;
        Assert.NotNull(details);

        // collect messages
        var messages = new List<string>();
        foreach (var d in details!)
        {
            var msgProp = d.GetType().GetProperty("message");
            if (msgProp != null)
            {
                var msg = msgProp.GetValue(d) as string;
                if (msg != null) messages.Add(msg);
            }
        }

        Assert.Contains("DB crash", messages);
        Assert.Contains("Invalid", messages);
        Assert.Equal(2, messages.Count);
    }
}
