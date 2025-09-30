using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record MinValue : ValueObject<string?>, ICreatable<MinValue, string?>
{
    public const int MaxLength = 50;

    private MinValue(string? value) : base(value) { }

    public static Result<MinValue> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfWhitespace<DomainLayer, MinValue>(value)
            .IfLengthTooLong<DomainLayer, MinValue>(value, MaxLength)
            .ExecuteIfNoErrors<MinValue>(() => new MinValue(value))
            .MapResult(v => v);

        return result;
    }
}
