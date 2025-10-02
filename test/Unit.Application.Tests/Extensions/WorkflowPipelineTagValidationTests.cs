using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Moq;
using Utilities.Workflows;
using FluentAssertions;

namespace Unit.Application.Tests.Extensions;

public class WorkflowPipelineTagValidationTests
{
    // Setup

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    // IfTagAlreadyExists Tests

    [Fact]
    public async Task IfTagAlreadyExists_ShouldAddError_WhenTagExists()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var style = StyleName.Create("Modern").Value;
        var tag = Tag.Create("Bold").Value;

        var repo = new Mock<IStyleRepository>();
        repo.Setup(r => r.CheckTagExistsInStyleAsync(style, tag, _cancellationToken))
            .ReturnsAsync(Result.Ok(true));

        // Act
        var result = await pipelineTask.IfTagAlreadyExists(style, tag, repo.Object, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Tag in style 'Modern' 'Bold' already exists"));
    }

    // IfTagNotExist Tests

    [Fact]
    public async Task IfTagNotExist_ShouldAddError_WhenTagDoesNotExist()
    {
        // Arrange
        var pipelineTask = Task.FromResult(WorkflowPipeline.Create([], false));
        var style = StyleName.Create("Classic").Value;
        var tag = Tag.Create("Italic").Value;

        var repo = new Mock<IStyleRepository>();
        repo.Setup(r => r.CheckTagExistsInStyleAsync(style, tag, _cancellationToken))
            .ReturnsAsync(Result.Ok(false));

        // Act
        var result = await pipelineTask.IfTagNotExist(style, tag, repo.Object, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Tag in style 'Classic' 'Italic' not found"));
    }
}