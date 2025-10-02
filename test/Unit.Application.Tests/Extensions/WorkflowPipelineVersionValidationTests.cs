using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Moq;
using Application.Extensions;
using Utilities.Workflows;

namespace Unit.Application.Tests.Extensions;
public class WorkflowPipelineVersionValidationTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task IfVersionAlreadyExists_ShouldAddError_WhenVersionExists()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var version = ModelVersion.Create("1").Value;

        var repo = new Mock<IVersionRepository>();
        repo.Setup(r => r.CheckVersionExistsInVersionsAsync(version, _cancellationToken)).ReturnsAsync(Result.Ok(true));

        var result = await pipelineTask.IfVersionAlreadyExists(version, repo.Object, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("Version '1' already exists"));
    }

    [Fact]
    public async Task IfVersionNotExists_ShouldAddError_WhenVersionDoesNotExist()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var version = ModelVersion.Create("2").Value;

        var repo = new Mock<IVersionRepository>();
        repo.Setup(r => r.CheckVersionExistsInVersionsAsync(version, _cancellationToken)).ReturnsAsync(Result.Ok(false));

        var result = await pipelineTask.IfVersionNotExists(version, repo.Object, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("Version '2' not found"));
    }

}
