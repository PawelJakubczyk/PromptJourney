using Application.UseCases.Versions.Queries;
using FluentAssertions;
using MediatR;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Results;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

public sealed class GetSupportedVersionsTests : VersionsControllerTestsBase
{
    [Theory]
    [InlineData(new[] { "1.0", "2.0", "3.0", "4.0", "5.0", "5.1", "5.2", "6.0", "niji 5", "niji 6" }, 10)]
    [InlineData(new[] { "1.0", "2.0", "3.0" }, 3)]
    [InlineData(new[] { "niji 4", "niji 5" }, 2)]
    public async Task GetSupported_ReturnsOkWithList_WhenSupportedVersionsExist(string[] expectedVersions, int expectedCount)
    {
        // Arrange
        var expectedVersionsList = expectedVersions.ToList();
        var result = Result.Ok(expectedVersionsList);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSupportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithCount(expectedCount)
            .WithValue(expectedVersionsList);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithEmptyList_WhenNoSupportedVersionsExist()
    {
        // Arrange
        var supportedEmpty = new List<string>();
        var result = Result.Ok(supportedEmpty);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSupportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult
            .Should()
            .BeOkResult()
            .WithCount(0)
            .WithValue(supportedEmpty);
    }

    [Fact]
    public async Task GetSupported_ReturnsConsistentResults_ForMultipleCalls()
    {
        // Arrange
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSupportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);

        // Act
        var actionResult1 = await controller.GetSupported(CancellationToken.None);
        var actionResult2 = await controller.GetSupported(CancellationToken.None);

        // Assert
        actionResult1
            .Should()
            .BeOkResult()
            .WithCount(10)
            .WithValue(supportedVersions);

        actionResult2
            .Should()
            .BeOkResult()
            .WithCount(10)
            .WithValue(supportedVersions);
    }

    [Fact]
    public async Task GetSupported_VerifiesQueryIsCalledWithSingleton()
    {
        // Arrange
        var result = Result.Ok(new List<string>());
        var senderMock = new Mock<ISender>();
        GetAllSupportedVersions.Query? capturedQuery = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSupportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<string>>>, CancellationToken>((query, ct) =>
            {
                capturedQuery = query as GetAllSupportedVersions.Query;
            })
            .ReturnsAsync(result);
        var controller = CreateController(senderMock);

        // Act
        await controller.GetSupported(CancellationToken.None);

        // Assert
        capturedQuery
            .Should()
            .NotBeNull();
        capturedQuery
            .Should()
            .BeSameAs(GetAllSupportedVersions.Query.Singleton);
    }

    [Fact]
    public async Task GetSupported_HandlesCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendThrowsOperationCanceledForAny<List<string>>();
        var controller = CreateController(senderMock);

        // Act & Assert
        await FluentActions
            .Awaiting(() => controller.GetSupported(cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetSupported_VerifiesSenderIsCalledOnce()
    {
        // Arrange
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSupportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);

        // Act
        await controller.GetSupported(CancellationToken.None);

        // Assert
        senderMock.Verify(
            s => s.Send(It.IsAny<GetAllSupportedVersions.Query>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}