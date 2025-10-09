using FluentResults;

namespace Utilities.Workflows;

public class WorkflowPipeline
{
    private readonly Dictionary<string, object> _results = [];
    public List<Error> Errors { get; }
    public bool BreakOnError { get; }

    private WorkflowPipeline(List<Error> errors, bool breakOnError)
    {
        Errors = errors ?? [];
        BreakOnError = breakOnError;
    }

    public T? GetResult<T>(string? key = null)
    {
        var resultKey = key ?? typeof(T).FullName!;
        return _results.TryGetValue(resultKey, out var value) ? (T)value : default;
    }

    public void SetResult<T>(T value, string? key = null)
    {
        var resultKey = key ?? typeof(T).FullName!;
        _results[resultKey] = value!;
    }

    public static WorkflowPipeline Create(List<Error> errors, bool breakOnError = true) =>
        new(errors, breakOnError);

    public static WorkflowPipeline Empty() =>
        Create([], breakOnError: true);

    public static Task<WorkflowPipeline> EmptyAsync() =>
        Task.FromResult(Create([], breakOnError: true));
}

