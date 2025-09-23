using FluentResults;

namespace Utilities.Validation;

public class ValidationPipeline
{
    public Task<List<Error>> ErrorsTask { get; }
    public List<Error> Errors { get; }
    public bool BreakOnError { get; }

    private ValidationPipeline(List<Error> errors, bool breakOnError)
    {
        Errors = errors ?? [];
        BreakOnError = breakOnError;
    }

    public static ValidationPipeline Create(List<Error> errors, bool breakOnError = true) =>
        new ValidationPipeline(errors, breakOnError);

    public static ValidationPipeline Empty() =>
        Create([], breakOnError: true);

    public static Task<ValidationPipeline> EmptyAsync()
    {
        return Task.FromResult(Create([], breakOnError: true));
    }
}

