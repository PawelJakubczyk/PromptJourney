using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Moq;
using Utilities.Workflows;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelineStyleValidationTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task IfStyleAlreadyExists_ShouldAddError_WhenStyleExists()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var style = StyleName.Create("ExistingStyle").Value;

        var repo = new Mock<IStyleRepository>();
        repo.Setup(r => r.CheckStyleExistsAsync(style, _cancellationToken)).ReturnsAsync(Result.Ok(true));

        var result = await pipelineTask.IfStyleAlreadyExists(style, repo.Object, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("Style 'ExistingStyle' already exists"));
    }

    [Fact]
    public async Task IfStyleNotExists_ShouldAddError_WhenStyleDoesNotExist()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var style = StyleName.Create("MissingStyle").Value;

        var repo = new Mock<IStyleRepository>();
        repo.Setup(r => r.CheckStyleExistsAsync(style, _cancellationToken)).ReturnsAsync(Result.Ok(false));

        var result = await pipelineTask.IfStyleNotExists(style, repo.Object, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("Style 'MissingStyle' not found"));
    }

}
