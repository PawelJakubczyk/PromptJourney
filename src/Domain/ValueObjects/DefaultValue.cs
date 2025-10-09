using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record DefaultValue : ValueObject<string?>, ICreatable<DefaultValue, string?>
{
    public const int MaxLength = 50;

    private DefaultValue(string? value) : base(value) { }

    public static Result<DefaultValue> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfWhitespace<DomainLayer, DefaultValue>(value)
            .IfLengthTooLong<DomainLayer, DefaultValue>(value, MaxLength)
            .ExecuteIfNoErrors<DefaultValue>(() => new DefaultValue(value))
            .MapResult<DefaultValue>();

        return result;
    }
}