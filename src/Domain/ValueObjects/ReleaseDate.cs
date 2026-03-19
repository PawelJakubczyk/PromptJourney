using Domain.Abstractions;
using Domain.Errors;
using Domain.Extensions;
using System.Globalization;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record ReleaseDate : ValueObject<DateTimeOffset>, ICreatable<ReleaseDate, string?>
{
    public override bool IsNone => false;
    private ReleaseDate(DateTimeOffset value) : base(value) { }

    public static Result<ReleaseDate> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfReleaseDateNullOrWhitespace(value)
            .IfDateFormatInvalid(value!)
            .ExecuteIfNoErrors<ReleaseDate>(() => new ReleaseDate(
                DateTimeOffset.Parse(value!, null, DateTimeStyles.AssumeUniversal)))
            .MapResult<ReleaseDate>();

        return result;
    }
}

internal static class ReleaseDateErrorsExtensions
{
    internal static WorkflowPipeline IfDateFormatInvalid
    (
        this WorkflowPipeline pipeline,
        string input
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        var isValid = DateTimeOffset.TryParse(
            input,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal,
            out _
        );

        if (!isValid)
        {
            pipeline.Errors.Add(DomainErrors.InvalidReleaseDateFormat(input));
        }

        return pipeline;
    }
}