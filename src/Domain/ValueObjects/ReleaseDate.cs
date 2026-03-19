using Domain.Abstractions;
using Domain.Extensions;
using System.Globalization;
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
            .IfNullOrWhitespace<ReleaseDate>(value)
            .IfDateFormatInvalid<ReleaseDate>(value!)
            .ExecuteIfNoErrors<ReleaseDate>(() => new ReleaseDate(
                DateTimeOffset.Parse(value!, null, DateTimeStyles.AssumeUniversal)))
            .MapResult<ReleaseDate>();

        return result;
    }
}