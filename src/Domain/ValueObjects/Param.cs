using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record Param : ValueObject<string?>, ICreatable<Param, string?>
{
    public const int MaxLength = 100;

    private Param(string? value) : base(value) { }

    public static Result<Param> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, Param>(value)
            .IfLengthTooLong<DomainLayer, Param>(value, MaxLength)
            .ExecuteIfNoErrors<Param>(() => new Param(value))
            .MapResult(p => p);

        return result;
    }
}
