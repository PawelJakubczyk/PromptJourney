using FluentResults;

namespace Utilities.ValidationPipeline;

public class ValidationPipeline
{
    protected static Task<List<Error>> ErrorsTask { get; private set; }
    protected static bool BreakOnError { get; private set; }

    private ValidationPipeline(Task<List<Error>> errorsTask, bool breakOnError)
    {
        ErrorsTask = errorsTask;
        BreakOnError = breakOnError;
    }

    public static ValidationPipeline Empty()
    {
        return new(Task.FromResult(new List<Error>()), breakOnError: true);
    }

    public static ValidationPipeline BeginValidationBlock()
    {
        return new(ErrorsTask, breakOnError: false);
    }

    public static ValidationPipeline EndValidationBlock()
    {
        return new(ErrorsTask, breakOnError: true);
    }

    public static ValidationPipeline ColectErrors(Task<List<Error>> errors)
    {

    }
}

