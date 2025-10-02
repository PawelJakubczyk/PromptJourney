using FluentResults;

namespace Utilities.Workflows;

public class WorkflowPipeline
{
    public List<Error> Errors { get; }

    public bool BreakOnError { get; }

    private WorkflowPipeline(List<Error> errors, bool breakOnError)
    {
        Errors = errors ?? [];
        BreakOnError = breakOnError;
    }

    public static WorkflowPipeline Create(List<Error> errors, bool breakOnError = true) =>
        new WorkflowPipeline(errors, breakOnError);

    public static WorkflowPipeline Empty() =>
        Create([], breakOnError: true);

    public static Task<WorkflowPipeline> EmptyAsync()
    {
        return Task.FromResult(Create([], breakOnError: true));
    }
}

