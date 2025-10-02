using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.ValueObjects;
using FluentAssertions;
using FluentResults;
using Moq;
using Utilities.Workflows;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelineLinkValidationTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task IfLinkAlreadyExists_ShouldAddError_WhenLinkExists()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var link = ExampleLink.Create("http://example.com").Value;

        var repo = new Mock<IExampleLinksRepository>();
        repo.Setup(r => r.CheckExampleLinkExistsAsync(link, _cancellationToken)).ReturnsAsync(Result.Ok(true));

        var result = await pipelineTask.IfLinkAlreadyExists(link, repo.Object, _cancellationToken);

        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Link 'http://example.com' already exists");
    }

    [Fact]
    public async Task IfLinkNotExists_ShouldAddError_WhenLinkDoesNotExist()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var link = ExampleLink.Create("http://missing.com").Value;

        var repo = new Mock<IExampleLinksRepository>();
        repo.Setup(r => r.CheckExampleLinkExistsAsync(link, _cancellationToken)).ReturnsAsync(Result.Ok(false));

        var result = await pipelineTask.IfLinkNotExists(link, repo.Object, _cancellationToken);

        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("Link 'http://missing.com' not found");
    }
}
