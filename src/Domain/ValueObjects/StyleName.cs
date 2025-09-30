using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record StyleName : ValueObject<string?>, ICreatable<StyleName, string?>
{
    public const int MaxLength = 150;

    private StyleName(string? value) : base(value) { }

    public static Result<StyleName> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, StyleName>(value)
            .IfLengthTooLong<DomainLayer, StyleName>(value, MaxLength)
            .ExecuteIfNoErrors<StyleName>(() => new StyleName(value))
            .MapResult(s => s);

        return result;
    }
}