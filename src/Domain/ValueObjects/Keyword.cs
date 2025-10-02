using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Keyword : ValueObject<string?>, ICreatable<Keyword, string?>
{
    public const int MaxLength = 50;

    private Keyword(string value) : base(value) { }

    public static Result<Keyword> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, Keyword>(value)
            .IfLengthTooLong<DomainLayer, Keyword>(value, MaxLength)
            .ExecuteIfNoErrors<Keyword>(() => new Keyword(value!))
            .MapResult(k => k);

        return result;
    }
}
