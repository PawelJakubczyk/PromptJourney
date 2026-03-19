using Domain.Abstractions;
using Domain.Extensions;
using System.Globalization;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record CreatedOn : ValueObject<DateTimeOffset>, ICreatable<CreatedOn, string?>
{
    public override bool IsNone => false;
    private CreatedOn(DateTimeOffset value) : base(value) { }

    public static Result<CreatedOn> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<CreatedOn>(value)
            .IfDateFormatInvalid<CreatedOn>(value!)
            .ExecuteIfNoErrors<CreatedOn>(() => new CreatedOn(
                DateTimeOffset.Parse(value!, null, DateTimeStyles.AssumeUniversal)))
            .MapResult<CreatedOn>();

        return result;
    }
}