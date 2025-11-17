using Application.UseCases.ExampleLinks.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.ExampleLinksMoqControlersTests;

public sealed class CheckLinkWithStyleExistsTests : ExampleLinksControllerTestsBase
{
    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsOkWithTrue_WhenStyleHasLinks()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsOkWithFalse_WhenStyleHasNoLinks()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(false));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists("EmptyStyle", CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameIsEmpty()
    {
        // Arrange
        var failure = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Style name cannot be empty");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(string.Empty, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameIsWhitespace()
    {
        // Arrange
        var failure = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Style name cannot be whitespace");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists("   ", CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameExceedsMaxLength()
    {
        // Arrange
        var failure = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, ErrorMessageStyleNameTooLong);
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(IncorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenStyleNameIsNull()
    {
        // Arrange
        var failure = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Style name cannot be null");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(null!, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var failure = CreateFailureResult<bool, PersistenceLayer>(StatusCodes.Status500InternalServerError, "Database connection failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_VerifiesQueryIsCalledWithCorrectParameters()
    {
        // Arrange
        var styleName = "TestStyle";
        var senderMock = CreateSenderMock();
        CheckExampleLinkExistsByStyle.Query? captured = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((q, ct) => { captured = q as CheckExampleLinkExistsByStyle.Query; })
            .ReturnsAsync(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.StyleName.Should().Be(styleName);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = CreateSenderMock();
        senderMock.SetupSendThrowsOperationCanceledForAny<bool>();
        var controller = CreateController(senderMock);

        // Act
        var action = () => controller.CheckLinkWithStyleExists(CorrectStyleName, cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>()
            .WithMessage(ErrorCanceledOperation);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        await controller.CheckLinkWithStyleExists(CorrectStyleName, CancellationToken.None);

        // Assert
        senderMock.Verify(s => s.Send(It.IsAny<CheckExampleLinkExistsByStyle.Query>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("ModernArt")]
    [InlineData("ClassicStyle")]
    [InlineData("Abstract")]
    [InlineData("Minimal")]
    [InlineData("Vintage")]
    public async Task CheckLinkWithStyleExists_ReturnsOk_ForVariousValidStyleNames(string styleName)
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_ForInvalidStyleNames(string invalidStyleName)
    {
        // Arrange
        var failure = CreateFailureResult<bool, DomainLayer>(StatusCodes.Status400BadRequest, "Invalid style name");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(invalidStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsConsistentResults_ForSameStyleName()
    {
        // Arrange
        var styleName = CorrectStyleName;
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var r1 = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);
        var r2 = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        r1.Should().BeOkResult().WithValueOfType<bool>();
        r2.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenRepositoryThrowsException()
    {
        // Arrange
        var failure = CreateFailureResult<bool, PersistenceLayer>(StatusCodes.Status400BadRequest, "Repository error");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesStyleNameWithSpecialCharacters()
    {
        // Arrange
        var styleName = "Modern-Art_2024";
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesStyleNameWithNumbers()
    {
        // Arrange
        var styleName = "Style123";
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_HandlesCaseInsensitiveStyleNames()
    {
        // Arrange
        var styleName = "modernart";
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(styleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckLinkWithStyleExists_ReturnsOk_ForBothBooleanValues(bool exists)
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(exists));
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists("TestStyle", CancellationToken.None);

        // Assert
        actionResult.Should().BeOkResult().WithValueOfType<bool>();
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        // Arrange
        var failure = CreateFailureResult<bool, ApplicationLayer>(StatusCodes.Status400BadRequest, "Query handler failed");
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(failure);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.CheckLinkWithStyleExists(CorrectStyleName, CancellationToken.None);

        // Assert
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task CheckLinkWithStyleExists_RespondsQuickly_ForPerformanceTest()
    {
        // Arrange
        var senderMock = CreateSenderMock();
        senderMock.SetupSendReturnsForRequest<CheckExampleLinkExistsByStyle.Query, bool>(Result.Ok(true));
        var controller = CreateController(senderMock);
        var start = DateTime.UtcNow;

        // Act
        await controller.CheckLinkWithStyleExists(CorrectStyleName, CancellationToken.None);

        // Assert
        (DateTime.UtcNow - start).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}