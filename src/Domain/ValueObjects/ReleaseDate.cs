using Domain.Abstractions;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Workflows;
using Utilities.Results;
using Domain.Extensions;

namespace Domain.ValueObjects;

public record ReleaseDate : ValueObject<string>, ICreatable<ReleaseDate, string?>
{
    private ReleaseDate(string value) : base(value) { }

    public static Result<ReleaseDate> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfDateFormatInvalid<DomainLayer>(value)
            .MapResult<ReleaseDate>();

        return result;
    }

    public DateTime ToDateTime()
    {
        return DateTime.Parse(Value, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
}

internal static class ReleaseDateErrorsExtensions
{
    internal const string InvalidDateFormatMessage = "Invalid date format (expected ISO 8601)";

    internal static WorkflowPipeline IfDateFormatInvalid<TLayer>
    (
        this WorkflowPipeline pipeline,
        string? value
    ) where TLayer : ILayer
    {
        if (pipeline.BreakOnError)
            return pipeline;

        // Check whether the date is in the correct ISO 8601 format
        var isValid = DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out _);

        if (!isValid)
        {
            pipeline.Errors.Add(
                ErrorFactories.InvalidPattern<string, TLayer>(value, InvalidDateFormatMessage)
            );
        }

        return pipeline;
    }
}