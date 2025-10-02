using FluentAssertions;
using FluentResults;
using Utilities.Workflows;

namespace Unit.Utilities.Tests.Workflow;

public class WorkflowPipelineTests
{
    [Fact]
    public void Create_WithEmptyErrors_ShouldReturnPipelineWithEmptyErrors()
    {
        // Arrange
        var errors = new List<Error>();

        // Act
        var pipeline = WorkflowPipeline.Create(errors, breakOnError: true);

        // Assert
        pipeline.Should().NotBeNull();
        pipeline.Errors.Should().BeEmpty();
        pipeline.BreakOnError.Should().BeTrue();
    }

    [Fact]
    public void Create_WithErrors_ShouldReturnPipelineWithErrors()
    {
        // Arrange
        var errors = new List<Error>
        {
            new Error("Error 1"),
            new Error("Error 2")
        };

        // Act
        var pipeline = WorkflowPipeline.Create(errors, breakOnError: false);

        // Assert
        pipeline.Should().NotBeNull();
        pipeline.Errors.Should().HaveCount(2);
        pipeline.Errors[0].Message.Should().Be("Error 1");
        pipeline.Errors[1].Message.Should().Be("Error 2");
        pipeline.BreakOnError.Should().BeFalse();
    }

    [Fact]
    public void Create_WithNullErrors_ShouldReturnPipelineWithEmptyErrors()
    {
        // Act
        var pipeline = WorkflowPipeline.Create(null, breakOnError: true);

        // Assert
        pipeline.Should().NotBeNull();
        pipeline.Errors.Should().BeEmpty();
        pipeline.BreakOnError.Should().BeTrue();
    }

    [Fact]
    public void Create_WithDefaultBreakOnError_ShouldDefaultToTrue()
    {
        // Arrange
        var errors = new List<Error>();

        // Act
        var pipeline = WorkflowPipeline.Create(errors);

        // Assert
        pipeline.BreakOnError.Should().BeTrue();
    }

    [Fact]
    public void Empty_ShouldReturnPipelineWithNoErrors()
    {
        // Act
        var pipeline = WorkflowPipeline.Empty();

        // Assert
        pipeline.Should().NotBeNull();
        pipeline.Errors.Should().BeEmpty();
        pipeline.BreakOnError.Should().BeTrue();
    }

    [Fact]
    public async Task EmptyAsync_ShouldReturnTaskWithEmptyPipeline()
    {
        // Act
        var pipelineTask = WorkflowPipeline.EmptyAsync();
        var pipeline = await pipelineTask;

        // Assert
        pipelineTask.Should().NotBeNull();
        pipeline.Should().NotBeNull();
        pipeline.Errors.Should().BeEmpty();
        pipeline.BreakOnError.Should().BeTrue();
    }

    [Fact]
    public void Errors_ShouldBeReadOnly_AfterCreation()
    {
        // Arrange
        var originalErrors = new List<Error> { new Error("Test") };
        var pipeline = WorkflowPipeline.Create(originalErrors);

        // Act & Assert
        pipeline.Errors.Should().HaveCount(1);

        // Modifying original list should not affect pipeline
        originalErrors.Add(new Error("New error"));
        pipeline.Errors.Should().HaveCount(1); // Should remain unchanged
    }

    [Fact]
    public void Multiple_Create_Calls_ShouldReturnIndependentInstances()
    {
        // Arrange
        var errors1 = new List<Error> { new Error("Error 1") };
        var errors2 = new List<Error> { new Error("Error 2") };

        // Act
        var pipeline1 = WorkflowPipeline.Create(errors1);
        var pipeline2 = WorkflowPipeline.Create(errors2);

        // Assert
        pipeline1.Should().NotBeSameAs(pipeline2);
        pipeline1.Errors.Should().HaveCount(1);
        pipeline2.Errors.Should().HaveCount(1);
        pipeline1.Errors[0].Message.Should().Be("Error 1");
        pipeline2.Errors[0].Message.Should().Be("Error 2");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Create_ShouldRespectBreakOnErrorParameter(bool breakOnError)
    {
        // Act
        var pipeline = WorkflowPipeline.Create([], breakOnError);

        // Assert
        pipeline.BreakOnError.Should().Be(breakOnError);
    }

    [Fact]
    public void WorkflowPipeline_ShouldMaintainErrorOrder()
    {
        // Arrange
        var errors = new List<Error>
        {
            new Error("First error"),
            new Error("Second error"),
            new Error("Third error")
        };

        // Act
        var pipeline = WorkflowPipeline.Create(errors);

        // Assert
        pipeline.Errors.Should().HaveCount(3);
        pipeline.Errors[0].Message.Should().Be("First error");
        pipeline.Errors[1].Message.Should().Be("Second error");
        pipeline.Errors[2].Message.Should().Be("Third error");
    }

    [Fact]
    public void WorkflowPipeline_ShouldHandleLargeNumberOfErrors()
    {
        // Arrange
        var errors = new List<Error>();
        for (int i = 0; i < 1000; i++)
        {
            errors.Add(new Error($"Error {i}"));
        }

        // Act
        var pipeline = WorkflowPipeline.Create(errors);

        // Assert
        pipeline.Errors.Should().HaveCount(1000);
        pipeline.Errors[0].Message.Should().Be("Error 0");
        pipeline.Errors[999].Message.Should().Be("Error 999");
    }
}