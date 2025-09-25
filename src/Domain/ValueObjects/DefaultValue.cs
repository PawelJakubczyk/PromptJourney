using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record DefaultValue : ValueObject<string?>, ICreatable<DefaultValue, string?>
{
    public const int MaxLength = 50;

    private DefaultValue(string? value) : base(value) { }

    public static Result<DefaultValue> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new DefaultValue(default(string)));

        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .IfWhitespace<DomainLayer, DefaultValue>(value)
                .IfLengthTooLong<DomainLayer, DefaultValue>(value, MaxLength))
            .ExecuteIfNoErrors<DefaultValue>(() => new DefaultValue(value))
            .MapResult(v => v);

        return result;
    }
}