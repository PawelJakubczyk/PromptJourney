using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record PropertyName : ValueObject<string?>, ICreatable<PropertyName, string?>
{
    public const int MaxLength = 25;

    private PropertyName(string? value) : base(value) { }

    public static Result<PropertyName> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, PropertyName>(value)
            .IfLengthTooLong<DomainLayer, PropertyName>(value, MaxLength)
            .ExecuteIfNoErrors<PropertyName>(() => new PropertyName(value))
            .MapResult(p => p);

        return result;
    }
}
