using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record Prompt : ValueObject<string?>, ICreatable<Prompt, string?>
{
    public const int MaxLength = 1000;

    private Prompt(string? value) : base(value) { }

    public static Result<Prompt> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<DomainLayer, Prompt>(value)
            .IfLengthTooLong<DomainLayer, Prompt>(value, MaxLength)
            .ExecuteIfNoErrors<Prompt>(() => new Prompt(value))
            .MapResult(p => p);

        return result;
    }
}
