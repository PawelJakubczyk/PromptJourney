using Domain.Abstractions;
using System.Globalization;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record ReleaseDate : ValueObject<DateTimeOffset?>, ICreatable<ReleaseDate?, string?>
{
    private ReleaseDate(DateTimeOffset? value) : base(value) { }

    public static Result<ReleaseDate?> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            value = null;
        }

        var result = WorkflowPipeline
            .Empty()
            .IfDateFormatInvalid<DomainLayer>(value)
            .ExecuteIfNoErrors<ReleaseDate>
                (() => new ReleaseDate(value == null
                    ? null
                    : DateTimeOffset.Parse(value, null, DateTimeStyles.AssumeUniversal)))
            .MapResult<ReleaseDate?>();

        return result;
    }
}

internal static class ReleaseDateErrorsExtensions
{
    internal static WorkflowPipeline IfDateFormatInvalid<TLayer>
    (
        this WorkflowPipeline pipeline,
        string? input
    ) where TLayer : ILayer
    {
        if (pipeline.BreakOnError)
            return pipeline;

        // ---- skip if null/empty -> handled by Required validator ----
        if (string.IsNullOrWhiteSpace(input))
            return pipeline;

        var isValid = DateTimeOffset.TryParse(
            input,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal,
            out _
        );

        if (!isValid)
        {
            pipeline.Errors.Add(
                ErrorFactories.InvalidDateFormat<ReleaseDate, DomainLayer>(
                    input
                )
            );
        }

        return pipeline;
    }

}