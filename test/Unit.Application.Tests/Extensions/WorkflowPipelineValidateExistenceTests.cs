using Application.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Moq;
using Utilities.Workflows;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelineValidateExistenceTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task ValidateExistence_ShouldAddError_WhenCheckFails()
    {
        // Arrange
        var pipeline = WorkflowPipeline.Create([], breakOnError: false);
        var pipelineTask = Task.FromResult(pipeline);
        var item = StyleName.Create("TestStyle").Value;

        var mockFunc = new Mock<Func<StyleName, CancellationToken, Task<Result<bool>>>>();
        mockFunc.Setup(f => f(item, _cancellationToken)).ReturnsAsync(Result.Fail<bool>("Database error"));

        // Act
        var result = await pipelineTask.ValidateExistence(item, mockFunc.Object, "Style", true, _cancellationToken);

        // Assert
        Assert.Contains(result.Errors, e => e.Message.Contains("Failed to check if Style exists"));
    }

    [Fact]
    public async Task ValidateExistence_ShouldAddNotFoundError_WhenItemDoesNotExist_AndShouldExist()
    {
        var pipeline = WorkflowPipeline.Create([], breakOnError: false);
        var pipelineTask = Task.FromResult(pipeline);
        var item = StyleName.Create("MissingStyle").Value;

        var mockFunc = new Mock<Func<StyleName, CancellationToken, Task<Result<bool>>>>();
        mockFunc.Setup(f => f(item, _cancellationToken)).ReturnsAsync(Result.Ok(false));

        var result = await pipelineTask.ValidateExistence(item, mockFunc.Object, "Style", true, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("Style 'MissingStyle' not found"));
    }

    [Fact]
    public async Task ValidateExistence_ShouldAddConflictError_WhenItemExists_AndShouldNotExist()
    {
        var pipeline = WorkflowPipeline.Create([], breakOnError: false);
        var pipelineTask = Task.FromResult(pipeline);
        var item = StyleName.Create("ExistingStyle").Value;

        var mockFunc = new Mock<Func<StyleName, CancellationToken, Task<Result<bool>>>>();
        mockFunc.Setup(f => f(item, _cancellationToken)).ReturnsAsync(Result.Ok(true));

        var result = await pipelineTask.ValidateExistence(item, mockFunc.Object, "Style", false, _cancellationToken);

        Assert.Contains(result.Errors, e => e.Message.Contains("Style 'ExistingStyle' already exists"));
    }

    [Fact]
    public async Task ValidateExistence_ShouldNotAddError_WhenConditionIsMet()
    {
        var pipeline = WorkflowPipeline.Create([], breakOnError: false);
        var pipelineTask = Task.FromResult(pipeline);
        var item = StyleName.Create("ValidStyle").Value;

        var mockFunc = new Mock<Func<StyleName, CancellationToken, Task<Result<bool>>>>();
        mockFunc.Setup(f => f(item, _cancellationToken)).ReturnsAsync(Result.Ok(true));

        var result = await pipelineTask.ValidateExistence(item, mockFunc.Object, "Style", true, _cancellationToken);

        Assert.Empty(result.Errors);
    }
}