using Application.UseCases.ExampleLinks.Responses;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinks;

public sealed class GetAllTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOk_WhenHandlerSucceeds()
    {
        // Arrange
        var list = new List<ExampleLinkResponse>
        {
            new("http://example1.com", "Style1", "1.0"),
            new("http://example2.com", "Style2", "2.0")
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
        AssertOkResult<ExampleLinkResponse>(actionResult, 2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenNoLinksExist()
    {
        // Arrange
        var emptyList = new List<ExampleLinkResponse>();
        var result = Result.Ok(emptyList);
        var senderMock = new Mock<ISender>();
        senderMock
            .Setup(s => s.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetAll(CancellationToken.None);

        // Assert
        AssertOkResult<ExampleLinkResponse>(actionResult, 0);
    }
}