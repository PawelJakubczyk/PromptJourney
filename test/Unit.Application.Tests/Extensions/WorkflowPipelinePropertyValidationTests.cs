using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Moq;
using Utilities.Workflows;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelinePropertyValidationTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task IfPropertyAlreadyExists_ShouldAddError_WhenPropertyExists()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var property = PropertyName.Create("Height").Value;
        var version = ModelVersion.Create("1").Value;

        var repo = new Mock<IPropertiesRepository>();
        repo.Setup(r => r.CheckPropertyExistsInVersionAsync(version, property, _cancellationToken)).ReturnsAsync(Result.Ok(true));

        var result = await pipelineTask.IfPropertyAlreadyExists(property, version, repo.Object, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("PropertyName 'Height' already exists"));
    }

    [Fact]
    public async Task IfPropertyNotExists_ShouldAddError_WhenPropertyDoesNotExist()
    {
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var property = PropertyName.Create("Width").Value;
        var version = ModelVersion.Create("2").Value;

        var repo = new Mock<IPropertiesRepository>();
        repo.Setup(r => r.CheckPropertyExistsInVersionAsync(version, property, _cancellationToken)).ReturnsAsync(Result.Ok(false));

        var result = await pipelineTask.IfPropertyNotExists(property, version, repo.Object, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("PropertyName 'Width' not found"));
    }
}