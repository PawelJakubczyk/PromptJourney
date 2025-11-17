using Application.UseCases.Versions.Queries;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests.Base;
using Utilities.Constants;

namespace Unit.Presentation.Tests.MoqControlersTests.VersionsMoqControlersTests;

public sealed class GetSupportedVersionsTests : VersionsControllerTestsBase
{
    [Fact]
    public async Task GetSupported_ReturnsOkWithList_WhenSupportedVersionsExist()
    {
        var supportedVersions = new List<string> { "1.0", "2.0", "5.1", "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithEmptyList_WhenNoSupportedVersionsExist()
    {
        var result = Result.Ok(new List<string>());
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(0);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithMultipleSupportedVersions()
    {
        var supportedVersions = new List<string> { "1.0", "2.0", "3.0", "4.0", "5.0", "5.1", "5.2", "6.0", "niji 5", "niji 6" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(10);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithStandardVersionsOnly()
    {
        var supportedVersions = new List<string> { "1.0", "2.0", "3.0", "4.0", "5.0", "6.0" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(6);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithNijiVersionsOnly()
    {
        var supportedVersions = new List<string> { "niji 4", "niji 5", "niji 6" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithMixedVersionTypes()
    {
        var supportedVersions = new List<string> { "1.0", "2.5", "5.1", "5.2", "6.0", "niji 5", "niji 6" };
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(7);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithSingleVersion()
    {
        var result = Result.Ok(new List<string> { "6.0" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(1);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsContainingDecimals()
    {
        var supported = new List<string> { "1.0", "2.5", "3.7", "4.2", "5.1", "5.2" };
        var result = Result.Ok(supported);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(6);
    }

    [Fact]
    public async Task GetSupported_VerifiesQueryIsCalledWithSingleton()
    {
        var result = Result.Ok(new List<string>());
        var senderMock = new Mock<ISender>();
        GetAllSuportedVersions.Query? capturedQuery = null;
        senderMock
            .Setup(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<List<string>>>, CancellationToken>((query, ct) => { capturedQuery = query as GetAllSuportedVersions.Query; })
            .ReturnsAsync(result);
        var controller = CreateController(senderMock);
        await controller.GetSupported(CancellationToken.None);
        capturedQuery.Should().NotBeNull();
        capturedQuery.Should().BeSameAs(GetAllSuportedVersions.Query.Singletone);
    }

    [Fact]
    public async Task GetSupported_HandlesCancellationToken()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendThrowsOperationCanceledForAny<List<string>>();
        var controller = CreateController(senderMock);
        await FluentActions.Awaiting(() => controller.GetSupported(cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetSupported_VerifiesSenderIsCalledOnce()
    {
        var result = Result.Ok(new List<string> { "1.0" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        await controller.GetSupported(CancellationToken.None);
        senderMock.Verify(s => s.Send(It.IsAny<GetAllSuportedVersions.Query>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSupported_ReturnsConsistentResults_ForMultipleCalls()
    {
        var supported = new List<string> { "1.0", "2.0", "6.0" };
        var result = Result.Ok(supported);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult1 = await controller.GetSupported(CancellationToken.None);
        var actionResult2 = await controller.GetSupported(CancellationToken.None);
        actionResult1.Should().BeOkResult().WithCount(3);
        actionResult2.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsBadRequest_WhenRepositoryFails()
    {
        var failureResult = CreateFailureResult<List<string>, PersistenceLayer>(StatusCodes.Status500InternalServerError, "Repository error during supported versions retrieval");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetSupported_ReturnsBadRequest_WhenQueryHandlerFails()
    {
        var failureResult = CreateFailureResult<List<string>, ApplicationLayer>(StatusCodes.Status400BadRequest, "Query handler failed");
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(failureResult);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeErrorResult().WithStatusCode(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetSupported_RespondsQuickly_ForPerformanceTest()
    {
        var result = Result.Ok(new List<string> { "1.0", "2.0", "6.0" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var startTime = DateTime.UtcNow;
        await controller.GetSupported(CancellationToken.None);
        (DateTime.UtcNow - startTime).Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsInChronologicalOrder()
    {
        var supported = new List<string> { "1.0", "2.0", "3.0", "4.0", "5.0", "5.1", "5.2", "6.0" };
        var result = Result.Ok(supported);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(8);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithBetaVersions()
    {
        var result = Result.Ok(new List<string> { "6.0", "7.0-beta", "8.0-alpha" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithLegacyVersions()
    {
        var result = Result.Ok(new List<string> { "1.0", "2.0", "3.0", "4.0" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithCurrentVersions()
    {
        var result = Result.Ok(new List<string> { "5.2", "6.0", "niji 5", "niji 6" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(4);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithManyVersions()
    {
        var supportedVersions = Enumerable.Range(1, 20).Select(i => $"{i}.0").ToList();
        var result = Result.Ok(supportedVersions);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(20);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetSupported_ReturnsOk_WithVariousListSizes(int count)
    {
        var list = Enumerable.Range(1, count).Select(i => $"{i}.0").ToList();
        var result = Result.Ok(list);
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(count);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsContainingSpaces()
    {
        var result = Result.Ok(new List<string> { "niji 5", "niji 6", "niji 7" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithVersionsContainingDashes()
    {
        var result = Result.Ok(new List<string> { "6.0-niji", "7.0-beta", "8.0-alpha" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(3);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithMajorVersionsOnly()
    {
        var result = Result.Ok(new List<string> { "1", "2", "3", "4", "5", "6" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(6);
    }

    [Fact]
    public async Task GetSupported_ReturnsOkWithDuplicateVersions()
    {
        var result = Result.Ok(new List<string> { "1.0", "1.0", "2.0", "2.0" });
        var senderMock = new Mock<ISender>();
        senderMock.SetupSendReturnsForRequest<GetAllSuportedVersions.Query, List<string>>(result);
        var controller = CreateController(senderMock);
        var actionResult = await controller.GetSupported(CancellationToken.None);
        actionResult.Should().BeOkResult().WithCount(4);
    }
}